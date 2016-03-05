using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class findDupes
{
    static void Main(string[] args)
    {
        CheckBeforeProceeding(args);

        string[] files = Directory.GetFiles(args[0]);
        List<string> filesums = new List<string>();

        foreach (string file in files)
            filesums.Add(GetFileSum(file));

        List<string> dupes = SearchForDupes(filesums);
        PrintDupes(filesums, dupes, files);
    }

    static void PrintDupes(List<string> sums, List<string> dupes, string[] files)
    {
        // Print output.
        foreach (string dupe in dupes)
        {
            Console.WriteLine("{0}\n----------", dupe);

            for (int i = 0; i <= (files.Length - 1); i++)
                if (sums[i] == dupe)
                    Console.WriteLine(files[i]);

            Console.WriteLine();
        }
    }

    static List<string> SearchForDupes(List<string> sums)
    {
        // Search for duplicate files within the given list of sums.
        List<string> dupes = new List<string>();

        for (int i = 0; i <= (sums.Count - 2); i++)
            for (int j = (i + 1); j <= (sums.Count - 2); j++)
                if (sums[i] == sums[j])
                    if (!dupes.Contains(sums[i]))
                        dupes.Add(sums[i]);

        return dupes;
    }

    static void CheckBeforeProceeding(string[] args)
    {
        // Check things are good with the target dir before proceeding.
        if (args.Length == 0)
        {
            Console.WriteLine("Error: No directory provided");
            Environment.Exit(1);
        }

        if (!Directory.Exists(args[0]))
        {
            Console.WriteLine("Error: '{0}' is not a valid directory", args[0]);
            Environment.Exit(2);
        }

        if (Directory.GetFiles(args[0]).Length == 0)
        {
            Console.WriteLine("Error: '{0}' does not contain any files", args[0]);
            Environment.Exit(3);
        }

        if (Directory.GetFiles(args[0]).Length == 1)
        {
            Console.WriteLine("Error: '{0}' only contains 1 file", args[0]);
            Environment.Exit(3);
        }
    }

    static string GetFileSum(string file)
    {
        // Function scalped from http://stackoverflow.com/a/10520086/1433400
        using (var sum = MD5.Create())
        using (var stream = File.OpenRead(file))
            return BitConverter.ToString(sum.ComputeHash(stream)).Replace("-", "").ToLower();
    }
}
