# Docker Hub — Development Workflow Demonstration

## Objective

Demonstrate how developers can share and continue developing an application using Docker Hub.

**Workflow:**

Build → Push → Pull → Extract source → Modify → Rebuild → Test → Push

We use the ASP.NET Core backend of our Hotel Booking Platform.

**Docker Hub repository:** `karynamisnik/hotel-backend`

> **Note:** For this educational demonstration, our Docker image includes the source code. Production images normally contain only the compiled application.

---

## 1. Create a demonstration branch

From the project root:

```bash
git switch -c docker/demo
```

This keeps the demonstration changes separate from `main`.

### Modify `backend/Dockerfile`

In the runtime stage, add:

```dockerfile
COPY --from=build /app /source
```

This copies the application source code from the build stage into the final image.

The relevant Dockerfile section should look like:

```dockerfile
# Stage 2: Run the ASP.NET Core application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Copy compiled application
COPY --from=build /app/publish .

# Include source code for further development
COPY --from=build /app /source

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "backend.dll"]
```

---

## 2. Build Docker image v1

From the project root:

```bash
docker build -t karynamisnik/hotel-backend:v1 ./backend
```

Or

```bash
docker build -t muruseni/hotel-backend:v1 ./backend
```

**Explanation:**

- `docker build` — creates a Docker image.
- `-t` — assigns the image name and version.
- `./backend` — directory containing the Dockerfile and application source.

Verify:

```bash
docker images
```

---

## 3. Push v1 to Docker Hub

Log in if necessary:

```bash
docker login
```

Push:

```bash
docker push karynamisnik/hotel-backend:v1
```

Or

```bash
docker push muruseni/hotel-backend:v1
```

The image is now available on Docker Hub.

---

## 4. Simulate another developer pulling the image

Remove the local image:

```bash
docker rmi karynamisnik/hotel-backend:v1
```

This removes the local image tag, not the image stored on Docker Hub.

Pull the image:

```bash
docker pull karynamisnik/hotel-backend:v1
```

Or

```bash
docker pull muruseni/hotel-backend:v1
```

**Explanation:**

Another developer can now obtain the application from Docker Hub.

---

## 5. Create a container from the image

```bash
docker create --name hotel-source-demo karynamisnik/hotel-backend:v1
```

Or

```bash
docker create --name hotel-source-demo muruseni/hotel-backend:v1
```

**Explanation:**

- `docker create` — creates a container without starting it.
- `--name` — assigns a recognizable container name.

We don't need to run the application because we only want to extract its source code.

---

## 6. Extract the source code

Create a directory:

```bash
mkdir -p ~/hotel-source-demo
```

Copy the source code:

```bash
docker cp hotel-source-demo:/source/. ~/hotel-source-demo/
```

**Explanation:**

- `docker cp` — copies files between a container and the host computer.
- `/source/.` — copies all contents of the source directory.
- `~/hotel-source-demo/` — destination directory on Ubuntu.

Verify:

```bash
ls ~/hotel-source-demo
```

Expected files include:

```text
Program.cs
Models/
Services/
Controllers/
Data/
backend.csproj
```

---

## 7. Modify the application

Open the extracted project:

```bash
code ~/hotel-source-demo
```

Open:

```text
Services/BookingService.cs
```

Find:

```csharp
public async Task<Booking> CreateBookingAsync(Booking booking)
{
```

Add the following validation at the beginning of the method:

```csharp
if (string.IsNullOrWhiteSpace(booking.GuestName))
{
    throw new ArgumentException("Guest name is required.");
}
```

**Explanation:**

This is a business logic improvement.

Previously, the service checked booking dates, room existence, and overlapping reservations.

Now it also rejects bookings without a guest name.

Save the file.

---

## 8. Prepare the Dockerfile for rebuilding

Return to the original project directory.

Copy its Dockerfile into the extracted project:

```bash
cp backend/Dockerfile ~/hotel-source-demo/
```

**Explanation:**

The Dockerfile provides instructions for building the modified application.

---

## 9. Build Docker image v2

```bash
docker build -t karynamisnik/hotel-backend:v2 ~/hotel-source-demo
```

**Explanation:**

- Docker compiles the modified C# source code.
- The updated image is tagged `v2`.
- The original `v1` remains available.

Verify:

```bash
docker images
```

---

## 10. Run the updated application

Our PostgreSQL database is already running through Docker Compose.

Run the updated backend:

```bash
docker run -d \
  --name hotel-demo-v2 \
  --network booking-room-platform_default \
  -p 5001:8080 \
  -e "ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=hotelbooking;Username=hoteluser;Password=YOUR_PASSWORD" \
  karynamisnik/hotel-backend:v2
```

Replace `YOUR_PASSWORD` with the PostgreSQL password.

**Explanation:**

- `-d` — runs the container in the background.
- `--name` — assigns a container name.
- `--network` — connects the container to the existing Docker Compose network.
- `-p 5001:8080` — maps host port 5001 to container port 8080.
- `-e` — supplies the database connection string.

> This demonstration assumes the original Docker Compose application and database are running. For normal development, use environment files or secrets instead of typing database passwords into commands.

Verify:

```bash
docker ps
```

Check logs if necessary:

```bash
docker logs hotel-demo-v2
```

---

## 11. Test the modified business logic

Send an invalid booking request:

```bash
curl -i -X POST http://localhost:5001/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "guestName": "",
    "guestEmail": "test@example.com",
    "checkIn": "2027-06-10",
    "checkOut": "2027-06-12"
  }'
```

**Expected result:**

```text
HTTP/1.1 400 Bad Request
```

If the controller returns the exception message, we should also see:

```text
Guest name is required.
```

This confirms that the new validation is working.

---

## 12. Push v2 to Docker Hub

```bash
docker push karynamisnik/hotel-backend:v2
```

The updated application is now available to other developers.

**We have successfully demonstrated:**

1. Creating a Docker image.
2. Publishing it to Docker Hub.
3. Downloading the published image.
4. Extracting the application source code.
5. Modifying C# business logic.
6. Building a new image version.
7. Testing the updated application.
8. Publishing the updated version.

---

## 13. Clean up demonstration containers

Remove the temporary containers:

```bash
docker rm -f hotel-demo-v2
```

```bash
docker rm hotel-source-demo
```

Delete the extracted source directory:

```bash
rm -r ~/hotel-source-demo
```

**Explanation:**

- `docker rm` — removes a container.
- `-f` — forces removal, including stopping a running container.
- `rm -r` — recursively deletes a directory and its contents.

These commands do not remove the main Docker Compose application or its PostgreSQL volume.

### Optional: Remove demonstration images

```bash
docker rmi karynamisnik/hotel-backend:v1
docker rmi karynamisnik/hotel-backend:v2
```

This removes local image tags. The versions remain available on Docker Hub.

---

## Conclusion

This demonstration shows how Docker Hub can be used to distribute an application and support further development.

A developer downloads an image, extracts its source code, modifies the application, rebuilds it, tests the changes, and publishes an updated version.

In real-world development, source code is normally shared through Git repositories, while Docker Hub distributes built application images.
