# ==============================================================================
# Stage 1: Restore dependencies
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

# Copy all csproj files first for layer caching
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["Tests/Tests.csproj", "Tests/"]
COPY ["TourManagement.slnx", "./"]

RUN dotnet restore "TourManagement.slnx"

# ==============================================================================
# Stage 2: Build & Test
# ==============================================================================
FROM restore AS build
WORKDIR /src
COPY . .

# Build the solution
RUN dotnet build "TourManagement.slnx" -c Release --no-restore

# Run unit tests — fail the build if any test fails
RUN dotnet test "Tests/Tests.csproj" -c Release --no-build --verbosity normal

# ==============================================================================
# Stage 3: Publish
# ==============================================================================
FROM build AS publish
WORKDIR /src/WebAPI
RUN dotnet publish "WebAPI.csproj" -c Release --no-build -o /app/publish

# ==============================================================================
# Stage 4: Runtime
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebAPI.dll"]