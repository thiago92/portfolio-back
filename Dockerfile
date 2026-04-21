# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Portfolio.Domain/Portfolio.Domain.csproj Portfolio.Domain/
COPY Portfolio.Application/Portfolio.Application.csproj Portfolio.Application/
COPY Portfolio.Infrastructure/Portfolio.Infrastructure.csproj Portfolio.Infrastructure/
COPY PortfolioApi/PortfolioApi.csproj PortfolioApi/
COPY Portfolio.Tests/Portfolio.Tests.csproj Portfolio.Tests/

RUN dotnet restore PortfolioApi/PortfolioApi.csproj

COPY . .
RUN dotnet publish PortfolioApi/PortfolioApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "PortfolioApi.dll"]
