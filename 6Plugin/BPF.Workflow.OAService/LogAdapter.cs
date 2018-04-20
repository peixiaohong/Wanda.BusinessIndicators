using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace Lvl
{
    /// <summary>
    /// ��־(Ĭ����־����ΪError.����Ϊ:Debug, Warn, Error, Fatal, Info),����Infoʼ��д��־
    /// </summary>
    public class LogAdapter
    {
        private static string basepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
        private static LogLevel globallevel = LogLevel.Error;
        private static string timeformat = "yyyy-MM-dd HH:mm:ss:fff  ";
        private string type;
        private static Dictionary<string, LogAdapter> dictlog = new Dictionary<string, LogAdapter>();
        private static LogAdapter app = null;
        private string logFilePath = null;
        private LogLevel level = LogLevel.None;
        internal static System.Globalization.CultureInfo cnci = new System.Globalization.CultureInfo("zh-cn");
        private const int PulseCount = 800;
        private int currentPulseCount = 0;
        #region ��������

        /// <summary>
        /// ��ȡ��������־�ļ��ĸ�Ŀ¼
        /// </summary>
        public static string BasePath
        {
            get
            {
                return basepath;
            }
            set
            {
                basepath = value;
            }
        }

        /// <summary>
        /// ��ȡ��������־��¼�ļ���(Ĭ��ΪNone.����Ϊ:Debug, Warn, Error, Fatal, Info)
        /// <para>
        /// �������ΪNone,��ʹ��ȫ��Ĭ�ϵļ���
        /// </para>
        /// </summary>
        public LogLevel Level
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
            }
        }

        /// <summary>
        /// ��ȡ��������־��¼�ļ���(Ĭ��ΪError.����Ϊ:Debug, Warn, Error, Fatal, Info)
        /// </summary>
        public static LogLevel GlobalLevel
        {
            get
            {
                return globallevel;
            }
            set
            {
                globallevel = value == LogLevel.None ? LogLevel.Error : value;
            }
        }

        /// <summary>
        /// �ܷ��¼������Ϣ
        /// </summary>
        public static bool CanDebug
        {
            get
            {
                return (int)LogLevel.Debug >= (int)LogAdapter.globallevel;
            }
        }

        /// <summary>
        /// �ܷ��¼������Ϣ
        /// </summary>
        public static bool CanError
        {
            get
            {
                return (int)LogLevel.Error >= (int)LogAdapter.globallevel;
            }
        }

        /// <summary>
        /// �ܷ��¼��������
        /// </summary>
        public static bool CanFatal
        {
            get
            {
                return (int)LogLevel.Fatal >= (int)LogAdapter.globallevel;
            }
        }

        /// <summary>
        /// �ܷ��¼������Ϣ
        /// </summary>
        public static bool CanWarn
        {
            get
            {
                return (int)LogLevel.Warn >= (int)LogAdapter.globallevel;
            }
        }

        #endregion

        /// <summary>
        /// ϵͳĬ����־ʵ��
        /// </summary>
        public static LogAdapter App
        {
            get
            {
                return app;
            }
        }

        /// <summary>
        /// ������־�����,��������Ϊһ������·�������·��
        /// </summary>
        public string LogFilePath
        {
            set
            {
                if (Path.IsPathRooted(value))
                {
                    this.logFilePath = value;
                }
                else
                {
                    this.logFilePath = Path.Combine(LogAdapter.basepath, value);
                }
            }
        }

        #region ���캯��
        /// <summary>
        /// ��̬���캯��,�Զ���ʼ��,ֻ��ʼ��һ�Ρ�
        /// </summary>
        static LogAdapter()
        {

            LogSection log = (LogSection)System.Configuration.ConfigurationManager.GetSection("log");
            if (log != null)
            {
                #region ����·��
                string path = log.Path.Trim();
                if (path.Length > 0)
                {
                    if (System.IO.Path.IsPathRooted(path))
                    {
                        basepath = path;
                    }
                    else
                    {
                        basepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                    }
                }
                #endregion

                #region ������־����
                LogAdapter.globallevel = log.Level;
                #endregion
            }
            app = GetLogger("App");
        }

        /// <summary>
        /// ��ȡLogAdapter���һ��ʵ��
        /// </summary>
        /// <param name="type">����</param>
        /// <returns>LogAdapter���һ��ʵ��</returns>
        public static LogAdapter GetLogger(string type)
        {
            LogAdapter log = null;
            string lowertype = type.ToLower();
            if (dictlog.ContainsKey(lowertype))
            {
                log = dictlog[lowertype];
            }
            else
            {
                log = new LogAdapter(type);
                dictlog.Add(lowertype, log);
            }
            return log;
        }

        /// <summary>
        /// ��ȡLogAdapter���һ��ʵ��
        /// </summary>
        /// <param name="type">����</param>
        /// <returns>LogAdapter���һ��ʵ��</returns>
        public static LogAdapter GetLogger(Type type)
        {
            return new LogAdapter(type.FullName);
        }
        /// <summary>
        /// ʹ��ָ�������Ƴ�ʼ��һ����־������
        /// </summary>
        /// <param name="type">��־����</param>
        private LogAdapter(string type)
        {
            this.type = type;
        }

        #endregion

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="level">��־����</param>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="type">��־���ݵ�����(�ᰴ�������������ļ���)</param>
        /// <param name="msg">��־����</param>
        /// <param name="compact">�Ƿ�ʹ��ѹ���ĸ�ʽ����¼��־��Ϣ</param>
        private void Write(LogLevel level, LogFileSpan sequence, string type, object msg, bool compact)
        {
            if (level == LogLevel.Info || (this.level == LogLevel.None ? (int)level >= (int)LogAdapter.globallevel : (int)level >= (int)this.level)) //����д��־
            {
                #region ʹ��ϵͳ������ļ�
                string path = this.logFilePath;
                if (string.IsNullOrEmpty(path))
                {
                    switch (sequence)
                    {
                        case LogFileSpan.None:
                            {
                                path = string.Format("{1}{0}{2}.txt", '_', level, type);
                                break;
                            }
                        case LogFileSpan.Year:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", '_', level, type, DateTime.Now.Year);
                                break;
                            }
                        case LogFileSpan.Month:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", '_', level, type, DateTime.Now.ToString("yyyyMM"));
                                break;
                            }
                        case LogFileSpan.Week:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}{4}��.txt", '_', level, type, DateTime.Now.Year, cnci.Calendar.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Friday));
                                break;
                            }
                        case LogFileSpan.Hour:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", '_', level, type, DateTime.Now.ToString("yyyyMMddHH"));
                                break;
                            }
                        case LogFileSpan.Lateast:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", '_', level, type, DateTime.Now.ToString("yyyyMMddHH"));
                                break;
                            }
                        default:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", '_', level, type, DateTime.Now.ToString("yyyyMMdd"));
                                break;
                            }
                    }
                    path = Path.Combine(LogAdapter.basepath, path);
                }
                try
                {
                    string dir = Path.GetDirectoryName(path);
                    if (!Path.IsPathRooted(dir))
                    {
                        dir = dir.Trim().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
                        dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
                    }
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    if (level == LogLevel.Pulse && this.currentPulseCount >= PulseCount && File.Exists(path))
                    {
                        string txt = null;
                        this.currentPulseCount = 0;
                        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
                        {
                            int totalCount = 0;
                            while (sr.ReadLine() != null)
                            {
                                totalCount++;
                            }
                            sr.BaseStream.Seek(0, SeekOrigin.Begin);
                            while (currentPulseCount++ < totalCount / 2)
                            {
                                sr.ReadLine();
                            }
                            txt = sr.ReadToEnd();
                        }
                        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                        {
                            sw.BaseStream.SetLength(0);
                            sw.Write(txt);
                        }
                    }
                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                    {
                        if (compact)
                        {
                            sw.Write(DateTime.Now.ToString(timeformat));
                            sw.WriteLine(msg.ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                        }
                        else
                        {
                            sw.Write("������������������������������������������  ");
                            sw.Write(DateTime.Now.ToString(timeformat));
                            sw.WriteLine("������������������������������������������");
                            sw.WriteLine(msg);
                        }
                        if (!compact)
                        {
                            sw.WriteLine();
                        }
                        sw.Flush();
                        sw.Close();
                    }
                }
                catch
                {
                }
                #endregion
            }
        }

        #region д������
        /// <summary>
        /// д��������Ϣ(���Զ������־��Ϣ�еĻ��з�)
        /// <para>
        /// ÿ����Ϣд��һ��,����ָ���������������ڵ�����
        /// </para>
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Pulse(object msg)
        {
            Pulse(msg.ToString());
        }

        /// <summary>
        /// д��������Ϣ(���Զ������־��Ϣ�еĻ��з�)
        /// <para>
        /// ÿ����Ϣд��һ��,����ָ���������������ڵ�����
        /// </para>
        /// </summary>
        /// <param name="format">��ʽ�����ʽ</param>
        /// <param name="msgs">��ʽ�����ʽ�Ĳ���</param>
        public void Pulse(string format, params object[] msgs)
        {
            Write(LogLevel.Pulse, LogFileSpan.None, this.type, string.Format(format, msgs), true);
            currentPulseCount++;
        }
        #endregion

        #region д����Ϣ

        /// <summary>
        /// д����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Info(object msg)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, msg, true);
        }

        /// <summary>
        /// д����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(string format, params object[] msgs)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Info(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Info, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Info, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="msg">��־����</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        public void Info(bool compact, object msg)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        public void Info(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Info, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Info, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region д�������Ϣ

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Debug(object msg)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, msg, false);
        }

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(string format, params object[] msgs)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, string.Format(format, msgs), false);
        }


        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Debug(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Debug, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Debug, sequence, this.type, string.Format(format, msgs), false);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Debug(bool compact, object msg)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, msg, compact);

        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);

        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Debug(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Debug, sequence, this.type, msg, compact);

        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Debug, sequence, this.type, string.Format(format, msgs), compact);

        }
        #endregion

        #region д�뾯����Ϣ

        /// <summary>
        /// д�뾯����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Warn(object msg)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, msg, false);

        }

        /// <summary>
        /// д�뾯����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(string format, params object[] msgs)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, string.Format(format, msgs), false);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Warn(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Warn, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Warn, sequence, this.type, string.Format(format, msgs), false);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Warn(bool compact, object msg)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Warn(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Warn, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Warn, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region д�������Ϣ

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="msg">������Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Error(string msg, Exception ex)
        {
            Error(string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, msg, ex.Message, ex.StackTrace));
        }

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Error(object msg)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, msg, false);

        }

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(string format, params object[] msgs)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, string.Format(format, msgs), false);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Error(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Error, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Error, sequence, this.type, string.Format(format, msgs), false);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Error(bool compact, object msg)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Error(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Error, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Error, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region д��������Ϣ

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="msg">������Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Fatal(string msg, Exception ex)
        {
            Fatal(string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, msg, ex.Message, ex.StackTrace));
        }

        /// <summary>
        /// д������������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Fatal(object msg)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, msg, false);
        }

        /// <summary>
        /// д������������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, string.Format(format, msgs), false);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Fatal(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Fatal, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Fatal, sequence, this.type, string.Format(format, msgs), false);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Fatal(bool compact, object msg)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Fatal(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Fatal, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion
    }

    /// <summary>
    /// ��Ӧ��config�ļ��Ľڵ㣬����������־�Ļ�����Ϣ
    /// </summary>
    public sealed class LogSection : ConfigurationSection
    {
        /// <summary>
        /// ��־��Ŀ¼��·��
        /// </summary>
        [ConfigurationProperty("path", IsRequired = false, DefaultValue = "log")]
        internal string Path
        {
            get
            {
                return (string)base["path"];
            }
        }

        /// <summary>
        /// ��־��Ŀ¼�ļ���
        /// </summary>
        [ConfigurationProperty("level", IsRequired = true, DefaultValue = LogLevel.Error)]
        internal LogLevel Level
        {
            get
            {
                return (LogLevel)base["level"];
            }
        }
    }

    /// <summary>
    /// ��־�ļ����ɵ�Ƶ��
    /// </summary>
    public enum LogFileSpan
    {
        /// <summary>
        /// û�м��,��־��Ϣд�뵽һ���ļ���
        /// </summary>
        None = 0,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Year = 1,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Month = 2,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Week = 3,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Day = 4,

        /// <summary>
        /// Сʱ
        /// </summary>
        Hour = 5,

        /// <summary>
        /// ֻ��¼�������Ϣ(500��,����500���Ժ������ļ�,���¿�ʼ)
        /// </summary>
        Lateast = 6,
    }

    /// <summary>
    /// ��־����
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// δ����
        /// </summary>
        None,

        /// <summary>
        /// ������Ϣ
        /// </summary>
        Debug = 1,

        /// <summary>
        /// ������Ϣ
        /// </summary>
        Warn = 2,

        /// <summary>
        /// ������Ϣ
        /// </summary>
        Error = 3,

        /// <summary>
        /// ��������
        /// </summary>
        Fatal = 4,

        /// <summary>
        /// ��ͨ��Ϣ(���κ�����¶����¼)
        /// </summary>
        Info = 5,

        /// <summary>
        /// д��������Ϣ(��������������״̬)
        /// </summary>
        Pulse = 6
    }
}