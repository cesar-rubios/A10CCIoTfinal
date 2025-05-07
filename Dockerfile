# 1. Imagen base: última versión de Ubuntu
FROM ubuntu:latest

# 2. Instala dependencias necesarias
RUN apt-get update && apt-get install -y wget apt-transport-https git sudo

# 3. Instala .NET SDK (ajusta la versión si hace falta)
RUN wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0

# 4. Copia los scripts de inicio si corresponde (como entrypoint.sh)
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]