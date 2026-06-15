---
name: frontend-ui-style
description: 定義 TXF Pipeline 前端 UI 風格規範，包含深色系 Design Token、字型、元件樣式與版面慣例。於新增或修改 frontend/ 頁面、元件、CSS 時套用，確保與現有風格一致。
---

# 前端 UI 風格規範 — 元件參考

> **禁止事項與命名規則已移至 `AGENTS.md`（project root），由 Claude Code 與 Antigravity 自動載入。**
> 本 Skill 保留 CSS 元件範例供操作參考。

詳細規則見：`AGENTS.md` → **前端規則** 章節

---

## 使用時機

- 新增或修改 `frontend/` 下的 HTML、CSS 時
- 使用者提到「照現有風格」「跟介面一致」「UI 風格」時
- 設計按鈕、卡片、標籤、遮罩、動畫等介面元件時

---

## Design Token（本專案實際色票）

```css
:root {
  --bg-0: #0b0e17;
  --bg-1: #111520;
  --bg-2: #181d2e;
  --bg-3: #1f2640;
  --accent: #3b82f6;
  --accent-glow: rgba(59, 130, 246, .25);
  --green: #22c55e;
  --red: #ef4444;
  --text-1: #f0f4ff;
  --text-2: #94a3b8;
  --text-3: #64748b;
  --border: rgba(255, 255, 255, .07);
  --radius: 12px;
}
```

**漲跌色慣例（台灣市場：漲紅跌綠）**

| Class | 顏色 | 語意 |
|---|---|---|
| `.up` | `#ef4444`（`--red`） | 價格上漲 |
| `.dn` | `#22c55e`（`--green`） | 價格下跌 |

---

## 元件 CSS 範例

### 按鈕

```css
padding: 5px 12px;
border-radius: 8px;
border: 1px solid var(--border);
background: transparent;
color: var(--text-2);
font-size: .8rem;
font-family: 'Inter', sans-serif;
cursor: pointer;
transition: all .15s;

/* Hover */
background: var(--bg-3); color: var(--text-1);
/* Active */
background: var(--accent); border-color: var(--accent); color: #fff;
```

### 狀態 Badge

```css
display: flex; align-items: center; gap: 6px;
font-size: .8rem; padding: 5px 12px;
background: var(--bg-2); border-radius: 20px;
border: 1px solid var(--border); color: var(--text-2);

/* 狀態點 */
.status-dot { width: 7px; height: 7px; border-radius: 50%; }
.status-dot.connected { background: var(--green); box-shadow: 0 0 8px var(--green); }
.status-dot.error     { background: var(--red);   box-shadow: 0 0 8px var(--red); }
```

### 卡片 / 列表項目

```css
padding: 12px 14px;
border-radius: var(--radius);
border: 1px solid transparent;
cursor: pointer;
transition: background .15s, border-color .15s;
position: relative; overflow: hidden;

/* Active */
background: var(--bg-3); border-color: var(--accent);

/* 光暈（::before） */
.card::before {
  content: ''; position: absolute; inset: 0;
  background: linear-gradient(135deg, var(--accent-glow), transparent);
  opacity: 0; transition: opacity .2s;
}
.card:hover::before  { opacity: .5; }
.card.active::before { opacity: 1; }
```

### 遮罩 / Overlay

```css
.overlay {
  position: absolute; inset: 0;
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  gap: 12px; color: var(--text-3);
  pointer-events: none;   /* 必填 */
}
.overlay.blur {
  background: rgba(11, 14, 23, .92);
  backdrop-filter: blur(6px);
}
```

### Stats Bar

```css
.stats-bar { display: flex; border-top: 1px solid var(--border); background: var(--bg-1); }
.stat-item { flex: 1; padding: 10px 18px; border-right: 1px solid var(--border); }
.stat-item:last-child { border-right: none; }
.stat-label { font-size: .68rem; color: var(--text-3); text-transform: uppercase; letter-spacing: .1em; }
.stat-value { font-family: 'JetBrains Mono', monospace; font-size: .95rem; font-weight: 600; }
```

---

## 動畫模式

```css
/* Pulse（心跳）— 連線指示燈 */
@keyframes pulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50%       { opacity: .6; transform: scale(.85); }
}

/* Flash（價格跳動）*/
@keyframes flash-up { 0% { background: rgba(239, 68, 68, .3); } 100% { background: transparent; } }
@keyframes flash-dn { 0% { background: rgba(34, 197, 94, .3); } 100% { background: transparent; } }
```

---

## 版面與間距

- **Header**：`padding: 14px 28px`、`position: sticky; top: 0; z-index: 100`、`backdrop-filter: blur(12px)`
- **主佈局**：`grid-template-columns: 260px 1fr`
- **Sidebar**：`260px`（固定）

---

## 快速檢查清單

- [ ] 所有顏色使用 CSS 變數，無硬寫色碼
- [ ] 數字欄位使用 JetBrains Mono，UI 文字使用 Inter
- [ ] 按鈕、Badge、卡片圓角與 transition 與現有一致
- [ ] Active / Hover 狀態使用 `--bg-3` 與 `--accent`
- [ ] 新增動畫時長 ≤ 0.5s（pulse 除外）
- [ ] Overlay 加上 `pointer-events: none`
