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
    }
}
