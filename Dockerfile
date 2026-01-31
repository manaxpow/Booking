# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["Project.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Api/Api.csproj", "Api/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Models/Models.csproj", "Models/"]

# Restore nuget packages
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish

# Final
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]