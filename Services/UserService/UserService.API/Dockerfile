# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Copy solution and csproj files
COPY SaanjhiCreation.sln ./

# Copy UserService and Shared project files
COPY Services/UserService/UserService.API/*.csproj ./Services/UserService/UserService.API/
COPY Services/UserService/UserService.Application/*.csproj ./Services/UserService/UserService.Application/
COPY Services/UserService/UserService.Domain/*.csproj ./Services/UserService/UserService.Domain/
COPY Services/UserService/UserService.Infrastructure/*.csproj ./Services/UserService/UserService.Infrastructure/
COPY Shared/Shared.*/*.csproj ./Shared/

# Restore dependencies
RUN dotnet restore ./Services/UserService/UserService.API/UserService.API.csproj

# Copy the full source
COPY . .

# Build and publish
WORKDIR /app/Services/UserService/UserService.API
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.API.dll"]
