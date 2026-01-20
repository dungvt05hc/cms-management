import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    // Exclude Playwright e2e tests from Vitest
    exclude: [
      '**/node_modules/**',
      '**/dist/**',
      '**/cypress/**',
      '**/.{idea,git,cache,output,temp}/**',
      '**/e2e/**',
      '**/*.spec.ts',
    ],
    // Include only unit test files
    include: [
      '**/*.{test,unit}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}',
    ],
  },
});
