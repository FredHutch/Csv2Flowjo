using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Csv2Flowjo
{
    class Program
    {
        const string parameterFileName = "parameterfile.txt";
        private const int ERROR_BAD_ARGUMENTS = 10;
        private const int ERROR_MISSING_PARAMETER_FILE = 20;
        private const int ERROR_NO_SAMPLE_FOLDERS = 30;

        static void Main(string[] args)
        {
            // Read in parameters from program start.
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("ERROR: No folder path argument provided.");
                Environment.Exit(ERROR_BAD_ARGUMENTS);
            }
            string path = args[0];

            // 1. Read parameters into parameters array
            string[,] parameters = ReadParameters(path);

            // 2. Create list of Sample subfolders
            IEnumerable<string> sampleFolders = Directory.EnumerateDirectories(path);
            if (sampleFolders == null)
            {
                Console.WriteLine($"ERROR: No sample subfolders found at {path}.");
                Environment.Exit(ERROR_NO_SAMPLE_FOLDERS);
            }

            // 3. Loop through each Sample subfolder and build sample array
            //    and write to output file with same root name as subfolder
            foreach (string sampleFolder in sampleFolders)
            {

            }
        }

        private static string[,] ReadParameters(string path)
        {
            // Read parameters from Parameterfile.txt located in path folder.
            string fullPath = path + parameterFileName;
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"ERROR: Missing {parameterFileName} file at {path}.");
                Environment.Exit(ERROR_MISSING_PARAMETER_FILE);
            }
            var items = File.ReadAllLines(fullPath);
            int numItems = items.Count();

            string[] _parameter = new string[2];
            string[,] _parameters = new string[numItems, 2];
            int i = 0;

            foreach(var item in items)
            {
                _parameter = item.Split(',');
                _parameters[i, 0] = _parameter[0];
                _parameters[i, 1] = _parameter[1];
                i++;
            }
            return _parameters;
        }

        //private static void AppendColumnToArray(StreamWriter outFile, string[] data)
        //{
        //    bool stop = false;
        //    string file = data[0];

        //    if (File.Exists(path + file))
        //    {
        //        while (!stop)
        //        {

        //        }
        //    }
        //    else
        //    {
        //        outFile
        //    }


        //}
    }
}
