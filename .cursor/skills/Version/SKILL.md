---
name: "Project/Skill version history definition"
description: "每次功能變更後更新版本紀錄。版本管理規則已移至 AGENTS.md 自動套用；本 Skill 提供版本號格式範例與觸發提醒。"
---

# Version Management — 觸發索引

> **版本管理規則已移至 `AGENTS.md`（project root），由 Claude Code 與 Antigravity 自動載入。**
> 本 Skill 僅作為觸發提醒與格式範例。

詳細規則見：`AGENTS.md` → **版本管理規則** 章節

---

## 使用時機

每次完成功能變更、Skill 新增、架構調整後，在 commit 前更新 `md/Version.md`。

---

## 版本號格式範例

```markdown
## v1.2.0 — 2026-05-19

### Changes
- 新增 AGENTS.md，整合跨平台共用規則（CodingStyle、UIStyleDefine、Version）
- 降格 CodingStyle / UIStyleDefine / Version Skill 為觸發索引
- 更新 .agent/skills/SKILL.md 索引加入 AGENTS.md 說明
```

---

## 遞增規則速查

| 版號 | 情境 |
|---|---|
| Patch (Z) | 文件更新、小修正、Skill 補充說明 |
| Minor (Y) | 新功能、新 Skill、結構重組 |
| Major (X) | 重大架構變更、破壞性變更 |

## 文件位置

- 版本歷史：`md/Version.md`
- 最新版本插在 `# Version History` 標題之後（最新在最上方）
