# Stage 1: Base Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage 2: Build & Restore
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln . 
COPY WebAPI/WebAPI.csproj WebAPI/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Application.Tests/Application.Tests.csproj Application.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all sources for full build
COPY . .

# Run NUnit 3 tests in Application.Tests
WORKDIR /src/Application.Tests
RUN dotnet test --framework net8.0 --no-restore --logger:trx

# Publish the WebAPI project
WORKDIR /src/WebAPI
RUN dotnet publish -c Release -o /app/publish --no-restore

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
