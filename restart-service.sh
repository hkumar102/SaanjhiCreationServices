#!/bin/bash

SERVICE_NAME=$1

if [ -z "$SERVICE_NAME" ]; then
  echo "❌ Please provide a service name."
  echo "Usage: ./restart-service.sh <service-name>"
  exit 1
fi

echo "🔄 Restarting service: $SERVICE_NAME"

# Stop the service
docker compose stop "$SERVICE_NAME"

# Remove container (and volume if you want a full reset)
docker compose rm -f "$SERVICE_NAME"

# Rebuild and start the service
docker compose up --build -d "$SERVICE_NAME"

echo "✅ Service '$SERVICE_NAME' restarted cleanly."