# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Copy solution and csproj files
COPY SaanjhiCreation.sln ./

# Copy CategoryService and Shared project files
COPY Services/CategoryService/CategoryService.API/*.csproj ./Services/CategoryService/CategoryService.API/
COPY Services/CategoryService/CategoryService.Application/*.csproj ./Services/CategoryService/CategoryService.Application/
COPY Services/CategoryService/CategoryService.Domain/*.csproj ./Services/CategoryService/CategoryService.Domain/
COPY Services/CategoryService/CategoryService.Infrastructure/*.csproj ./Services/CategoryService/CategoryService.Infrastructure/
COPY Shared/Shared.*/*.csproj ./Shared/

# Restore dependencies
RUN dotnet restore ./Services/CategoryService/CategoryService.API/CategoryService.API.csproj

# Copy the full source
COPY . .

# Build and publish
WORKDIR /app/Services/CategoryService/CategoryService.API
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "CategoryService.API.dll"]
