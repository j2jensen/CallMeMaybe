@ECHO OFF

ECHO.

:: ensure we have Version.txt
IF NOT EXIST Version.txt (
	ECHO Version.txt is missing!
	GOTO error
)

:: get the version and comment from Version.txt lines 2 and 3
SET RELEASE=
SET COMMENT=
FOR /F "skip=1 delims=" %%i IN (Version.txt) DO IF NOT DEFINED RELEASE SET RELEASE=%%i
FOR /F "skip=2 delims=" %%i IN (Version.txt) DO IF NOT DEFINED COMMENT SET COMMENT=%%i

SET VERSION=%RELEASE%
IF [%COMMENT%] EQU [] (SET VERSION=%RELEASE%) ELSE (SET VERSION=%RELEASE%-%COMMENT%)

ECHO.
ECHO Building CallMeMaybe %VERSION%
ECHO.

CALL dotnet restore ..\CallMeMaybe.sln
CALL dotnet msbuild ..\CallMeMaybe.sln /t:Publish /p:OutputPath=..\build\temp\artifacts /p:Configuration=Release

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