using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using SeliwareAPI;
using System.Threading;

class Program
{
    private static string ExecuteDir = "./executable";
    
    static void Main(string[] args)
    {
        Seliware.Initialize();

        if (!Directory.Exists(ExecuteDir))
        {
            Directory.CreateDirectory(ExecuteDir);
        }

        foreach (string fl in Directory.GetFiles(ExecuteDir))
        {
            File.Delete(fl);
        }
        
        using (var watcher = new FileSystemWatcher(ExecuteDir))
        {
            watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

            watcher.Created += OnChanged;

            watcher.Filter = "*.lua";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        new ManualResetEventSlim(false).Wait();
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Created || e.ChangeType != WatcherChangeTypes.Renamed)
        {
            if (e.Name.StartsWith("InjectProcess")) // Inject
            {
                var ct = File.ReadAllText(e.FullPath);

                int pid;
                if (int.TryParse(ct, out pid))
                {
                    Inject(pid);
                }
                else
                {
                    Seliware.Inject();
                }
                
                File.Delete(e.FullPath);
            }
            else
            { // Execute
                Thread.Sleep(100);

                var source = File.ReadAllText(e.FullPath);

                Execute(source);

                Thread.Sleep(50);

                File.Delete(e.FullPath);
            }

            return;
        }
    }

    static void Inject(int pid)
    {
        // Inject 
        Seliware.Inject(pid);
    }

    static void Execute(string source)
    {
        // Execute
        Seliware.Execute(source);
    }
}
