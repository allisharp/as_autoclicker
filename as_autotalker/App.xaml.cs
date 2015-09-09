using AlliSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace as_autotalker
{
    public partial class App : Application
    {
        static Mutex mutex = new Mutex(true, "{C7057838-B587-4CA5-9C90-9FACE821E422}");

        private MainWindow as_autotyper;
        public App()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.ToString());

            UnhandledErrorWindow uew = new UnhandledErrorWindow(e.Exception);
            if (uew.ShowError())
            {
                try
                {
                    as_autotyper.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Application.Current.Shutdown();
            }
            e.Handled = true;
        }

        private void SetAddRemoveProgramsIcon()
        {
            //only run if deployed 
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed
                 && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                try
                {
                    Assembly code = Assembly.GetExecutingAssembly();
                    AssemblyDescriptionAttribute asdescription =
                        (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(code, typeof(AssemblyDescriptionAttribute));
                    string assemblyDescription = asdescription.Description;

                    //the icon is included in this program
                    string iconSourcePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "faviconhuge.ico");

                    if (!File.Exists(iconSourcePath))
                        return;

                    RegistryKey myUninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                    string[] mySubKeyNames = myUninstallKey.GetSubKeyNames();
                    for (int i = 0; i < mySubKeyNames.Length; i++)
                    {
                        RegistryKey myKey = myUninstallKey.OpenSubKey(mySubKeyNames[i], true);
                        object myValue = myKey.GetValue("DisplayName");
                        if (myValue != null && myValue.ToString() == assemblyDescription)
                        {
                            myKey.SetValue("DisplayIcon", iconSourcePath);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log an error
                }
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                as_autotyper = new MainWindow();
                as_autotyper.Show();
                mutex.ReleaseMutex();
                SetAddRemoveProgramsIcon();
            }
            else
            {
                new AlliSharpMessageBox("AlliSharp.com", "Only one instance of the auto-talker is allowed to run at a time.");
            }
        }
    }
}
