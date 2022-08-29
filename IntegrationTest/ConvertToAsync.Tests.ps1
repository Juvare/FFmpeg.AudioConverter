Describe 'ConvertToAsync' {
    BeforeAll {
        dotnet build "./Notify.Utils.Ffmpeg/Notify.Utils.Ffmpeg.csproj"
        Add-Type -Path "./Notify.Utils.Ffmpeg/bin/Debug/net6.0/Notify.Utils.Ffmpeg.dll"

        $converter = [Notify.Utils.Ffmpeg.AudioConverter]::new()

        $converter | Should -Not -Be $null

        Function Convert-File {
            param([string]$RelativePath)

            $path = Resolve-Path $RelativePath

            $format = [Notify.Utils.Ffmpeg.InputFormat]::Parse($path)
            $stream = [System.IO.File]::OpenRead($path)

            $output = $converter.ConvertToAsync($stream, $format).GetAwaiter().GetResult()

            $destinationPath = (Resolve-Path ./IntegrationTest/).Path + "converted.wav"
            Write-Host $destinationPath
            $writer = [System.IO.File]::OpenWrite($destinationPath)
            $output.WriteTo($writer)

            $writer.Dispose()
            $stream.Dispose()
            $output.Dispose()
        }

        Function Get-OutputProbed {
            $path = Resolve-Path ./IntegrationTest/converted.wav
            ./ffmpeg/ffprobe.exe -hide_banner -v quiet -print_format json -show_format -show_streams $path ` | ConvertFrom-Json
        }
    }

    BeforeEach {
        $path = Resolve-Path ./IntegrationTest/converted.wav -ErrorAction SilentlyContinue
        if ($path) {
            Remove-Item -Path $path -ErrorAction SilentlyContinue
        }
    }

    It 'Converts .mp3 file to 8K sample rate MuLaw wav file' {
        Convert-File -RelativePath './IntegrationTest/file_example_MP3_2MG.mp3'

        $info = Get-OutputProbed

        $info.streams[0].codec_name         | Should -Be "pcm_mulaw"
        $info.streams[0].sample_rate        | Should -Be "8000"
        $info.streams[0].channels           | Should -Be 1
        $info.streams[0].bits_per_sample    | Should -Be 8
        $info.format.format_name            | Should -Be "wav"
        $info.format.duration               | Should -Be "52.794625"
    }

    It 'Converts .wav file to 8K sample rate MuLaw wav file' {
        Convert-File -RelativePath './IntegrationTest/file_example_WAV_1MG.wav'

        $info = Get-OutputProbed

        $info.streams[0].codec_name         | Should -Be "pcm_mulaw"
        $info.streams[0].sample_rate        | Should -Be "8000"
        $info.streams[0].channels           | Should -Be 1
        $info.streams[0].bits_per_sample    | Should -Be 8
        $info.format.format_name            | Should -Be "wav"
        $info.format.duration               | Should -Be "33.529625"
    }
}