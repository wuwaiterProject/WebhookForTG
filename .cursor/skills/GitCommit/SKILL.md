---
name: "git-commit"
description: "git commit 工作流：依據修改內容自動建立本地分支、產生結構化 commit 訊息（標題＋摘要＋逐檔說明）。不自動 push 至 origin，由使用者自行決定推送時機。"
---

# Git Commit — Wade 工作流規範

## 觸發時機

使用者說以下任何詞彙時觸發：
「commit」、「提交」、「存版本」、「幫我 commit」、「git commit」

---

## 工作流程

### Step 1｜收集變更資訊

執行以下指令，取得本次所有修改：

```bash
git status          # 列出所有變更檔案（modified / added / deleted）
git diff            # 讀取未 stage 檔案的實際差異
git diff --cached   # 讀取已 stage 檔案的實際差異
git log --oneline -3  # 確認最近 commit 的命名風格
```

---

### Step 2｜產生 Branch Name

格式：
```
Wade/<年份>_<本次修改總結（英文，底線分隔，首字母大寫）>
```

| 欄位 | 說明 | 範例 |
|---|---|---|
| `Wade/` | 固定前綴 | `Wade/` |
| `<年份>` | 當下西元年 | `2026` |
| `_` | 分隔符 | `_` |
| `<總結>` | 本次修改的英文概要，3–6 個單字，底線分隔 | `Add_Subscription_Management` |

完整範例：`Wade/2026_Add_Subscription_Management`

---

### Step 3｜建立分支

```bash
git checkout -b Wade/<年份>_<總結>
```

> 若目前已在正確分支上，跳過此步驟。

---

### Step 3.5｜Pre-Commit Check（必要）

**在 Stage 之前，必須先執行 `PreCommitCheck` Skill 的完整流程。**

```
→ 執行 PreCommitCheck Skill
  ├── [✅ PASS]：繼續執行 Step 4
  └── [❌ FAIL]：停止流程，回報問題，等待使用者修正後重新觸發
```

Pre-Commit Check 涵蓋：
- 安全性掃描（無 `.env`、無敏感字串）
- Python 語法檢查（`py_compile`）
- TOML 格式驗證（若 `config.toml` 有修改）
- HTML 人工確認（若 `index.html` 有修改）
- 單元測試（若 `tests/` 非空）

> 詳細規則見 `.agent/skills/PreCommitCheck/SKILL.md`

---

### Step 4｜Stage 檔案

```bash
# 明確列出每個修改檔案，禁止使用 git add -A 或 git add .
git add <檔案路徑1> <檔案路徑2> ...
```

> 不可加入 `.env`、金鑰、憑證等敏感檔案。

---

### Step 5｜產生 Commit Message

Commit 訊息結構（三段式）：

```
Wade/<年份>_<本次修改總結>

<本次修改的整體摘要：2–4 句話，說明做了什麼、為什麼做>

Files changed:
- <相對路徑/檔案名稱>：<這個檔案的修改內容，一句話>
- <相對路徑/檔案名稱>：<這個檔案的修改內容，一句話>
```

**範例：**

```
Wade/2026_Add_Subscription_Management

新增動態訂閱清單管理功能，使用者可在前端 Modal 即時新增或刪除期貨與股票
訂閱，變更即時寫入 config.toml 並透過 Redis reload 指令觸發 shioaji.py
動態重載，無需重啟服務。

Files changed:
- src/app/workers/shioaji.py：新增 reload_subscriptions() diff 訂閱邏輯與 reload cmd 處理
- src/app/main.py：新增 GET/POST/DELETE /api/subscriptions 三個 endpoint
- frontend/index.html：Sidebar 新增齒輪按鈕與訂閱管理 Modal UI
- requirements.txt：新增 tomli_w 依賴
- md/Version.md：更新版本紀錄至 v0.6.0
```

---

### Step 6｜執行 Commit

```bash
git commit -m "$(cat <<'EOF'
Wade/<年份>_<總結>

<摘要段落>

Files changed:
- <檔案>：<說明>
- <檔案>：<說明>
EOF
)"
```

---

### Step 7｜完成回報

回報以下資訊給使用者：
- Commit hash（短版 7 碼）
- Branch 名稱
- 修改檔案數量

**不執行 git push。** Push 至 origin 由使用者自行決定時機。

---

## 禁止事項

| 禁止行為 | 原因 |
|---|---|
| `git add -A` 或 `git add .` | 可能意外包含敏感檔案 |
| `git push` 任何形式 | Push 時機由使用者決定 |
| `git commit --amend` | 不修改既有 commit 歷史 |
| 加入 `Co-Authored-By` 簽名 | 本專案不使用 Claude 署名 |
| 加入 `Generated with Claude Code` 等字樣 | 同上 |

---

## 注意事項

- Branch 名稱同時作為 commit 訊息的第一行（標題）
- 年份取執行當下的西元年（`date +%Y`）
- 若同一批修改已有 branch，不重複建立
- Files changed 的路徑使用相對於 repo 根目錄的路徑
