# AKS-3-Tier-App

A sample three-tier application built with .NET 8 and designed for deployment on [Azure Kubernetes Service (AKS)](https://learn.microsoft.com/en-us/azure/aks/). The app demonstrates how to run a Blazor WebAssembly frontend, an ASP.NET Core API, and a Redis data store together in Kubernetes using Helm.

## Architecture

```
┌────────────────────────────────────────────┐
│                    AKS                      │
│                                            │
│  ┌──────────┐    ┌──────────┐    ┌───────┐ │
│  │ Frontend │───▶│   API    │───▶│ Redis │ │
│  │ (Blazor) │    │(ASP.NET) │    │  (DB) │ │
│  └──────────┘    └──────────┘    └───────┘ │
│       ▲                                    │
│  ┌────┴──────────────┐                     │
│  │  NGINX Ingress    │                     │
│  └───────────────────┘                     │
└────────────────────────────────────────────┘
```

| Tier     | Technology                          | Description                                              |
|----------|-------------------------------------|----------------------------------------------------------|
| Frontend | Blazor WebAssembly + ASP.NET Server | Displays environment info from both frontend and backend |
| API      | ASP.NET Core 8 + Swagger            | Returns host/IP/memory info and persists entries to Redis |
| Database | Redis                               | Stores the last 10 API-caller host names as a list       |

The home page shows live environment details (hostname, IP addresses, OS, .NET version, memory) from both the frontend server and the backend API, along with the most recent hosts that connected to the database.

## Tech Stack

- **.NET 8** – Blazor WebAssembly (client), ASP.NET Core (server & API)
- **Redis** – in-memory data store for the data tier
- **Docker / Docker Compose** – local development environment
- **Kubernetes (AKS)** – production deployment target
- **Helm** – Kubernetes package management
- **NGINX Ingress Controller** – routes external traffic to the frontend
- **GitHub Actions** – CI/CD pipeline that builds and publishes container images to [GitHub Container Registry](https://ghcr.io)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes Docker Compose)
- [kubectl](https://kubernetes.io/docs/tasks/tools/) (for Kubernetes deployments)
- [Helm 3](https://helm.sh/docs/intro/install/) (for Helm-based deployments)
- An AKS cluster (for cloud deployments) – see [Quickstart: Deploy an AKS cluster](https://learn.microsoft.com/en-us/azure/aks/learn/quick-kubernetes-deploy-portal)

## Local Development

Use Docker Compose to run all three services locally:

```bash
docker compose up --build
```

| Service  | Local URL               |
|----------|-------------------------|
| Frontend | http://localhost:5100   |
| API      | http://localhost:5101   |
| Redis    | localhost:6379          |

The API exposes a Swagger UI at `http://localhost:5101/swagger`.

## Container Images

Container images are published to the GitHub Container Registry on every push to `master`:

| Image | Pull command |
|-------|-------------|
| API | `docker pull ghcr.io/macel94/aks-three-tier-app/api:master` |
| Frontend | `docker pull ghcr.io/macel94/aks-three-tier-app/frontend.server:master` |

## Kubernetes Deployment

### Option 1 – Helm (recommended)

1. Add the NGINX Ingress Controller repository and install it:

   ```bash
   helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
   helm repo update
   ```

2. Run the deploy script from `deploy/k8s/scripts/`:

   ```powershell
   cd deploy/k8s/scripts
   ./deploy.ps1
   ```

   The script runs:

   ```bash
   helm upgrade nginx ingress-nginx/ingress-nginx --install
   helm upgrade three-tier-app ./../helm --install
   ```

3. To uninstall:

   ```powershell
   ./uninstall.ps1
   ```

### Option 2 – Raw kubectl manifests

Apply the individual manifests from `deploy/k8s/helm/templates/`:

```bash
kubectl apply -f deploy/k8s/helm/templates/db-redis.yaml
kubectl apply -f deploy/k8s/helm/templates/api.yaml
kubectl apply -f deploy/k8s/helm/templates/frontend.yaml
kubectl apply -f deploy/k8s/helm/templates/ingress-nginx.yaml
```

### Helm values

Key values that can be overridden in `deploy/k8s/helm/values.yaml`:

| Key | Default | Description |
|-----|---------|-------------|
| `Env` | `Development` | Application environment name |

## CI/CD

GitHub Actions workflows build Docker images on every pull request and push to `master`:

| Workflow | Trigger | Action |
|----------|---------|--------|
| `api` | Push/PR to `master` affecting `AKS.Three.Tier.App.API/**` | Build (PR) or Build & Push (merge) |
| `frontend-server` | Push/PR to `master` affecting `AKS.Three.Tier.App.Frontend/**` | Build (PR) or Build & Push (merge) |

Images are tagged with the branch name (e.g., `master`) and pushed to `ghcr.io`.

## Project Structure

```
AKS-3-Tier-App/
├── AKS.Three.Tier.App.API/        # ASP.NET Core 8 REST API
│   ├── Controllers/               # API controllers (environment info + Redis)
│   ├── Dockerfile
│   └── Program.cs
├── AKS.Three.Tier.App.Frontend/   # Blazor WebAssembly + Server
│   ├── Client/                    # Blazor WASM client
│   ├── Server/                    # ASP.NET Core host & BFF
│   └── Shared/                    # Shared DTOs
├── deploy/
│   └── k8s/
│       ├── helm/                  # Helm chart (Chart.yaml, values.yaml, templates/)
│       └── scripts/               # deploy.ps1 / uninstall.ps1
├── docker-compose.yml             # Local multi-service setup
└── .github/workflows/             # CI/CD pipelines
```

## License

[MIT](LICENSE.txt)

