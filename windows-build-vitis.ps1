. .\windows-scripts.ps1
$env:path += ";.\flexmlrt\share\cmake\FlexmlRT"
$env:CMAKE_GENERATOR = "Visual Studio 18 2026"
BuildWindows -Configuration "Release" -Arch "x64" -Vitis $true
nuget pack runtimes/Whisper.net.Runtime.VitisAI.Windows.nuspec -Version "1.0.0" -OutputDirectory ./nupkgs