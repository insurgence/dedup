using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace dedup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var allFiles = Program.GetAllFiles(@"C:\tos", "*.*");
            Directory.GetFiles(@"C:\tos", "*.*")
                .Select(
                    f => new
                    {
                        FileName = f,
                        FileHash = Encoding.UTF8.GetString(new SHA1Managed()
                                                                    .ComputeHash(new FileStream(f,
                                                                                     FileMode.Open,
                                                                                     FileAccess.Read)))
                    })
                .GroupBy(f => f.FileHash)
                .Select(g => new { FileHash = g.Key, Files = g.Select(z => z.FileName).ToList() })
                .SelectMany(f => f.Files.Skip(1))
                .ToList()
                .ForEach(File.Delete);
            Console.WriteLine("End...");
            Console.ReadKey();
        }

        public static IEnumerable<String> GetAllFiles(string path, string searchPattern)
        {
            return System.IO.Directory.EnumerateFiles(path, searchPattern).Union(
                System.IO.Directory.EnumerateDirectories(path).SelectMany(d =>
                {
                    try
                    {
                        return GetAllFiles(d, searchPattern);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        return Enumerable.Empty<String>();
                    }
                }));
        }

    }
}
