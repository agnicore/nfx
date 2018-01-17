@echo off

set PROJECT_HOME=%AGNICORE_HOME%\
SET LAST=%PROJECT_HOME:~-1%
IF %LAST% NEQ \ (SET PROJECT_HOME=%PROJECT_HOME%\)

set DOCFX_PATH=%DOCFX_PATH%\
SET LAST=%DOCFX_PATH:~-1%
IF %LAST% NEQ \ (SET DOCFX_PATH=%DOCFX_PATH%\)


set NFX_HOME=%PROJECT_HOME%NFX\
set NFX_DOCS_PROJ=%NFX_HOME%elm\help\nfxlib\
set NFX_DOCS_OUTPUT=%NFX_HOME%out\help\

set DOCFX_EXE="%DOCFX_PATH%docfx.exe"

echo Removing previously generated YAML files ---------------------
for %%i in (%NFX_DOCS_PROJ%docs\*.yml) do if not "%%~nxi"=="toc.yml" del "%%i"

echo Building NFX API Documentation ---------------------
%DOCFX_EXE% %NFX_DOCS_PROJ%docfx.json -o %NFX_DOCS_OUTPUT%

echo Removing generated YAML files ---------------------
for %%i in (%NFX_DOCS_PROJ%docs\*.yml) do if not "%%~nxi"=="toc.yml" del "%%i"

if errorlevel 1 goto ERROR

echo Done Building Documentation!
goto :FINISH

:ERROR
 echo Error happened!
:FINISH
 pause




