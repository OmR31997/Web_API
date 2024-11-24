# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the .csproj file to the container
COPY EntertaimentLib_API.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy the entire project to the container
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build-env /app/out ./

# Expose the default port
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "EntertaimentLib_API.dll"]
