FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/GerenciadorProcessos.Api/GerenciadorProcessos.Api.csproj", "src/GerenciadorProcessos.Api/"]
COPY ["src/GerenciadorProcessos.Application/GerenciadorProcessos.Application.csproj", "src/GerenciadorProcessos.Application/"]
COPY ["src/GerenciadorProcessos.Domain/GerenciadorProcessos.Domain.csproj", "src/GerenciadorProcessos.Domain/"]
COPY ["src/GerenciadorProcessos.Infrastructure/GerenciadorProcessos.Infrastructure.csproj", "src/GerenciadorProcessos.Infrastructure/"]
COPY ["src/GerenciadorProcessos.Shared/GerenciadorProcessos.Shared.csproj", "src/GerenciadorProcessos.Shared/"]
RUN dotnet restore "./src/GerenciadorProcessos.Api/GerenciadorProcessos.Api.csproj"
COPY . .
WORKDIR "/src/src/GerenciadorProcessos.Api"
RUN dotnet build "./GerenciadorProcessos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GerenciadorProcessos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GerenciadorProcessos.Api.dll"]
