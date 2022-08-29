# Notify Utils ffmpeg

ffmpeg wrapper for dotnet

## Building ffmpeg binaries for aplipe and windows
`docker build --file ffmpeg.Dockerfile --output ffmpeg .` will compile ffmpeg for mp3 and wav conversion to wav files. Folder will contain following binaries:
* ffmpeg
* ffprobe
* ffmpeg.exe
* ffprobe.exe

> probe files are usefull to inspect audio files.
