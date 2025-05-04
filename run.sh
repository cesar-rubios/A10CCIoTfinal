#!/bin/bash
cd /home/azureuser/app
dotnet restore
nohup dotnet run > output.log 2>&1 &