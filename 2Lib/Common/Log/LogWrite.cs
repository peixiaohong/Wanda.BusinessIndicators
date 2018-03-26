using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Threading;


namespace LJTH.BusinessIndicators.Common
{
    /// <summary>
    /// GeneralOfficePortal LogHelp
    /// </summary>
    public class LogWrite
    {
        static private string _logDirName = @"Log";
        static private string _filePreName = "Log";
        static Mutex m_WriteMutex = new Mutex();

        public LogWrite()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        static public bool WriteLine(string dataText)
        {
            bool ret = false;

            ret = WriteLine_Create(dataText);

            return ret;
        }

        static public bool WriteLine_Create(string dataText)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            bool ret = true;
            m_WriteMutex.WaitOne();
            string dirStr = Path.DirectorySeparatorChar.ToString();
            try
            {
                //SPSecurity.RunWithElevatedPrivileges(delegate()
                //{
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory;
                if (!filePath.EndsWith(dirStr))
                {
                    filePath += dirStr;
                }
                string FileName = filePath + _logDirName + dirStr;
                //CHECK文件目录是否存在
                if (!Directory.Exists(FileName))
                {
                    Directory.CreateDirectory(FileName);
                }
                FileName += _filePreName + WebHelper.DateTimeNow.ToString("yyyyMMdd") + ".txt";
                //CHECK文件是否存在
                if (!File.Exists(FileName))
                {
                    FileStream tempfs = File.Create(FileName);
                    tempfs.Close();
                }

                fs = new FileStream(
                    FileName,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.None);
                fs.Seek(0, System.IO.SeekOrigin.End);
                sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                string LineText = WebHelper.DateTimeNow.ToString("yyy-MM-dd ") + WebHelper.DateTimeNow.ToString("T") + "\r\n" + dataText;
                sw.WriteLine(LineText);
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                //  });
            }

            catch (Exception)
            {
                ret = false;
            }

            finally
            {
                try
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw = null;
                    }

                    if (fs != null)
                    {
                        fs.Close();
                        fs = null;
                    }
                }

                catch
                {

                }
                m_WriteMutex.ReleaseMutex();
            }
            return ret;
        }

        static public void WriteFile(string filename, byte[] dataText)
        {
            try
            {
                System.IO.FileInfo finfo = new System.IO.FileInfo(filename);
                FileStream fs = finfo.OpenWrite();
                fs.Write(dataText, 0, dataText.Length);
                fs.Close();
                return;

            }

            catch (Exception)
            {
                return;
            }

        }

        static public bool WriteException(Exception ex, int errorRows)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                bool ret = false;
                sb.AppendLine("ErrInfo:" + ex.Message);
                sb.AppendLine("ErrDataRows:" + errorRows);
                // sb.AppendLine("ErrObject:" + ex.Source);
                if (ex.TargetSite != null)
                {
                    sb.AppendLine("ErrMethod:" + ex.TargetSite.Name);
                }
                sb.AppendLine("ErrPosition:" + ex.StackTrace);

                ret = WriteLine(sb.ToString());

                return ret;

            }
            catch
            {
                throw ex;
            }

        }
    }

}