# FFmpeg Audio Converter

FFmpeg wrapper for dotnet. Converts mp3 and wav files to wav (8000kHz 8bit mono, MuLaw)

## Building ffmpeg binaries for alpine and windows
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
```powershell
dotnet build --configuration Release ./FFmpeg.AudioConverter/FFmpeg.AudioConverter.csproj -p:Version=${NUGET_VERSION}
sn -R ./FFmpeg.AudioConverter/bin/Release/net6.0/FFmpeg.AudioConverter.dll <path to .snk>
dotnet pack --no-build --configuration Release ./FFmpeg.AudioConverter/FFmpeg.AudioConverter.csproj -p:Version=${NUGET_VERSION}
```

## Works on
* Windows
* Alpine:3.15
* Ubuntu if `apt-get install musl` is installed
