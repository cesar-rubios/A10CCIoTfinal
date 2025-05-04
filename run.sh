#!/bin/bash
cd /home/azureuser/app
pwd >> direccion.txt
dotnet simulated-device-2.dll > output.log 2>&1 &