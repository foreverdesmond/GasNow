<div align="center">

# ⛽ GasNow

**Real-time Ethereum & L2 Gas Price API · ETH 与 L2 实时 Gas 价格 API**

</div>

<!-- 语言切换按钮 -->
<div align="center" style="margin: 16px 0">
  <a href="#zh-CN"><b>🇨🇳 中文</b></a> ·
  <a href="#en"><b>🇬🇧 English</b></a>
</div>

---

<!-- ==================== 中文版 ==================== -->
<div id="zh-CN">

# 项目简介

GasNow 是一个基于 **.NET 8** 的后端服务,聚合 **以太坊主网及多条 Layer 2 网络** 的实时 Gas 费用数据,为 [gasnow.link](https://gasnow.link/) 提供数据支撑(前端仓库见 [`gas-now-web-app`](https://github.com/foreverdesmond/gas-now-web-app))。

## ✨ 功能特性

- **实时 Gas 追踪**:支持 ETH、Arbitrum、Optimism、Base、Linea、zkSync 六条网络
- **多源聚合**:Etherscan、Lineascan、Alchemy、Infura、Blocknative 多数据源互为备份
- **多链支持**:基于 Blocknative chainid 查询,天然支持多链
- **接口文档**:内置 Swagger(Swashbuckle)
- **后台任务**:`GasNowConsole` 定时抓取数据
- **可选持久化**:EF Core(SQL)+ Redis 缓存(配置开关)
- **Docker 支持**:API 与后台 worker 均有 Dockerfile
- **结构化日志**:NLog

## 🏗️ 架构

```
GasNow.sln
├── GasNow.API        # Web API(Blocknative / GasFee / Price 控制器)
├── GasNow            # 核心库(Business / DTOs / ExternalApis / Factory / Helper / Mapping / Module / Service)
├── GasNowConsole     # 后台数据抓取 worker
└── GasNowTest        # 单元测试(控制器、外部 API、服务)
```

## 📋 环境要求

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- 数据源 API Key(见下表)

## ⚙️ 配置

复制示例环境文件并填入你的 API Key:

```bash
cp GasNow.API/.env.example GasNow.API/.env
```

| 变量 | 必填 | 来源 |
|---|---|---|
| `ETHERSCAN_API_KEY` | 是 | https://etherscan.io |
| `ALCHEMY_API_KEY` | 否 | https://www.alchemy.com |
| `INFURA_API_KEY` | 否 | https://www.infura.io |
| `LINEASCAN_API_KEY` | 否 | https://lineascan.build |
| `BLOCKNAVIE_API_KEY` | 否 | https://www.blocknative.com |

应用配置(连接串、API 地址、数据库开关)在 `GasNow/appsetting.example.json` —— 复制为 `appsetting.json` 后按需修改。

## 🚀 运行

```bash
# API 服务
cd GasNow.API && dotnet run

# 后台抓取 worker
cd GasNowConsole && dotnet run
```

或使用 Docker(见 `GasNow.API/docker/` 与 `GasNowConsole/docker/`)。

## 🔌 API 端点

- `GET /api/gasfee` — 当前 Gas 价格
- `GET /api/price` — ETH 价格
- `GET /api/blocknavie` — Blocknative 按链 Gas 价格

## 🧪 测试

```bash
dotnet test GasNowTest
```

## 📄 许可证

[MIT](LICENSE)

## 🔗 相关

- 前端仓库:[gas-now-web-app](https://github.com/foreverdesmond/gas-now-web-app)
- 线上站点:[gasnow.link](https://gasnow.link/)

</div>

<!-- ==================== English ==================== -->
<div id="en">

# Overview

GasNow is a **.NET 8** backend service that aggregates real-time gas fee data
for **Ethereum and multiple Layer 2 networks**. It powers
[gasnow.link](https://gasnow.link/), built with the companion frontend
[`gas-now-web-app`](https://github.com/foreverdesmond/gas-now-web-app).

## ✨ Features

- **Real-time gas tracking** for ETH, Arbitrum, Optimism, Base, Linea, zkSync
- **Multi-source aggregation**: Etherscan, Lineascan, Alchemy, Infura, Blocknative
- **Multi-chain support** via Blocknative chainid-based queries
- **Swagger API docs** (Swashbuckle)
- **Background worker** (`GasNowConsole`) for scheduled collection
- **Optional persistence**: EF Core (SQL) + Redis caching (toggle in config)
- **Docker support** for API and worker
- **Structured logging** with NLog

## 🏗️ Architecture

```
GasNow.sln
├── GasNow.API        # Web API (Blocknative / GasFee / Price controllers)
├── GasNow            # Core library (Business / DTOs / ExternalApis / Factory / Helper / Mapping / Module / Service)
├── GasNowConsole     # Background data-collection worker
└── GasNowTest        # Unit tests (controllers, external APIs, services)
```

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- API keys for the data sources you use (see below)

## ⚙️ Configuration

Copy the example env file and fill in your API keys:

```bash
cp GasNow.API/.env.example GasNow.API/.env
```

| Variable | Required | Source |
|---|---|---|
| `ETHERSCAN_API_KEY` | Yes | https://etherscan.io |
| `ALCHEMY_API_KEY` | No | https://www.alchemy.com |
| `INFURA_API_KEY` | No | https://www.infura.io |
| `LINEASCAN_API_KEY` | No | https://lineascan.build |
| `BLOCKNAVIE_API_KEY` | No | https://www.blocknative.com |

Application settings live in `GasNow/appsetting.example.json` — copy it to
`appsetting.json` and adjust.

## 🚀 Running

```bash
# API server
cd GasNow.API && dotnet run

# Background worker
cd GasNowConsole && dotnet run
```

Or with Docker (see `GasNow.API/docker/` and `GasNowConsole/docker/`).

## 🔌 API Endpoints

- `GET /api/gasfee` — current gas prices
- `GET /api/price` — ETH price
- `GET /api/blocknavie` — Blocknative gas prices by chain

## 🧪 Tests

```bash
dotnet test GasNowTest
```

## 📄 License

[MIT](LICENSE)

## 🔗 Related

- Frontend: [gas-now-web-app](https://github.com/foreverdesmond/gas-now-web-app)
- Live site: [gasnow.link](https://gasnow.link/)

</div>
