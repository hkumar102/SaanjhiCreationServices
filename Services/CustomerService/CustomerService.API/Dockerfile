# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Copy solution and csproj files
COPY SaanjhiCreation.sln ./

# Copy CustomerService and Shared project files
COPY Services/CustomerService/CustomerService.API/*.csproj ./Services/CustomerService/CustomerService.API/
COPY Services/CustomerService/CustomerService.Application/*.csproj ./Services/CustomerService/CustomerService.Application/
COPY Services/CustomerService/CustomerService.Domain/*.csproj ./Services/CustomerService/CustomerService.Domain/
COPY Services/CustomerService/CustomerService.Infrastructure/*.csproj ./Services/CustomerService/CustomerService.Infrastructure/
COPY Shared/Shared.*/*.csproj ./Shared/

# Restore dependencies
RUN dotnet restore ./Services/CustomerService/CustomerService.API/CustomerService.API.csproj

# Copy the full source
COPY . .

# Build and publish
WORKDIR /app/Services/CustomerService/CustomerService.API
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "CustomerService.API.dll"]
