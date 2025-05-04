#!/bin/bash
pwd >> direccion.txt
cd /home/azureuser/app
pwd >> direccion.txt
dotnet restore
pwd >> direccion.txt
dotnet run
pwd >> direccion.txt