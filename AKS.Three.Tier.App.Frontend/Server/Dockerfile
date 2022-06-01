#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AKS.Three.Tier.App.Frontend/Server/AKS.Three.Tier.App.Frontend.Server.csproj", "AKS.Three.Tier.App.Frontend/Server/"]
COPY ["AKS.Three.Tier.App.Frontend/Client/AKS.Three.Tier.App.Frontend.Client.csproj", "AKS.Three.Tier.App.Frontend/Client/"]
COPY ["AKS.Three.Tier.App.Frontend/Shared/AKS.Three.Tier.App.Frontend.Shared.csproj", "AKS.Three.Tier.App.Frontend/Shared/"]
RUN dotnet restore "AKS.Three.Tier.App.Frontend/Server/AKS.Three.Tier.App.Frontend.Server.csproj"
COPY . .
WORKDIR "/src/AKS.Three.Tier.App.Frontend/Server"
RUN dotnet build "AKS.Three.Tier.App.Frontend.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AKS.Three.Tier.App.Frontend.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AKS.Three.Tier.App.Frontend.Server.dll"]