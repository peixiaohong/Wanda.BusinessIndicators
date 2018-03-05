using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Security.Principal;
using System.Configuration;

namespace Lib.Web
{


    /// <summary>
    /// 
    /// </summary>
    public class FileConnect : IDisposable
    {

        #region win32 API
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername,
                                             string lpszDomain,
                                             string lpszPassword,
                                             int dwLogonType,
                                             int dwLogonProvider,
                                             ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Duplicates the token.
        /// </summary>
        /// <param name="existingTokenHandle">The existing token handle.</param>
        /// <param name="SECURITY_IMPERSONATION_LEVEL">The SECURIT y_ IMPERSONATIO n_ LEVEL.</param>
        /// <param name="duplicateTokenHandle">The duplicate token handle.</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr existingTokenHandle,
                                                 int SECURITY_IMPERSONATION_LEVEL,
                                                 ref IntPtr duplicateTokenHandle);


        // logon types
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        // logon providers
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT50 = 3;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT35 = 1;

        WindowsIdentity newIdentity;
        WindowsImpersonationContext impersonatedUser;
        bool isSuccess = false;
        IntPtr token = IntPtr.Zero;

        /// <summary>
        /// Gets a value indicating whether this instance is connectted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is connectted; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnectted
        {
            get { return isSuccess; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConnect"/> class.
        /// </summary>
        public FileConnect()
        {
            string fileServer = ConfigurationManager.AppSettings["FileServer"];

            if (!string.IsNullOrEmpty(fileServer))
            {
                isSuccess = Connect(fileServer, ConfigurationManager.AppSettings["FileServerUserName"], ConfigurationManager.AppSettings["FileServerPassword"]);
            }
            else
            {
                isSuccess = true;
            }
        }

        /// <summary>
        /// Connects the specified remote addr.
        /// </summary>
        /// <param name="remoteAddr">The remote addr.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool Connect(string remoteAddr, string userName, string password)
        {

            bool isSuccess = LogonUser(userName,
                                              remoteAddr,
                                              password,
                                              LOGON32_LOGON_NEW_CREDENTIALS,
                                              LOGON32_PROVIDER_DEFAULT, ref token);

            newIdentity = new WindowsIdentity(token);
            impersonatedUser = newIdentity.Impersonate();
            return true;
        }

        /// <summary>
        /// Dises the connect.
        /// </summary>
        public void DisConnect()
        {
            if (isSuccess)
            {
                if (impersonatedUser != null)
                    impersonatedUser.Undo();
                if (token != IntPtr.Zero)
                    CloseHandle(token);
            }
        }



        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            DisConnect();
        }

        #endregion

    }
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Reads the file to string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string ReadFileToString(string path)
        {
            byte[] buffer = ReadFile(path);

            if (buffer == null) return string.Empty;

            return Encoding.UTF8.GetString(buffer);
        }
        /// <summary>
        /// 
        /// </summary>
        public static class SequentialGuid
        {
            [DllImport("rpcrt4.dll", SetLastError = true)]
            private static extern int UuidCreateSequential(out Guid guid);

            /// <summary>
            /// News the GUID.
            /// </summary>
            /// <returns></returns>
            public static Guid NewGuid()
            {
                const int RPC_S_OK = 0;

                Guid guid;
                int result = UuidCreateSequential(out guid);
                if (result != RPC_S_OK)
                {
                    throw new ApplicationException("Create sequential guid failed: " + result);
                }

                return guid;
            }
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static byte[] ReadFile(string path)
        {
            byte[] result = null;
            if (!File.Exists(path))
                return result;

            try
            {
                using (FileStream fileStream = File.OpenRead(path))
                {
                    result = new byte[fileStream.Length];
                    fileStream.Read(result, 0, result.Length);
                    fileStream.Close();
                }
            }
            catch
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        public static void WriteFile(string path, string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            WriteFile(path, buffer);
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        public static void WriteFile(string path, byte[] data)
        {
            try
            {
                string dirPath = path.Substring(0, path.LastIndexOf("\\"));

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }


                using (FileStream fileStream = File.Create(path))
                {
                    fileStream.Write(data, 0, data.Length);
                    fileStream.Close();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path"></param>
        public static void CreateFolder(string path)
        {
            string dirPath = path.Substring(0, path.LastIndexOf("\\"));

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

        }


    }
}
