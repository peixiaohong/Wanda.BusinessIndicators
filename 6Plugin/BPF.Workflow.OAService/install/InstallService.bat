@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo ���Թ���Ա������д�����
echo ȷ���޸��˷������ƺ�ִ���ļ���ַ��ִ���ļ�������Release�汾����
echo Installing WindowsService...
echo ---------------------------------------------------
sc create LJTH.JYZBGK.OAMQServices binpath= "D:\jyzbgk\winService\BPF.Workflow.OAService\BPF.OAMQServices.exe"
echo ---------------------------------------------------
echo Done.
pause