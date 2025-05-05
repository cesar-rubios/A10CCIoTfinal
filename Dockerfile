# Imagen base de .NET para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Imagen de SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar el proyecto y restaurar dependencias
COPY simulated-device-2.csproj ./
RUN dotnet restore ./simulated-device-2.csproj

# Copiar el resto del código y compilar
COPY . ./
RUN dotnet build ./simulated-device-2.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ./simulated-device-2.csproj -c Release -o /app/publish

# Imagen final para ejecución
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "simulated-device-2.dll"]
