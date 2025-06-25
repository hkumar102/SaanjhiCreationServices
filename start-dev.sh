#!/bin/bash
echo "🔧 Building and starting services..."
docker compose up --build -d
echo "📡 Attaching to logs..."
docker compose logs -f
