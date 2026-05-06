# KiwiConnect

KiwiConnect is a community-oriented content and information exchange platform. This repository contains:

- `web/`: the public-facing frontend, built with Vue 3 and Vite
- `admin-vue3/`: the admin console, built with Vue 3 and Vite
- `server-dotnet/`: the backend API service, built with .NET 8 and ASP.NET Core Web API
- `KiwiConnect.sln`: the Visual Studio solution file used to manage the backend project

## Repository Structure

```text
KiwiConnect/
├── web/            # Public website
├── admin-vue3/     # Admin console
├── server-dotnet/  # .NET 8 backend API
└── KiwiConnect.sln # Solution file
```

## Features

- User registration, login, and profile management
- Content browsing, search, comments, and replies
- Help, feedback, report, and appeal workflows
- Admin-side content review, user management, carousel, and announcement management
- File upload and image asset handling

## Tech Stack

- Frontend: Vue 3, Vue Router, Vuex, Element Plus, Vite, Axios
- Backend: .NET 8, ASP.NET Core Web API, MySQL, JWT
- Development tools: Visual Studio, VS Code

## Local Setup

### 1. Start the Backend

```bash
cd server-dotnet
dotnet restore
dotnet run
```

By default, the backend runs at `http://localhost:3000`.

### 2. Start the Public Frontend

```bash
cd web
npm install
npm run dev
```

The public frontend development server runs at `http://localhost:5050` by default.

### 3. Start the Admin Console

```bash
cd admin-vue3
npm install
npm run dev
```

The admin console also uses a Vite development server. The exact port is shown in the terminal output.

## Common Scripts

### `web/`

- `npm run dev`: start the development server
- `npm run build`: build the production bundle
- `npm run preview`: preview the production build
- `npm run lint`: run code linting

### `admin-vue3/`

- `npm run dev`: start the development server
- `npm run build`: build the production bundle
- `npm run preview`: preview the production build
- `npm run lint`: run code linting
- `npm run lint:fix`: automatically fix lintable issues

### `server-dotnet/`

- `dotnet restore`: restore dependencies
- `dotnet run`: start the backend API
- See `server-dotnet/InfoExchange.Api.csproj` for project configuration

## Configuration Notes

- Backend configuration lives in `server-dotnet/appsettings.json`
- Database connection settings, JWT settings, and the upload directory are configured there
- The default upload directory is `uplodes`
- Frontend and admin requests typically depend on the backend API base URL, so adjust the proxy or base URL for your local environment

## Related Documentation

- [Frontend README](web/README.md)
- [Backend README](server-dotnet/README.md)

## Recommended Run Order

1. Start `server-dotnet`
2. Start `web`
3. Start `admin-vue3` if you need the admin console

This order is the easiest way to test login, content browsing, and admin workflows end to end.
