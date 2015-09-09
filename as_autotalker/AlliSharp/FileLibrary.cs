using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AlliSharp
{
    public static class FileLibrary
    {
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {

            string[] searchPatterns = searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (string sp in searchPatterns)
                files.AddRange(Directory.GetFiles(path, sp, searchOption));
            files.Sort();
            return files.ToArray();

        }
    }

    public class FileLibrary<T> : ObservableCollection<T>, IDisposable
    {
        public string savePath;
        public string name;
        public event EventHandler DirectoryUpdate;
        private FileSystemWatcher monitor;

        public FileLibrary(string libraryName)
            : base()
        {

            this.name = libraryName;
            string saveDir = System.AppDomain.CurrentDomain.BaseDirectory + @"Librarys\";
            if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);
            this.savePath = Path.Combine(saveDir, name + ".alli");

        }

        #region Directory Montior

        private void DirectoryMonitor_OnChanged(object source, FileSystemEventArgs e)
        {
            OnDirectoryUpdate(new EventArgs());
        }

        private void DirectoryMonitor_OnRenamed(object source, RenamedEventArgs e)
        {
            OnDirectoryUpdate(new EventArgs());
        }

        public void InitializeDirectoryWatcher(string directory)
        {
            if (directory != null)
            {
                if (Directory.Exists(directory))
                {
                    monitor = new FileSystemWatcher(directory);
                    monitor.Changed += new FileSystemEventHandler(DirectoryMonitor_OnChanged);
                    monitor.Created += new FileSystemEventHandler(DirectoryMonitor_OnChanged);
                    monitor.Deleted += new FileSystemEventHandler(DirectoryMonitor_OnChanged);
                    monitor.Renamed += new RenamedEventHandler(DirectoryMonitor_OnRenamed);
                    monitor.EnableRaisingEvents = true;
                }
            }
        }

        public void ToggleDirectoryWatcher(bool on)
        {
            if (monitor != null)
            {
                monitor.EnableRaisingEvents = on;
            }
        }

        public bool IsDirectoryWatcherInitialized { get { return (monitor != null); } }

        public virtual void OnDirectoryUpdate(EventArgs e)
        {
            Console.WriteLine("Library - " + monitor.Path + " - Directory Changed");
            if (DirectoryUpdate != null)
            {
                DirectoryUpdate(this, e);
            }
        }

        #endregion

        #region Load/Save

        public void LoadLibrary()
        {
            if (!File.Exists(savePath)) return;
            Console.WriteLine("Library - Loading " + name);

            using (FileStream stream = new FileStream(savePath, FileMode.OpenOrCreate))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    List<T> list = formatter.Deserialize(stream) as List<T>;
                    foreach (T t in list)
                    {
                        this.Add(t);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Library - Error loading: " + e.ToString());
                }
            }
        }

        public void SaveLibrary()
        {
            Console.WriteLine("Library - Saving " + savePath);

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                List<T> list = new List<T>();

                foreach (T t in this)
                {
                    list.Add(t);
                }
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, list);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileLibrary()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources               
            }
            // free native resources if there are any.
        }

        #endregion
    }
}
