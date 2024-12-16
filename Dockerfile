# Stage 1: Base Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage 2: Build & Restore
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln . 
COPY WebAPI/WebAPI.csproj WebAPI/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Application.Tests/Application.Tests.csproj Application.Tests/
RUN dotnet restore

# Copy all sources after restore for full build
COPY . .
WORKDIR /src/WebAPI
RUN dotnet test
RUN dotnet publish -c Release -o /app/publish --no-restore

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]