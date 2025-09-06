# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# Copy project files
COPY Ovation.WebAPI/Ovation.WebAPI.csproj ./Ovation.WebAPI/
COPY Ovation.Domain/Ovation.Domain.csproj ./Ovation.Domain/
COPY Ovation.Application/Ovation.Application.csproj ./Ovation.Application/
COPY Ovation.Infrastructure/Ovation.Persistence.csproj ./Ovation.Infrastructure/

# Restore dependencies
RUN dotnet restore Ovation.WebAPI/Ovation.WebAPI.csproj

# Copy the remaining source files
COPY . .

# Build and publish the WebAPI project
WORKDIR /src/Ovation.WebAPI
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build-env /out .

# Set the environment to Production
ENV ASPNETCORE_ENVIRONMENT Production

# Run the WebAPI
ENTRYPOINT ["dotnet", "Ovation.WebAPI.dll"]
