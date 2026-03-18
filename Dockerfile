# Use .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files with folder structure
COPY ["AcadLinkEduBackEnd.API/AcadLinkEduBackEnd.API.csproj", "AcadLinkEduBackEnd.API/"]
COPY ["AcadLinkEduBackEnd.Application/AcadLinkEduBackEnd.Application.csproj", "AcadLinkEduBackEnd.Application/"]
COPY ["AcadLinkEduBackEnd.Domain/AcadLinkEduBackEnd.Domain.csproj", "AcadLinkEduBackEnd.Domain/"]
COPY ["AcadLinkEduBackEnd.Infrastructure/AcadLinkEduBackEnd.Infrastructure.csproj", "AcadLinkEduBackEnd.Infrastructure/"]

# Restore packages
RUN dotnet restore "AcadLinkEduBackEnd.API/AcadLinkEduBackEnd.API.csproj"

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish "AcadLinkEduBackEnd.API/AcadLinkEduBackEnd.API.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "AcadLinkEduBackEnd.API.dll"]
