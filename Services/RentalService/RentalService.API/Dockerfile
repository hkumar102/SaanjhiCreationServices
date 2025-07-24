# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Copy solution and csproj files
COPY SaanjhiCreation.sln ./


# Copy RentalService, Shared, and NotificationService project files
COPY Services/RentalService/RentalService.API/*.csproj ./Services/RentalService/RentalService.API/
COPY Services/RentalService/RentalService.Application/*.csproj ./Services/RentalService/RentalService.Application/
COPY Services/RentalService/RentalService.Domain/*.csproj ./Services/RentalService/RentalService.Domain/
COPY Services/RentalService/RentalService.Infrastructure/*.csproj ./Services/RentalService/RentalService.Infrastructure/
COPY Shared/Shared.*/*.csproj ./Shared/
# Copy NotificationService project files
COPY Services/NotificationService/NotificationService.Application/*.csproj ./Services/NotificationService/NotificationService.Application/
COPY Services/NotificationService/NotificationService.Domain/*.csproj ./Services/NotificationService/NotificationService.Domain/
COPY Services/NotificationService/NotificationService.Infrastructure/*.csproj ./Services/NotificationService/NotificationService.Infrastructure/
COPY Services/NotificationService/NotificationService.Contracts/*.csproj ./Services/NotificationService/NotificationService.Contracts/

# Restore dependencies
RUN dotnet restore ./Services/RentalService/RentalService.API/RentalService.API.csproj

# Copy the full source
COPY . .

# Build and publish
WORKDIR /app/Services/RentalService/RentalService.API
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "RentalService.API.dll"]
