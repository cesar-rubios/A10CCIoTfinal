# Usar la imagen base de .NET
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Usar la imagen de SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar el proyecto y restaurar dependencias
COPY ["simulated-device-2/simulated-device-2.csproj", "simulated-device-2/"]
RUN dotnet restore "simulated-device-2/simulated-device-2.csproj"

# Copiar el código fuente y construir la aplicación
COPY . .
WORKDIR "/src/simulated-device-2"
RUN dotnet build "simulated-device-2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "simulated-device-2.csproj" -c Release -o /app/publish

# Crear la imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "simulated-device-2.dll"]
