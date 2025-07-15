# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# ✅ Install required SkiaSharp native dependencies
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libjpeg-dev \
    libpng-dev \
    && rm -rf /var/lib/apt/lists/*
    
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Copy solution and csproj files
COPY SaanjhiCreation.sln ./

# Copy ProductService and Shared project files
COPY Services/ProductService/ProductService.API/*.csproj ./Services/ProductService/ProductService.API/
COPY Services/ProductService/ProductService.Application/*.csproj ./Services/ProductService/ProductService.Application/
COPY Services/ProductService/ProductService.Domain/*.csproj ./Services/ProductService/ProductService.Domain/
COPY Services/ProductService/ProductService.Infrastructure/*.csproj ./Services/ProductService/ProductService.Infrastructure/
COPY Services/CategoryService/CategoryService.Contracts/*.csproj ./Services/CategoryService/CategoryService.Contracts/
COPY Shared/Shared.*/*.csproj ./Shared/

# Restore dependencies
RUN dotnet restore ./Services/ProductService/ProductService.API/ProductService.API.csproj

# Copy the full source
COPY . .

# Build and publish
WORKDIR /app/Services/ProductService/ProductService.API
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "ProductService.API.dll"]
