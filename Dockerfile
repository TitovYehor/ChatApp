# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the backend source, including the solution and all referenced projects
COPY backend/ ./backend/

# Publish the API and its project dependencies
RUN dotnet publish backend/src/ChatApp.Api/ChatApp.Api.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render provides PORT; 10000 is a fallback for local Docker runs
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet ChatApp.Api.dll --urls http://0.0.0.0:${PORT:-10000}"]