# Use .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first for layer caching
COPY ["ScanAndKnow2/ScanAndKnow2.csproj", "ScanAndKnow2/"]
COPY ["ScanToKnowBusiness/ScanToKnowBusiness.csproj", "ScanToKnowBusiness/"]
COPY ["ScanToKnowDataAccess/ScanToKnowDataAccess.csproj", "ScanToKnowDataAccess/"]

# Restore packages
RUN dotnet restore "ScanAndKnow2/ScanAndKnow2.csproj"

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish "ScanAndKnow2/ScanAndKnow2.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ScanAndKnow2.dll"]
