FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build-env
WORKDIR /CarAllowedApi

# Copy csproj and restore
COPY CarAllowedApi.csproj .
RUN dotnet restore "CarAllowedApi.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "CarAllowedApi.csproj" -c Release -o /build

# Publish
RUN dotnet publish "CarAllowedApi.csproj" -c Release -o /publish

# Final stage - ใช้ aspnet runtime image (เล็กกว่า sdk)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Copy published output
COPY --from=build-env /publish .

# Expose port (default ASP.NET Core ใช้ port 8080)
EXPOSE 8080

# Set entry point
ENTRYPOINT ["dotnet", "CarAllowedApi.dll"]