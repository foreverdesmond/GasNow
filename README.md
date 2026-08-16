# GasNow

Real-time Ethereum and Layer 2 gas price monitoring API.

GasNow is a .NET 8 backend service that aggregates real-time gas fee data for
Ethereum and multiple Layer 2 networks. It powers [gasnow.link](https://gasnow.link/),
which is built with the companion frontend repo
[`gas-now-web-app`](https://github.com/foreverdesmond/gas-now-web-app).

## Features

- **Real-time gas tracking** for Ethereum and L2 networks (Arbitrum, Optimism, Base, Linea, zkSync)
- **Multi-source aggregation**: Etherscan, Lineascan, Alchemy, Infura, Blocknative
- **Multi-chain support** via Blocknative chainid-based queries
- **Swagger API docs** (Swashbuckle)
- **Background worker** (`GasNowConsole`) for scheduled data collection
- **Optional persistence**: EF Core (SQL) + Redis caching (toggle via config)
- **Docker support** for API and console worker
- **Structured logging** with NLog

## Architecture

```
GasNow.sln
├── GasNow.API        # Web API (Blocknative / GasFee / Price controllers)
├── GasNow            # Core library (Business / DTOs / ExternalApis / Factory / Helper / Mapping / Module / Service)
├── GasNowConsole     # Background data-collection worker
└── GasNowTest        # Unit tests (controllers, external APIs, services)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- API keys for the data sources you want to use (see below)

## Configuration

Copy the example environment file and fill in your API keys:

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

Application settings (connection strings, API URLs, database toggle) live in
`GasNow/appsetting.example.json` — copy it to `appsetting.json` and adjust.

## Running

```bash
# API server
cd GasNow.API && dotnet run

# Background worker
cd GasNowConsole && dotnet run
```

Or with Docker (see `GasNow.API/docker/` and `GasNowConsole/docker/` for
Dockerfiles).

## API Endpoints

- `GET /api/gasfee` — current gas prices
- `GET /api/price` — ETH price
- `GET /api/blocknavie` — Blocknative gas prices by chain

## Tests

```bash
dotnet test GasNowTest
```

## License

[MIT](LICENSE)

## Related

- Frontend: [gas-now-web-app](https://github.com/foreverdesmond/gas-now-web-app)
- Live site: [gasnow.link](https://gasnow.link/)
