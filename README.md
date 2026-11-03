# booking-room-platform

A room booking system built as a DevOps course project, focusing on software development, automation, deployment, and infrastructure.

### To start the project, follow these steps:

**Backend — ASP.NET Core**

```c#
cd C:\booking-room-platform\backend
dotnet run --no-launch-profile
```

> Expected address: http://localhost:5000 See: <code>Program.cs</code>

Rooms API: http://localhost:5000/api/rooms

**Frontend — React + Vite**

```c#
cd C:\booking-room-platform\frontend
npm.cmd run dev
```

> Expected address: http://localhost:5173 See: <code>vite.config.ts</code>

Backend requires database provider or other related dependecies. After ```pull request``` you need to run ```dotnet restore``` or ```dotnet build```.
.NET reads the project file and downloads the required package for the environment.

**Development Workflow**

The project uses GitHub Actions to automatically build the ASP.NET Core backend and React frontend. 
Changes to the protected main branch are introduced through pull requests. 
After merging, GitHub Actions synchronizes the updated branch with GitLab.

See `docs/ci-cd.md` for implementation details.
