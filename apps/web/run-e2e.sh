#!/bin/bash
# E2E Test Runner Script
# This script sets up the environment and runs E2E tests

set -e

echo "🚀 Setting up E2E test environment..."

# Check if API is running
if ! curl -s -f -o /dev/null http://localhost:5000/products?limit=1; then
  echo "⚠️  Backend API is not running on http://localhost:5000"
  echo "   E2E tests will run but may skip tests that require backend data"
  echo ""
fi

# Install Playwright browsers if needed
if [ ! -d "$HOME/.cache/ms-playwright" ]; then
  echo "📥 Installing Playwright browsers..."
  npx playwright install chromium
fi

# Run E2E tests
echo "🧪 Running E2E tests..."
npm run e2e

echo "✅ E2E tests complete!"
