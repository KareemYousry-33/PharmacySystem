# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /src

# Copy solution and project files
COPY PharmacySystem.slnx .
COPY PharmacySystem.API/ ./PharmacySystem.API/
COPY PharmacySystem.Application/ ./PharmacySystem.Application/
COPY PharmacySystem.Domain/ ./PharmacySystem.Domain/
COPY PharmacySystem.Infrastructure/ ./PharmacySystem.Infrastructure/

# Restore and build
RUN dotnet restore PharmacySystem.slnx
RUN dotnet build PharmacySystem.slnx -c Release --no-restore

# Publish
RUN dotnet publish PharmacySystem.API/PharmacySystem.API.csproj -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "PharmacySystem.API.dll"]
