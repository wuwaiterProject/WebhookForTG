---
name: "CodingStyle"
description: "提醒套用 Python 程式碼風格規範。新增或修改任何 src/ 下的 Python 檔案、Code Review、重構時觸發。"
---

# Coding Style — 觸發索引

> **規則已移至 `AGENTS.md`（project root），由 Claude Code 與 Antigravity 自動載入。**
> 本 Skill 僅作為觸發提醒與快速檢查清單。

詳細規則見：`AGENTS.md` → **Python 規則** 章節

---

## 使用時機

- 新增或修改 `src/` 下任何 Python 檔案時
- Code Review 或重構既有程式碼時
- 使用者提到「風格」「命名」「格式」「排版」時

---

## 快速檢查清單

- [ ] 函式、變數、常數命名符合對應風格（`snake_case` / `UPPER_SNAKE_CASE` / `PascalCase`）
- [ ] Import 分三區塊並依字母排序
- [ ] 所有公開函式有型別標注與 docstring
- [ ] `except` 明確指定例外類型並輸出錯誤訊息
- [ ] Print 輸出含 `>>>` 前綴與適當 `[標籤]`
- [ ] 無硬寫敏感資料，敏感值透過 `app.config` 引入
