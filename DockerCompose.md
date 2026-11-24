# Docker Compose Demonstration Guide

## 1. Introduction

Our Hotel Booking Platform consists of three services:

- **Frontend:** React application served by NGINX.
- **Backend:** ASP.NET Core Web API.
- **Database:** PostgreSQL 18.

We use Docker Compose to build, start, and manage these services together.

**Explain:**

Docker Compose allows us to define multiple containers in one configuration file instead of creating and starting each container manually.

Our configuration is stored in `compose.yaml`.

---

## 2. Show the Compose Configuration

### Backend

```yaml
backend:
  build:
    context: ./backend
  ports:
    - "5000:8080"
```

**Explain:**

- `build` tells Docker where to find the backend Dockerfile.
- `ports` maps port 5000 on our computer to port 8080 inside the container.

The backend also receives a database connection string through environment variables.

### Frontend

```yaml
frontend:
  build:
    context: ./frontend
  ports:
    - "8080:80"
```

**Explain:**

The frontend is built using its Dockerfile and served through NGINX.

We access it through port 8080 on our computer.

### Database

```yaml
db:
  image: postgres:18
```

**Explain:**

We use the official PostgreSQL image instead of creating our own database Dockerfile.

The database also has:

- A health check to verify PostgreSQL is ready.
- A named volume to preserve data.
- Environment variables for database configuration.

### Container Communication

**Explain:**

Docker Compose automatically creates a network for the application.

The backend connects to PostgreSQL using:

`Host=db`

Here, `db` is the Compose service name, which Docker resolves to the database container.

The backend waits until PostgreSQL passes its health check before starting.

---

## 3. Build and Start the Application

> Optionally: <code>docker compose down</code> - removes all Compose containers
> <code>docker compose ps -a</code> - verifies that there is no Compose container
> <code>docker compose up -d --build</code> - builds and starts everything

Navigate to the project root containing `compose.yaml`.

```bash
docker compose up -d --build
```

**What it does:**

- Builds the frontend and backend images.
- Creates containers if necessary.
- Starts all three services.
- Runs them in the background.

**Explain:**

One command starts our complete application.

The database uses the existing named volume, so rebuilding the application does not erase stored data.

---

## 4. Check Running Containers

```bash
docker compose ps
```

**What it checks:**

Displays the status of all services.

**Expected result:**

| Service  | Status            | Port            |
| -------- | ----------------- | --------------- |
| Backend  | Running           | 5000            |
| Frontend | Running           | 8080            |
| Database | Running (healthy) | 5432 (internal) |

**Explain:**

All three containers are running.

PostgreSQL reports a healthy status, meaning its health check succeeds.

The database port is available inside the Docker network and does not need to be exposed to our computer.

---

## 5. Test the Backend API

```bash
curl -i http://localhost:5000/api/rooms
```

For Windows try:

```powershell
curl.exe -i http://localhost:5000/api/rooms
```

OR interactively:

```bash
docker compose exec db psql -U hoteluser -d hotelbooking
```

**What it checks:**

Sends an HTTP GET request to the backend API.

**Expected result:**

- HTTP `200 OK`.
- JSON containing hotel rooms.

**Explain:**

The backend is accessible through port 5000.

The API retrieves room information from PostgreSQL and returns it as JSON.

This verifies that the backend and database can communicate.

---

## 6. Test the Frontend

Open:

`http://localhost:8080`

**What it checks:**

The React application loads and displays hotel rooms.

**Explain:**

NGINX serves our React application.

The frontend requests room information from the backend API and displays it to the user.

This demonstrates that the application works through its user interface.

**Important:** The browser communicates with the backend through the exposed API port. Docker's internal service-name resolution is used for communication between containers.

---

## 7. Verify PostgreSQL

```bash
docker compose exec db psql -U hoteluser -d hotelbooking -c 'SELECT * FROM "Rooms";'
```

**What it does:**

- `docker compose exec db` — executes a command inside the database container.
- `psql` — opens PostgreSQL's command-line interface.
- `-U hoteluser` — specifies the database user.
- `-d hotelbooking` — selects the database.
- `-c` — executes the SQL query.

**Expected result:**

A table containing stored hotel rooms.

**Explain:**

This confirms that room data exists in PostgreSQL.

The frontend displays information that originates from this database and is retrieved through our backend API.

---

## 8. Demonstrate Data Persistence (Optional)

Create a new booking (please, no more 'test' input), show newly created booking
<code> docker compose exec db psql -U hoteluser -d hotelbooking -c 'SELECT \* FROM "Bookings";'</code>

Stop and remove the application's containers:

```bash
docker compose down
```

**Explain:**

This removes the Compose containers and network, but preserves the named database volume.

Start the application again:

```bash
docker compose up -d
```

Verify the database:

```bash
docker compose exec db psql -U hoteluser -d hotelbooking -c 'SELECT * FROM "Rooms";'
```

**Explain:**

The rooms are still available because PostgreSQL stores its data in the named volume `postgres_data`.

The data is not lost when the database container is removed and recreated.

**Warning:** Do not use `docker compose down -v`, because it also removes the database volume.

---

## 9. Conclusion

We successfully demonstrated how Docker Compose manages our three-container application.

We verified that:

- All three services start successfully.
- The backend API returns room data.
- The frontend displays the data.
- PostgreSQL stores the information.
- Docker Compose provides networking between containers.
- The database volume preserves data between container recreations.

### Main Concepts to Remember

**Dockerfile vs. Docker Compose**

A Dockerfile describes how to build an image for one application component.

Docker Compose describes how multiple services work together.

**Image vs. Container**

An image is a packaged application and its dependencies.

A container is a running instance of an image.

**Docker Network**

Allows containers to communicate using service names, such as `db`.

**Docker Volume**

Stores data independently of the container's lifecycle.

**Health Check**

Checks whether a service is ready to accept connections, not just whether its container is running.
