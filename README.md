# Hotel Booking Platform

A full-stack hotel booking application developed as part of a DevOps course project.

The project uses a React frontend, an ASP.NET Core REST API, and a PostgreSQL database. The application is containerized using Docker and managed with Docker Compose.

## Table of Contents

- [Technology Stack](#technology-stack)
  - [Frontend](#frontend)
  - [Backend](#backend)
  - [Database](#database)
  - [DevOps & Development Tools](#devops--development-tools)
- [Application Architecture](#application-architecture)
- [Project Structure](#project-structure)
- [Repository and Version Control](#repository-and-version-control)
  - [Repository Setup](#repository-setup)
  - [Development Workflow](#development-workflow)
  - [Branch Protection](#branch-protection)

## Technology Stack

### Frontend

| Technology | Purpose                                       |
| ---------- | --------------------------------------------- |
| TypeScript | Programming language for frontend development |
| React      | Building reusable UI components               |
| Vite       | Frontend development and build tool           |
| HTML & CSS | Page structure and styling                    |
| NGINX      | Serving the production frontend               |

### Backend

| Technology            | Purpose                                                 |
| --------------------- | ------------------------------------------------------- |
| C#                    | Backend programming language                            |
| ASP.NET Core 10       | Building the REST API                                   |
| Entity Framework Core | Object-relational mapping (ORM) and database operations |
| Npgsql                | PostgreSQL provider for Entity Framework Core           |

### Database

| Technology         | Purpose                                  |
| ------------------ | ---------------------------------------- |
| PostgreSQL 18      | Relational database for application data |
| EF Core Migrations | Managing database schema changes         |

### DevOps & Development Tools

| Technology         | Purpose                                           |
| ------------------ | ------------------------------------------------- |
| Git                | Version control                                   |
| GitHub             | Source code hosting and collaboration             |
| GitLab             | Course repository synchronization                 |
| GitHub Actions     | Automated build validation (CI)                   |
| Docker             | Building images and running containers            |
| Docker Compose     | Managing frontend, backend, and database services |
| Docker Hub         | Storing and sharing Docker images                 |
| Visual Studio Code | Development environment                           |

## Application Architecture

The application consists of three services:

1. **Frontend:** A React application that provides the user interface and communicates with the backend through HTTP requests.
2. **Backend:** An ASP.NET Core REST API that handles application logic and database operations.
3. **Database:** PostgreSQL stores application data, including hotel rooms and bookings.

Docker Compose manages the services and provides an internal network for communication between the backend and database.

## Project Structure

```text
booking-room-platform/
├── backend/             # ASP.NET Core REST API
├── frontend/            # React + TypeScript application
├── docs/                # Project documentation
├── .github/workflows/   # CI/CD workflows
├── compose.yaml         # Docker Compose configuration
└── README.md            # Project overview
```

## Repository and Version Control

The project uses **Git** for version control and **GitHub** as the primary development platform.

A separate GitLab repository is maintained for course requirements.

### Repository Setup

- **GitHub:** Primary repository for source code, documentation, and collaboration.
- **GitLab:** Course repository synchronized with GitHub.
- **GitHub Actions:** Automated CI workflows for backend and frontend builds.

### Development Workflow

1. Create a feature branch for new functionality.
2. Implement and commit changes locally.
3. Push the branch to GitHub.
4. Open a Pull Request targeting `main`.
5. GitHub Actions automatically builds the backend and frontend.
6. Merge the Pull Request after successful CI checks.
7. The updated `main` branch is synchronized with GitLab.

### Branch Protection

The `main` branch is protected by repository rules.

Changes must be introduced through Pull Requests, and the required CI checks must pass before merging.

This helps prevent untested changes from entering the main branch.
