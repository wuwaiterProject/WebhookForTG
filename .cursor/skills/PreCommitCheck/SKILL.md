---
name: "pre-commit-check"
description: "Commit 前的四階段驗證：安全性掃描、語法格式檢查、TOML 驗證、單元測試。由 GitCommit Skill 在 Stage 前自動呼叫；tests/ 為空時測試步驟自動跳過。"
---

# Pre-Commit Check — 提交前驗證規範

## 觸發時機

- **自動觸發**：由 GitCommit Skill 的 Step 3.5 呼叫
- **手動觸發**：使用者說「檢查」、「verify」、「跑測試」、「pre-commit」

---

## 執行流程總覽

```
[Check 1] 安全性掃描   — 敏感資料 / 危險函式偵測
[Check 2] Python 語法  — 僅針對本次修改的 .py 檔案
[Check 3] TOML 格式    — 僅當 config.toml 有修改時執行
[Check 4] 單元測試     — tests/ 非空時執行，否則跳過
         ↓
   彙整結果，全部通過才允許進入 GitCommit Stage 步驟
```

任一 **必要** 項目失敗 → **停止，列出問題，等待修正，最多重試 3 次後改為回報**。

---

## Check 1｜安全性掃描

### 1-A 禁止 commit 的檔案

```bash
# 確認 .env 沒有被 staged
git diff --cached --name-only | grep -E "^\.env"

# 確認憑證目錄沒有被 staged
git diff --cached --name-only | grep -E "^cert/"
```

| 禁止檔案 | 原因 |
|---|---|
| `.env` / `.env.*` | API Key、DB 帳密 |
| `cert/*.pem` / `*.key` / `*.crt` | 憑證與私鑰 |

### 1-B 敏感字串掃描（OWASP / hardcoded secrets）

針對 staged 的變更內容掃描以下模式：

```bash
git diff --cached | grep -inE \
  "(password|passwd|api_key|apikey|secret|token|private_key)\s*=\s*['\"][^'\"]+"
```

### 1-C 危險函式掃描（參考 alirezarezvani/skill-security-auditor）

僅針對本次修改的 `.py` 檔案：

```bash
CHANGED_PY=$(git diff --cached --name-only | grep "\.py$")

# 掃描危險呼叫
grep -n "eval\|exec\|os\.system\|subprocess\.call\|pickle\.loads\|yaml\.load(" \
  $CHANGED_PY 2>/dev/null
```

> `yaml.safe_load` 為安全用法，不納入告警。
> 若命中，**回報給使用者確認是否為預期用法**，不自動 FAIL，由使用者決定是否繼續。

---

## Check 2｜Python 語法檢查

**僅對本次修改的 `.py` 檔案執行**，未修改的檔案不重複檢查。

```bash
# 取得本次修改的 .py 檔案
CHANGED_PY=$(git diff --name-only HEAD | grep "\.py$")
STAGED_PY=$(git diff --cached --name-only | grep "\.py$")
ALL_PY=$(echo "$CHANGED_PY $STAGED_PY" | tr ' ' '\n' | sort -u)

# 無 .py 變更 → 跳過
if [ -z "$ALL_PY" ]; then
  echo "[⏭️] Python 語法   無 .py 變更，跳過"
  exit 0
fi

# 語法檢查（內建，無需安裝）
for f in $ALL_PY; do
  python -m py_compile "$f" && echo "  ✅ $f" || echo "  ❌ $f"
done
```

> **升級選項**（若已安裝）：
> - `ruff check $ALL_PY` — 更完整的 lint（包含未使用變數、import 順序）
> - `mypy $ALL_PY --ignore-missing-imports` — 型別檢查

---

## Check 3｜TOML 格式驗證

**僅當 `config.toml` 有修改時執行。**

```bash
# 確認 config.toml 是否在本次修改範圍內
git diff --name-only HEAD | grep -q "config.toml" || {
  echo "[⏭️] TOML 格式     config.toml 未修改，跳過"
  exit 0
}

# 驗證格式（使用已安裝的 tomli）
python -c "
import tomli, sys
try:
    with open('config.toml', 'rb') as f:
        tomli.load(f)
    print('[✅] TOML 格式     config.toml OK')
except Exception as e:
    print(f'[❌] TOML 格式     config.toml 解析失敗：{e}')
    sys.exit(1)
"
```

---

## Check 4｜單元測試

```bash
# 檢查 tests/ 是否有測試檔案
TEST_FILES=$(find tests/ -name "test_*.py" -o -name "*_test.py" 2>/dev/null)

if [ -z "$TEST_FILES" ]; then
  echo "[⏭️] 單元測試      tests/ 為空，跳過"
  exit 0
fi

# 執行測試（使用 pytest）
python -m pytest tests/ -v --tb=short

# 失敗時最多重試 3 次
# 第 1-2 次：嘗試修正並重跑
# 第 3 次仍失敗：停止，回報問題，不進行 commit
```

> **重試上限規則**（參考 dev.to QA workflow）：
> - 測試失敗分類：`test bug`（測試本身寫錯）/ `app bug`（程式碼有問題）/ `flakiness`（不穩定）
> - 分類後嘗試修正，最多 **3 次**；第 3 次仍失敗則回報給使用者，停止 commit

---

## 結果彙整輸出格式

```
╔══════════════════════════════════════════╗
║         Pre-Commit Check 結果            ║
╠══════════════════════════════════════════╣
║ [✅] 安全性掃描    無 .env / 敏感字串    ║
║ [✅] Python 語法   2 個檔案通過          ║
║ [✅] TOML 格式     config.toml OK        ║
║ [⏭️] 單元測試      tests/ 為空，跳過     ║
╠══════════════════════════════════════════╣
║  結果：PASS → 進入 GitCommit 流程        ║
╚══════════════════════════════════════════╝
```

| 狀態符號 | 意義 |
|---|---|
| `✅` | 通過 |
| `❌` | 失敗（停止流程） |
| `⚠️` | 警告（需使用者確認後決定是否繼續） |
| `⏭️` | 跳過（條件不符，非失敗） |

---

## 各檢查的必要性與跳過條件

| 檢查 | 必要？ | 跳過條件 |
|---|---|---|
| 安全性掃描 | ✅ 必要，不可跳過 | 無 |
| Python 語法 | ✅ 必要 | 本次無 `.py` 變更 |
| TOML 格式 | ✅ 必要 | `config.toml` 未修改 |
| 危險函式 | ⚠️ 警告 | 無 `.py` 變更 |
| 單元測試 | ✅ 必要 | `tests/` 為空 |

---

## 與 GitCommit Skill 的整合位置

```
GitCommit Flow
  Step 1｜收集變更資訊
  Step 2｜產生 Branch Name
  Step 3｜建立分支
  ──────────────────────────────
  Step 3.5｜[Pre-Commit Check]   ← 本 Skill
    PASS → 繼續
    FAIL → 中止，回報問題
  ──────────────────────────────
  Step 4｜Stage 檔案
  Step 5｜產生 Commit Message
  Step 6｜執行 Commit
  Step 7｜完成回報
```
