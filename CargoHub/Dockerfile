# ---------- Stage 1: bouwen ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/CargoHub.Api/CargoHub.Api.csproj src/CargoHub.Api/        
RUN dotnet restore src/CargoHub.Api/CargoHub.Api.csproj          

COPY src/CargoHub.Api/ src/CargoHub.Api/                           
RUN dotnet publish src/CargoHub.Api/CargoHub.Api.csproj -c Release -o /app/publish  

# ---------- Stage 2: draaien ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CargoHub.Api.dll"]                        