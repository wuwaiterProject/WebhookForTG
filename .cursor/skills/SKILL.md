# TXF Pipeline — Skills 索引

本目錄定義 AI Agent 處理此專案時的行為規範與參考指南。

> **自動套用規則（無需觸發）：** 專案根目錄的 `AGENTS.md` 定義所有跨平台共用規則
> （Python 命名/型別/錯誤處理、前端色彩/字型、架構禁止事項、版本管理）。
> Claude Code 與 Antigravity 啟動時自動讀取，每次互動均生效。

各 Skill 獨立成資料夾，觸發條件與用途如下：

| Skill | 觸發時機 |
|---|---|
| `txf-pipeline` | 修改目錄結構、新增服務、調整設定管理、UI 元件對照 |
| `CodingStyle` | 新增或修改任何 `src/` Python 檔案、Code Review、重構 |
| `Shioaji` | 修改永豐金 API 串接、訂閱邏輯、流量處理 |
| `Redis` | 新增 Redis 操作、擴充 Stream Key、調整指令通道 |
| `InfluxDB-Schema` | 修改 InfluxDB 查詢、新增 Measurement 或 Field |
| `UIStyleDefine` | 新增或修改前端元件、CSS 樣式、動畫 |
| `CICD_Define` | 建立或修改 GitHub Actions、部署流程 |
| `Version` | 每次功能變更後更新版本紀錄 |
| `Docker` | 操作 docker-compose（啟動/重建/log/除錯）、修改 Dockerfile 或 docker-compose.yml |
| `PreCommitCheck` | Commit 前自動驗證（語法、安全性、測試）；由 GitCommit Skill 自動呼叫 |
| `GitCommit` | 執行 git commit（Pre-Commit Check → 建立本地分支 → 結構化訊息，不 push origin） |
| `example-skill` | 建立新 Skill 時的格式範本 |
