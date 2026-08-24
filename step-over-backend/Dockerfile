# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GoalApi/GoalApi.csproj GoalApi/
RUN dotnet restore "GoalApi/GoalApi.csproj"

COPY GoalApi/ GoalApi/
WORKDIR /src/GoalApi
RUN dotnet publish "GoalApi.csproj" --configuration Release --output /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --chown=app:app --from=build /app/publish .

USER app
ENTRYPOINT ["dotnet", "GoalApi.dll"]
