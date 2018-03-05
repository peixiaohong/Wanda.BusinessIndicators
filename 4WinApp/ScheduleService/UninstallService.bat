@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo 请以管理员身份运行此命令
echo 确认修改了服务名称
echo UnInstalling WindowsService...
echo ---------------------------------------------------
sc delete ServiceName
echo ---------------------------------------------------
echo Done.
pause