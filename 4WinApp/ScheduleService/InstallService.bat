@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo ���Թ���Ա������д�����
echo ȷ���޸��˷������ƺ�ִ���ļ���ַ��ִ���ļ�������Release�汾����
echo Installing WindowsService...
echo ---------------------------------------------------
sc create ServeiceName binpath= c:\Test\Common.ScheduleService.exe
echo ---------------------------------------------------
echo Done.
pause