using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Algorithm
{
    public class DuplicateFilesFinder
    {
        public static List<FileInfo> files = new List<FileInfo>();

        public static void ListDrive(string drive, bool enumerateFolders)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(drive);
                foreach (FileInfo fi in di.EnumerateFiles())
                {
                    files.Add(fi);
                }

                if (enumerateFolders)
                {
                    foreach (DirectoryInfo sdi in di.EnumerateDirectories())
                    {
                        ListDrive(sdi.FullName, enumerateFolders);
                    }
                }
            }

            catch (UnauthorizedAccessException) { }
        }

        public static void ListDuplicates()
        {
            var duplicatedFiles = files.GroupBy(x => new { x.Name, x.Length }).Where(t => t.Count() > 1).ToList();

            Console.WriteLine("Total items: {0}", files.Count);
            Console.WriteLine("Probably duplicates {0}", duplicatedFiles.Count());

            StreamWriter duplicatesFoundLog = new StreamWriter(@"C:\DuplicatedFileList.txt");

            foreach (var filter in duplicatedFiles)
            {
                duplicatesFoundLog.WriteLine("Probably duplicated item: Name: {0}, Length: {1}",
                    filter.Key.Name,
                    filter.Key.Length);

                var items = files.Where(x => x.Name == filter.Key.Name &&
                   x.Length == filter.Key.Length).ToList();

                int c = 1;
                foreach (var suspected in items)
                {
                    try
                    {
                        duplicatesFoundLog.WriteLine("{3}, {0} - {1}, Creation date {2}",
                            suspected.Name,
                            suspected.FullName,
                            suspected.CreationTime,
                            c);
                        c++;
                    }
                    catch(PathTooLongException) { continue; }
                }

                duplicatesFoundLog.WriteLine();
            }

            duplicatesFoundLog.Flush();
            duplicatesFoundLog.Close();
        }

    }
}

public class findDupes
{
    static void Main(string[] args)
    {
        var watch = Stopwatch.StartNew();

        Algorithm.DuplicateFilesFinder.ListDrive(args[0], true);
        Algorithm.DuplicateFilesFinder.ListDuplicates();

        watch.Stop();
        var elapsedMS = watch.ElapsedMilliseconds / 1000;
        
        Console.WriteLine($"Elapsed time: {elapsedMS}\nPress any key to exit from program...");
        Console.ReadKey();
    }
}