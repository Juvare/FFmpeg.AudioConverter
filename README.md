# Notify Utils ffmpeg

ffmpeg wrapper for dotnet

## Building ffmpeg binaries for aplipe and windows
`docker build --file ffmpeg.Dockerfile --output ffmpeg .` will compile ffmpeg for mp3 and wav conversion to wav files. Folder will contain following binaries:
* ffmpeg
* ffprobe
* ffmpeg.exe
* ffprobe.exe

> probe files are usefull to inspect audio files.

## Integration tests
To run tests execute:
```powershell
Install-Module -Name Pester -Force # if not already installed or old version
Import-Module Pester
Invoke-Pester -Path .\IntegrationTest\ -Show All
```

## Create nuget
`dotnet pack --configuration Release ./Notify.Utils.Ffmpeg/Notify.Utils.Ffmpeg.csproj -p:Version=${NUGET_VERSION}`
