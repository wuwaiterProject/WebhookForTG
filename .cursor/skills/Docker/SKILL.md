---
name: "docker"
description: "TXF Pipeline Docker / docker-compose 操作規範：Service 架構、常用指令、重建判斷、容器除錯 SOP。修改 docker-compose.yml、Dockerfile、或詢問容器啟動/除錯問題時觸發。"
---

# Docker — TXF Pipeline 操作規範

## Service 架構對照表

| Service | Image | 對外 Port | 職責 |
|---|---|---|---|
| `redis` | `redis:7.2-alpine` | `6379` | Celery Broker + Tick Stream 儲存 |
| `influxdb` | `influxdb:2.7.5-alpine` | `8086` | OHLC K 線長期儲存 |
| `grafana` | `grafana/grafana:10.4.2` | `3000` | K 線儀表板可視化 |
| `python-ingest` | 自建（`docker/python/Dockerfile`） | — | Shioaji 登入 / 訂閱 / Tick 寫入 Redis |
| `celery-worker` | 自建（同上） | — | 執行 OHLC 聚合任務（`agg_1m/5m/60m`） |
| `celery-beat` | 自建（同上） | — | 定時觸發 Celery 排程 |
| `flask-web` | 自建（同上） | `8080` | REST API + WebSocket 推播 |

> 所有自建 Service 共用同一個 Dockerfile（`docker/python/Dockerfile`），以 YAML anchor `x-python-base` 繼承。

---

## 常用指令

### 啟動 / 停止

```bash
# 全部服務背景啟動
docker-compose up -d

# 只啟動特定 service
docker-compose up -d flask-web redis

# 停止並移除容器（保留 volume 資料）
docker-compose down

# 停止並移除容器 + 清空所有 volume（資料歸零，慎用）
docker-compose down -v
```

### 重建映像

```bash
# 重建所有自建 Python service（修改程式碼後使用）
docker-compose up -d --build python-ingest celery-worker celery-beat flask-web

# 只重建單一 service
docker-compose up -d --build flask-web

# 強制不使用 cache 重建（依賴版本有變動時）
docker-compose build --no-cache python-ingest
docker-compose up -d python-ingest
```

### 查看 Log

```bash
# 即時追蹤 log（Ctrl+C 結束）
docker-compose logs -f python-ingest
docker-compose logs -f celery-worker
docker-compose logs -f celery-beat
docker-compose logs -f flask-web

# 查看最後 100 行
docker-compose logs --tail=100 celery-worker

# 同時查看多個 service
docker-compose logs -f celery-worker celery-beat
```

### 進入容器

```bash
# 進入 Python service shell
docker-compose exec python-ingest bash
docker-compose exec flask-web bash

# 進入 Redis CLI
docker-compose exec redis redis-cli
```

---

## 修改後是否需要 `--build`

| 修改的檔案 | 需要 `--build`？ | 原因 |
|---|---|---|
| `src/app/**/*.py` | ✅ 需要 | `COPY src/ ./src/` 進映像 |
| `frontend/index.html` | ✅ 需要 | `COPY frontend/ ./frontend/` 進映像 |
| `config.toml` | ✅ 需要 | `COPY config.toml .` 進映像 |
| `requirements.txt` | ✅ 需要 | `pip install` 在建置時執行 |
| `docker/python/Dockerfile` | ✅ 需要 | 映像定義本身變更 |
| `.env` | ❌ 不需要 | `env_file: .env` 每次啟動時掛載，重啟即生效 |
| `docker-compose.yml` | ❌ 不需要 | 重新 `up -d` 即可套用 |
| `config/grafana/**` | ❌ 不需要 | Volume mount，重啟 grafana 即生效 |
| `config/influxdb/**` | ⚠️ 僅首次 | init script 只在 volume 為空時執行一次 |

> **快速判斷原則**：只要是 `Dockerfile` 裡有 `COPY` 的路徑，修改後就需要 `--build`。

---

## 容器內除錯 SOP

### Redis 狀態確認

```bash
# 進入 Redis CLI
docker-compose exec redis redis-cli

# 常用指令
GET tick:txf:status          # 查看連線狀態
GET tick:txf:cmd             # 查看目前指令
GET tick:txf:usage_bytes     # 查看已用流量
KEYS tick:txf:*              # 列出所有 key
XLEN tick:txf:fop:MXFR1     # 查看 Stream 筆數
XREVRANGE tick:txf:fop:MXFR1 + - COUNT 3  # 查看最新 3 筆 Tick
```

### InfluxDB

- Web UI：`http://localhost:8086`（帳密來自 `.env` 的 `INFLUXDB_INIT_USERNAME / PASSWORD`）
- 確認資料寫入：Data Explorer → bucket `txf` → measurement `txf`

### Flask Web API

```bash
# 健康確認
curl http://localhost:8080/api/streams

# 查看訂閱清單
curl http://localhost:8080/api/subscriptions
```

### Grafana

- Web UI：`http://localhost:3000`（帳密來自 `.env` 的 `GRAFANA_ADMIN_USER / PASSWORD`）

---

## Service 依賴啟動順序

```
redis
  └── python-ingest   (depends_on: redis)
  └── celery-worker   (depends_on: redis)
  └── celery-beat     (depends_on: redis)
  └── flask-web       (depends_on: redis)

influxdb              (無依賴，獨立啟動)
grafana               (無依賴，獨立啟動)
```

> `influxdb` 和 `grafana` 沒有 `depends_on`，但實際資料流依賴 `influxdb` 存在。若 `influxdb` 尚未初始化完成，`celery-worker` 的寫入會失敗並輸出 error log。

---

## 禁止事項

| 禁止行為 | 原因 |
|---|---|
| `docker-compose down -v` 用於 production | 會清除所有歷史 K 線與 Redis 資料 |
| 修改程式碼後不 `--build` 直接 `up -d` | 容器仍執行舊映像，修改不生效 |
| 直接修改容器內的檔案 | 容器重建後會消失，修改必須在 host 端進行 |
