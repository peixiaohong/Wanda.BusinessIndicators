@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo 请以管理员身份运行此命令
echo 确认修改了服务名称和执行文件地址，执行文件必须以Release版本编译
echo Installing WindowsService...
echo ---------------------------------------------------
sc create LJTH.JYZBGK.EmployeeTransfer binpath= "D:\jyzbgk\winService\EmployeeTransfer\Plugin.EmployeeTransfer.exe"
echo ---------------------------------------------------
echo Done.
pause