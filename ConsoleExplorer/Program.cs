using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo rootDirectory = null;
            Console.WriteLine(@"Enter the path:");
            string path = Console.ReadLine();

            while (true)
            {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        if (rootDirectory != null)
                        {
                            string subPath = Path.Combine(rootDirectory.FullName, path);
                            if (!Directory.Exists(subPath))
                            {
                                Console.WriteLine("Path {0} is undefined. Try again", path);
                                path = Console.ReadLine();
                                continue;
                            }
                            else
                            {
                                path = subPath;
                            }
                        }
                    }
                    if(!Directory.Exists(path))
                    {
                        Console.WriteLine("Path {0} is undefined. Try again", path);
                        continue;
                    }
                    
                    rootDirectory = new DirectoryInfo(path);

                    List<DirectoryInfo> subDirectory = rootDirectory.GetDirectories().OrderBy(_ => _.Name).ToList();
                    List<FileInfo> dirFiles = rootDirectory.GetFiles().OrderBy(_ => _.Name).ToList();

                    Console.WriteLine("Current folder: {0}", path);

                    foreach (var directory in subDirectory)
                    {
                        long directorySizeInBytes = GetDirectorySize(directory.FullName);
                        string formattedSize = BytesToString(directorySizeInBytes);
                        Console.WriteLine("Directory: {0} SIZE: {1}", directory.Name, formattedSize);
                    }

                    foreach (var file in dirFiles)
                    {
                        long fileSizeInBytes = GetFileSize(file.FullName);
                        string formattedSize = BytesToString(fileSizeInBytes);

                        Console.WriteLine("File: {0} SIZE: {1}", file.Name, formattedSize);
                    }

                    Console.WriteLine("Press 'E' for exit or enter the path");

                    string input = Console.ReadLine();
                    if (input.ToLower() == "e")
                    {
                        break;
                    }
                    path = input;
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex);
                    Console.ReadLine();
                }
            }
        }

        private static long GetFileSize(string path)
        {
            try
            {
                long sizeInBytes = 0;

                FileInfo fileInfo = new FileInfo(path);
                sizeInBytes = fileInfo.Length;

                return sizeInBytes;
            }
            catch (UnauthorizedAccessException ex)
            {
                return -1;
            }
        }

        private static long GetDirectorySize(string path)
        {
            try
            {
                long sizeInBytes = 0;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (var fileInfo in files)
                {
                    sizeInBytes += fileInfo.Length;
                }

                return sizeInBytes;

            }
            catch (UnauthorizedAccessException ex)
            {
                return -1;
            }
        }

        private static string BytesToString(long byteCount)
        {
            if(byteCount < 0)
            {
                return "Access denied";
            }

            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
