FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY RohitPortfolio.Shared/RohitPortfolio.Shared.csproj RohitPortfolio.Shared/
COPY RohitPortfolio.Api/RohitPortfolio.Api.csproj RohitPortfolio.Api/
RUN dotnet restore RohitPortfolio.Api/RohitPortfolio.Api.csproj
COPY RohitPortfolio.Shared/ RohitPortfolio.Shared/
COPY RohitPortfolio.Api/ RohitPortfolio.Api/
RUN dotnet publish RohitPortfolio.Api/RohitPortfolio.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RohitPortfolio.Api.dll"]
