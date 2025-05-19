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
        Console.WriteLine("Initialize Seliware");

        Seliware.Initialize();

        if (!Directory.Exists(ExecuteDir))
        {
            Directory.CreateDirectory(ExecuteDir);
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

            watcher.Filter = "*.zovware";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            watcher.Created += OnChanged;

            new ManualResetEventSlim(false).Wait();
        }

        new ManualResetEventSlim(false).Wait();
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine(e.FullPath);

        if (e.ChangeType != WatcherChangeTypes.Created || e.ChangeType != WatcherChangeTypes.Renamed)
        {
            if (e.Name.StartsWith("InjectProcess")) // Inject
            {
                Inject();

                File.Delete(e.FullPath);
            } else { // Execute
                Thread.Sleep(100);

                var source = File.ReadAllText(e.FullPath);

                Execute(source);

                Thread.Sleep(50);

                File.Delete(e.FullPath);
            }

            return;
        }
    }

    static void Inject()
    {
        // Inject 
        Seliware.Inject();
    }

    static void Execute(string source)
    {
        // Execute
        Seliware.Execute(source);
    }
}
