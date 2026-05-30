# BikeRental UI - Modern Frontend

React 19 + TypeScript single-page application replacing the legacy static HTML pages.

## Modernization Summary

The legacy app served three static HTML files (index.html, beach-cruisers.html, mountain-bikes.html) directly from IIS with zero interactivity. Users had to reload the entire page for every action. There was no styling system, no client-side state, and no separation between data and presentation.

This modern frontend introduces:

- **Component-based architecture** - Each UI element is an isolated, reusable component with its own props and state
- **Client-side routing** - Navigate between pages without full reloads (path-based routing in App.tsx)
- **Typed API layer** - Every backend response is typed via TypeScript interfaces, catching shape mismatches at compile time
- **Real-time feedback** - Toast notifications, loading states, and animated transitions instead of page reloads
- **Responsive layout** - CSS Grid adapts from 3-column desktop to single-column mobile automatically
- **Post-rental upsell flow** - After renting a bike, a modal offers accessories with live price calculation and bundle discount

## Technology Choices and Why

| Technology | Role | Why It Was Chosen |
|-----------|------|-------------------|
| **React 19** | UI library | Component model, hooks for state, massive ecosystem, no class components needed |
| **TypeScript** | Type safety | Catches API contract mismatches at build time, self-documenting interfaces |
| **Vite 6** | Build tool | Sub-second HMR in dev, fast production builds, native ES modules |
| **CSS Custom Properties** | Styling | Design tokens without external dependencies, zero runtime cost |
| **Fetch API** | HTTP client | Native browser API, no axios/library overhead needed for simple REST calls |

## What Each Part Does

**src/services/api.ts** - Single typed fetch wrapper. All API calls go through one function that handles errors, sets headers, and parses JSON. If the backend changes a response shape, TypeScript will flag every consumer.

**src/types/index.ts** - Mirrors the backend DTOs exactly. BeachCruiser, MountainBike, Accessory, RentResponse, OrderResponse. Acts as the contract between frontend and backend.

**src/pages/** - Three page components (Home, BeachCruisers, MountainBikes). Each fetches its own data on mount, manages rental state, and renders a grid of cards.

**src/components/** - Presentational cards (BeachCruiserCard, MountainBikeCard) display bike data with pricing and availability. AccessoryModal handles the post-rental upsell with quantity controls and bundle discount calculation.

**src/index.css** - Complete design system in one file. CSS variables define colors, spacing, shadows. No preprocessor, no CSS-in-JS, no external framework. Includes responsive breakpoints, animations, and component styles.

## Architecture Decisions

- **No router library** - The app has only 3 routes. window.location.pathname is sufficient. Adding react-router would be overhead for no benefit.
- **No state management library** - Component-local useState is enough. No global store needed when each page owns its data.
- **No CSS framework** - Custom CSS avoids bundle size and gives full control over the design. The entire stylesheet is under 300 lines.
- **No build-time linting** - ESLint was removed. TypeScript already catches most issues. Added complexity wasn't justified for this scope.

## Running

Requires Node.js 18+ and the backend API running on port 5035.

```
npm install --legacy-peer-deps
npm run dev
```

Opens at http://localhost:5173. The API URL is configured in .env (VITE_API_URL=http://localhost:5035/api).
