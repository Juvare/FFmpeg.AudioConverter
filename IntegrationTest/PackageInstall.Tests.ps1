Describe 'Package install' {
    AfterAll {
        Get-Item ./IntegrationTest/nugets | Remove-Item -Recurse
        Get-Item ./IntegrationTest/TestProject | Remove-Item -Recurse
    }

    It 'Creates nuget package successfully' {
        dotnet pack --configuration Release ./Notify.Utils.Ffmpeg/Notify.Utils.Ffmpeg.csproj -p:Version=0.1 -o ./IntegrationTest/nugets

        Test-Path ./IntegrationTest/nugets/Notify.Utils.Ffmpeg.0.1.0.nupkg | Should -Be $true
    }

    It 'Copies ffmpeg binaries to bin dir' {
        #mkdir ./IntegrationTest/TestProject
        dotnet new console -o ./IntegrationTest/TestProject --force
        dotnet add ./IntegrationTest/TestProject/TestProject.csproj package -s ./IntegrationTest/nugets/ Notify.Utils.Ffmpeg
        dotnet build ./IntegrationTest/TestProject/TestProject.csproj

        Test-Path ./IntegrationTest/TestProject/bin/Debug/net6.0/ffmpeg/ffmpeg     | Should -Be $true
        Test-Path ./IntegrationTest/TestProject/bin/Debug/net6.0/ffmpeg/ffmpeg.exe | Should -Be $true
    }
}