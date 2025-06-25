# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Copy solution and csproj files
COPY SaanjhiCreation.sln ./

# Copy MediaService and Shared project files
COPY Services/MediaService/MediaService.API/*.csproj ./Services/MediaService/MediaService.API/
COPY Services/MediaService/MediaService.Application/*.csproj ./Services/MediaService/MediaService.Application/
COPY Services/MediaService/MediaService.Domain/*.csproj ./Services/MediaService/MediaService.Domain/
COPY Services/MediaService/MediaService.Infrastructure/*.csproj ./Services/MediaService/MediaService.Infrastructure/
COPY Shared/Shared.*/*.csproj ./Shared/

# Restore dependencies
RUN dotnet restore ./Services/MediaService/MediaService.API/MediaService.API.csproj

# Copy the full source
COPY . .

# Build and publish
WORKDIR /app/Services/MediaService/MediaService.API
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "MediaService.API.dll"]
