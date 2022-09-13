# FFmpeg Audio Converter

FFmpeg wrapper for dotnet. Converts mp3 and wav files to wav (8000kHz 8bit mono, MuLaw)

## Usage
```csharp
using FFmpeg.AudioConverter;

var converter = new AudioConverter();
// implements IAudioConverter interface so can be easily added to DI container and mocked

// convert mp3 to wav
using var mp3Stream = File.OpenRead(@"path.mp3");
using var wavStream = await converter.ConvertToWavAsync(mp3Stream, InputFormat.MP3, CancellationToken.None);

// save output to file
using var outputStream = File.OpenWrite(@"C:\temp\mulaw.wav");
await wavStream.CopyToAsync(outputStream);
await outputStream.FlushAsync();
```

## Works on
* Windows
* Alpine:3.15
* Ubuntu if `apt-get install musl` is installed

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
