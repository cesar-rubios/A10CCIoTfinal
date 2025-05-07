   #!/bin/bash
   set -e

   git clone "$REPO_URL" /repo

   cd /repo

   dotnet run --project ./SimulatedDevice.cs