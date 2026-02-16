# NomNomApp

A full-stack recipe management and social platform where users can discover recipes, create their own, rate and comment on them, follow other users, and receive real-time notifications.

## Tech Stack

| Layer    | Technology                                      |
| -------- | ----------------------------------------------- |
| Frontend | React 19, TypeScript, Vite, Bootstrap           |
| Backend  | ASP.NET Core 10 (.NET 10), C#                   |
| Database | PostgreSQL 16 (Docker)                           |
| ORM      | Entity Framework Core 10                         |
| Auth     | JWT Bearer tokens in HTTP-only cookies           |
| Storage  | Cloudinary (photo uploads)                       |
| Realtime | Server-Sent Events (SSE)                         |
| External | Spoonacular API (recipe data)                    |

## Features

- **Recipe Discovery** - Search recipes by nutrients via the Spoonacular API
- **Custom Recipes** - Create and save your own recipes with ingredients and photos
- **Comments & Ratings** - Rate recipes (1-5 stars) and leave comments
- **User Profiles** - View profiles with follower/following lists
- **Follow System** - Follow other users and discover new people
- **Real-time Notifications** - Receive toast notifications for new follows and comments via SSE
- **Authentication** - Secure registration and login with JWT stored in HTTP-only cookies

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+)
- [Docker](https://www.docker.com/) (for PostgreSQL)

## Getting Started

### 1. Start the database

```bash
docker-compose up -d
```

This starts a PostgreSQL 16 container on port `5432`.

### 2. Run the backend

```bash
cd server/server
dotnet ef database update
dotnet run
```

The API starts at `https://localhost:5001`.

### 3. Run the frontend

```bash
cd client
npm install
npm run dev
```

The app starts at `http://localhost:3000`.

## Project Structure

```
NomNomApp/
├── client/                     # React frontend
│   └── src/
│       ├── components/         # Pages and UI components
│       ├── context/            # AuthContext, SseContext
│       ├── hooks/              # Custom hooks (useSse)
│       └── lib/api/            # Axios client and types
│
├── server/
│   ├── server/                 # ASP.NET Core API
│   │   ├── Data/               # EF Core DbContext
│   │   ├── Domain/             # Entity models
│   │   ├── Features/           # Feature-based modules
│   │   │   ├── Auth/           # Authentication & user management
│   │   │   ├── Comments/       # Recipe comments & ratings
│   │   │   ├── Follows/        # User follow system
│   │   │   ├── Recipes/        # Recipe CRUD & external API
│   │   │   ├── Sse/            # Real-time event streaming
│   │   │   └── Shared/         # Result pattern utilities
│   │   └── Migrations/         # Database migrations
│   └── server.Tests/           # Unit tests
│
└── docker-compose.yml          # PostgreSQL container
```

## API Endpoints

### Auth
| Method | Endpoint             | Description              |
| ------ | -------------------- | ------------------------ |
| POST   | `/api/auth/register` | Register a new user      |
| POST   | `/api/auth/login`    | Login                    |
| POST   | `/api/auth/logout`   | Logout (clear cookie)    |
| GET    | `/api/auth/me`       | Get current user         |
| GET    | `/api/auth/users`    | List all users           |

### Recipes
| Method | Endpoint              | Description                    |
| ------ | --------------------- | ------------------------------ |
| GET    | `/api/recipe/{id}`    | Get recipe by ID               |
| GET    | `/api/recipe/search`  | Search recipes by nutrients    |
| POST   | `/api/createrecipe`   | Create a custom recipe         |

### Comments
| Method | Endpoint                            | Description             |
| ------ | ----------------------------------- | ----------------------- |
| POST   | `/api/comments`                     | Add comment/rating      |
| GET    | `/api/comments/recipe/{recipeId}`   | Get comments for recipe |
| GET    | `/api/comments/recipe/{id}/score`   | Get average rating      |
| DELETE | `/api/comments/{id}`                | Delete a comment        |

### Follows
| Method | Endpoint                     | Description          |
| ------ | ---------------------------- | -------------------- |
| POST   | `/api/follows/{userId}`      | Follow a user        |
| DELETE | `/api/follows/{userId}`      | Unfollow a user      |
| GET    | `/api/follows/followers`     | Get your followers    |
| GET    | `/api/follows/following`     | Get who you follow    |
| GET    | `/api/follows/check/{userId}`| Check follow status  |

### SSE
| Method | Endpoint          | Description                  |
| ------ | ----------------- | ---------------------------- |
| GET    | `/api/sse/stream` | Subscribe to real-time events|

## Configuration

The backend requires the following configuration in `server/server/appsettings.json`:

- **ConnectionStrings:DefaultConnection** - PostgreSQL connection string
- **SpoonacularApi:ApiKey** - API key from [spoonacular.com](https://spoonacular.com/food-api)
- **CloudinarySettings** - Cloudinary account credentials for photo uploads
- **JwtSettings** - Secret key, issuer, audience, and token expiry

The frontend uses environment variables in `client/.env.development`:

- `VITE_API_URL` - Backend API base URL
- `VITE_SPOONACULAR_KEY` - Spoonacular API key

## Architecture

The backend follows a **feature-based architecture** where each feature (Auth, Recipes, Comments, Follows, SSE) is self-contained with its own controller, handler, and DTOs. Cross-cutting concerns use:

- **Result pattern** for consistent success/failure handling
- **Provider interfaces** for external services (Cloudinary, Spoonacular)
- **Dependency injection** throughout

The frontend uses React **Context API** for global state (auth and SSE) and **Axios** for API communication with automatic cookie-based credential handling.
