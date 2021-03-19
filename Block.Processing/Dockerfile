FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["Block.Processing", "Block.Processing"]

WORKDIR /src/Block.Processing

RUN dotnet restore -s https://api.nuget.org/v3/index.json -s http://dockerdev.simcord.com:8081/repository/nuget-releases2/
RUN dotnet build -nowarn:CS0436 --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Block.Processing.dll"]
