using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;


public class Adler32Managed : HashAlgorithm
{
    private ushort o_sum_1;
    private ushort o_sum_2;

    public Adler32Managed()
    {
        Initialize();
    }

    public override int HashSize
    {
        get
        {
            return 32;
        }
    }

    public override void Initialize()
    {
        // reset the sum values
        o_sum_1 = 1;
        o_sum_2 = 0;
    }

    protected override void HashCore(byte[] p_array, int p_start_index,
    int p_count)
    {
        // process each byte in the array
        for (int i = p_start_index; i < p_count; i++)
        {
            o_sum_1 = (ushort)((o_sum_1 + p_array[i]) % 65521);
            o_sum_2 = (ushort)((o_sum_1 + o_sum_2) % 65521);
        }
    }

    protected override byte[] HashFinal()
    {
        // concat the two 16 bit values to form
        // one 32-bit value
        uint x_concat_value = (uint)((o_sum_2 << 16) | o_sum_1);
        // use the bitconverter class to render the
        // 32-bit integer into an array of bytes
        return BitConverter.GetBytes(x_concat_value);
    }
}

public class findDupes
{
    static void Main(string[] args)
    {
        CheckBeforeProceeding(args);

        DateTime now = DateTime.Now;
        Console.WriteLine(now);

        List<string> all = new List<string>();
        string path = args[0];

        if (Directory.Exists(path))
        {
            ProcessDirectory(path, ref all);
        }
        else
        {
            Console.WriteLine("{0} is not a valid file or directory.", path);
        }

        Console.WriteLine("Ok. Func ProcessDirectory is success");
        DateTime suck = DateTime.Now;
        Console.WriteLine(suck);

        string[] files = all.ToArray();
        List<string> filesums = new List<string>();

        foreach (string file in files)
            try
            {
                filesums.Add(GetFileSum(file));
            }
            catch (System.IO.IOException)
            {
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }

        Console.WriteLine("Ok. Func filesums is success");
        DateTime filesum = DateTime.Now;
        Console.WriteLine(filesum);

        List<string> dupes = SearchForDupes(filesums);

        Console.WriteLine("Ok. Func SearchForDupes is success");
        DateTime tSearchForDupes = DateTime.Now;
        Console.WriteLine(tSearchForDupes);

        PrintDupes(filesums, dupes, files);

        DateTime end = DateTime.Now;
        Console.WriteLine(end);

        Console.WriteLine("End");
        Console.ReadKey();
    }

    public static void ProcessDirectory(string targetDirectory, ref List<string> temp)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
            temp.Add(fileName);

        foreach (var directory in Directory.GetDirectories(targetDirectory))
        {
            try
            {
                ProcessDirectory(directory, ref temp);
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
        }
    }

    static void PrintDupes(List<string> sums, List<string> dupes, string[] files)
    {
        List<string> lines = new List<String>();
        foreach (string dupe in dupes)
        {

            lines.Add(dupe + "\n----------");

            for (int i = 0; i <= (files.Length - 1); i++)
                if (sums[i] == dupe)
                    lines.Add(files[i]);
        }
        File.WriteAllLines(@"C:\dupes.txt", lines);
    }

    static List<string> SearchForDupes(List<string> sums)
    {
        // Search for duplicate files within the given list of sums.
        // ArgumentOutOfRangeException
        List<string> dupes = new List<string>();

        for (int i = 0; i <= (sums.Count - 2); i++)
            for (int j = (i + 1); j <= (sums.Count - 2); j++)
                if (sums[i] == sums[j])
                    if (!dupes.Contains(sums[i]))
                        dupes.Add(sums[i]);

        return dupes;
    }

    static string GetFileSum(string file)
    {
        // Function scalped from http://stackoverflow.com/a/10520086/1433400
        // using (MD5 sum = MD5.Create())
        // using (FileStream stream = File.OpenRead(file))
        // return BitConverter.ToString(sum.ComputeHash(stream)).Replace("-", "").ToLower();
        using (HashAlgorithm x_hash_alg = new Adler32Managed())
        using (FileStream stream = File.OpenRead(file))
        {
            //return GetMd5Hash(md5Hash, stream);
            return BitConverter.ToString(x_hash_alg.ComputeHash(stream)).Replace("-", "").ToLower();
        }
    }

    static string GetMd5Hash(MD5 md5Hash, FileStream input)
    {

        byte[] data = md5Hash.ComputeHash(input);

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    static void CheckBeforeProceeding(string[] args)
    {
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
} 