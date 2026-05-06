# KiwiConnect Web

Modern frontend project built with Vue 3 + Element Plus + Vite

## Project Setup

### Install Dependencies

```bash
npm install
```

### Development Mode

```bash
npm run dev
```

The project will automatically open at `http://localhost:5050`.

### Production Build

```bash
npm run build
```

Build output is generated in the `dist/` directory.

### Preview Production Build

```bash
npm run preview
```

### Lint and Fix

```bash
npm run lint
```

## Project Structure

```
src/
├── assets/        # Static assets
├── components/    # Reusable components
├── router/        # Router configuration
├── store/         # Vuex state management
├── views/         # Page components
├── App.vue        # Root component
└── main.js        # Application entry
```

## Tech Stack

- **Vue**: 3.5.12
- **Vue Router**: 4.4.0
- **Vuex**: 4.1.0
- **Element Plus**: 2.7.5
- **Vite**: 5.4.0
- **Axios**: 1.7.7
- **ECharts**: 5.5.0

## Configuration

- Server runs at `http://localhost:5050`
- API proxy configuration: requests to `/api` are proxied to the backend server
- Default language: Chinese (`zh-CN`)

## Environment Requirements

- Node.js >= 16.x
- npm >= 8.x or yarn >= 1.22.x
