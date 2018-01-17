@echo off

SET PROJECT_HOME=%AGNICORE_HOME%
SET LAST=%PROJECT_HOME:~-1%
IF %LAST% NEQ \ (SET PROJECT_HOME=%PROJECT_HOME%\)


set NFX_HOME=%PROJECT_HOME%NFX\

set DOTNET_FRAMEWORK_DIR=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin

set MSBUILD_EXE="%DOTNET_FRAMEWORK_DIR%\MSBuild.exe"
set BUILD_ARGS=/t:Restore;Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /p:DefineConstants="DEBUG;TRACE" /p:METABASE=dev /verbosity:normal /maxcpucount:1

echo Building DEBUG NFX ---------------------------------------
%MSBUILD_EXE% "%NFX_HOME%src\NFXv5.sln" %BUILD_ARGS%
if errorlevel 1 goto ERROR

echo Done!
goto :FINISH

:ERROR
 echo Error happened!
:FINISH
 pause