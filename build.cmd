@ECHO OFF

ECHO.
ECHO APPVEYOR_BUILD_NUMBER : %APPVEYOR_BUILD_NUMBER%
ECHO APPVEYOR_BUILD_VERSION : %APPVEYOR_BUILD_VERSION%

IF EXIST .\artifacts (
	ECHO Cleaning up artifacts folder
	DEL .\artifacts /q /s
) 

SET RELEASE=%APPVEYOR_BUILD_VERSION%
SET COMMENT=%MAYBE_PRERELEASE_SUFFIX%
IF [%COMMENT%]==[] (SET VERSION=%RELEASE%) ELSE (SET VERSION=%RELEASE%-%COMMENT%)
IF NOT [%1]==[] (SET VERSION=%1)

ECHO arg is %1
ECHO version is [%VERSION%]==[]

IF [%VERSION%]==[] (
	ECHO No version was provided, either via command line or appveyor environment variables.
	ECHO   Example: build 0.1.1-alpha
	GOTO :error
)

ECHO.
ECHO Building CallMeMaybe %VERSION%
ECHO.

CALL dotnet restore CallMeMaybe.sln
CALL dotnet msbuild CallMeMaybe.sln /p:Configuration=Release

powershell -Command ".\UpdateNuSpec.ps1 -version " %VERSION%

CALL dotnet pack CallMeMaybe\CallMeMaybe.csproj --include-symbols /p:NuspecFile=CallMeMaybe.nuspec /p:PackageVersion=%VERSION% -o ..\artifacts

:success
ECHO.
ECHO No errors were detected!
ECHO There may still be some in the output, which you would need to investigate.
ECHO Warnings are usually normal.
ECHO.
GOTO :EOF

:error

ECHO.
ECHO Errors were detected!
ECHO.