using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Install;
using System.ServiceProcess;
namespace Lvl.App
{
    /// <summary>
    /// Windows服务的基类,通过使用这个类作为基类,就可以在安装服务时,指定服务的名称,而不使用在开发时默认的名称.这个基类通过在安装时的参数中指定服务名称,在安装时传递给安装程序.
    /// <para>
    /// 安装示例(不重写ServiceName): InstallUtil /ServiceName=MyService App.exe 
    /// </para>
    /// <para>
    /// 卸载示例(不重写ServiceName): InstallUtil /u /ServiceName=MyService App.exe
    /// </para>
    /// </summary>
    public class ServiceInstaller : Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller spi;
        private System.ServiceProcess.ServiceInstaller si;

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.spi = new System.ServiceProcess.ServiceProcessInstaller();
            this.si = new System.ServiceProcess.ServiceInstaller();

            this.spi.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.spi.Password = null;
            this.spi.Username = null;

            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.spi,
            this.si});

            this.BeforeInstall += new InstallEventHandler(ServiceInstaller_BeforeInstall);
            this.BeforeUninstall += new InstallEventHandler(ServiceInstaller_BeforeUninstall);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ServiceInstaller()
        {
            InitializeComponent();
        }

        void ServiceInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            this.si.ServiceName = ServiceName;
            this.si.DisplayName = ServiceName;

            this.spi.Account = this.Account;
            this.spi.Password = this.Password;
            this.spi.Username = this.Username;
        }

        void ServiceInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            this.spi.Account = this.Account;
            this.spi.Password = this.Password;
            this.spi.Username = this.Username;

            this.si.ServiceName = ServiceName;
            this.si.DisplayName = ServiceName;
            this.si.StartType = StartType;


        }

        /// <summary>
        /// 获取服务名称,使用可选择参数名称为ServiceName的参数来指定服务的名称,这个属性可以在子类中重写.
        /// <para>
        /// 例1,直接给出名称: return "MyService";
        /// </para>
        /// <para>
        /// 例2,使用配置文件:return ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["ServiceName"].Value;
        /// </para>
        /// <para>
        /// 例3,使用可选参数:return this.Context.Parameters["SrvName"];
        /// </para>
        /// </summary>
        public virtual string ServiceName
        {
            get
            {
                return this.Context.Parameters["ServiceName"];
            }
        }

        /// <summary>
        /// 运行这个服务的用户(默认:LocalSystem)
        /// </summary>
        public virtual ServiceAccount Account
        {
            get
            {
                return ServiceAccount.LocalSystem;
            }
        }

        /// <summary>
        /// 用户密码(默认:null)
        /// </summary>
        public virtual string Password
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 用户名(默认:null)
        /// </summary>
        public virtual string Username
        {
            get
            {
                return null;
            }

        }

        /// <summary>
        /// 服务的启动方式(默认:Automatic)
        /// </summary>
        public virtual ServiceStartMode StartType
        {
            get
            {
                return ServiceStartMode.Automatic;
            }
        }

    }
}