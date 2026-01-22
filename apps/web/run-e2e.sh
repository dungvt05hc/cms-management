#!/bin/bash
# E2E Test Runner Script
# This script starts the backend API, frontend, and runs E2E tests

set -e

echo "🚀 Setting up E2E test environment..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Store PIDs for cleanup
API_PID=""
WEB_PID=""

# Cleanup function
cleanup() {
  echo ""
  echo "🧹 Cleaning up..."
  
  if [ ! -z "$API_PID" ]; then
    echo "Stopping backend API (PID: $API_PID)..."
    kill $API_PID 2>/dev/null || true
  fi
  
  if [ ! -z "$WEB_PID" ]; then
    echo "Stopping frontend (PID: $WEB_PID)..."
    kill $WEB_PID 2>/dev/null || true
  fi
  
  echo "✅ Cleanup complete"
}

# Set trap to cleanup on exit
trap cleanup EXIT INT TERM

# Navigate to backend directory
cd ../../src

# Check if backend is already running
if curl -s -f -o /dev/null http://localhost:5000/health; then
  echo -e "${YELLOW}⚠️  Backend API is already running on http://localhost:5000${NC}"
else
  echo "🔧 Starting backend API..."
  dotnet run --project Api/Api.csproj --no-build -c Release > /tmp/api-logs.txt 2>&1 &
  API_PID=$!
  
  # Wait for API to be ready
  echo "⏳ Waiting for backend API to start..."
  for i in {1..30}; do
    if curl -s -f -o /dev/null http://localhost:5000/health; then
      echo -e "${GREEN}✅ Backend API is ready!${NC}"
      break
    fi
    if [ $i -eq 30 ]; then
      echo -e "${RED}❌ Backend API failed to start within 30 seconds${NC}"
      echo "Logs from API:"
      cat /tmp/api-logs.txt
      exit 1
    fi
    sleep 1
  done
fi

# Navigate back to web directory
cd ../apps/web

# Install Playwright browsers if needed
if [ ! -d "$HOME/.cache/ms-playwright" ]; then
  echo "📥 Installing Playwright browsers..."
  npx playwright install chromium
fi

# Run E2E tests (Playwright will start the frontend automatically)
echo "🧪 Running E2E tests..."
npm run e2e

echo -e "${GREEN}✅ E2E tests complete!${NC}"
