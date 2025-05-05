#!/bin/bash

echo "[INFO] --- Despliegue iniciado ---"
date

# Matar procesos anteriores del dotnet que corran esa app
echo "[INFO] Matando procesos anteriores..."
pkill -f "dotnet simulated-device-2.dll"

# Esperar unos segundos para liberar recursos (como puertos)
sleep 3

# Cambiar al directorio de la app
cd /home/azureuser/app || { echo "[ERROR] No se pudo acceder a /home/azureuser/app"; exit 1; }

# Lanzar la app en segundo plano
echo "[INFO] Lanzando aplicación..."
nohup dotnet simulated-device-2.dll > output.log 2>&1 &

echo "[INFO] Aplic
