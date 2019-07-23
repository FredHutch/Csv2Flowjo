using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Csv2Flowjo
{
    class Program
    {
        const string parameterFileName = "parameterfile.txt";
        const int maxSampleRows = 200;
        private const int ERROR_BAD_ARGUMENTS = 10;
        private const int ERROR_MISSING_PARAMETER_FILE = 20;
        private const int ERROR_NO_SAMPLE_FOLDERS = 30;
        private const int ERROR_FILE_DOES_NOT_EXIST = 40;

        private static int sampleElementCount;

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
            sampleElementCount = parameters.GetLength(0);

            //Now create an array
            string[,] output = new string[sampleElementCount, maxSampleRows];

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
                output.Initialize();
                ProcessSampleFolder(sampleFolder, parameters, ref output);
            }
        }

        private static void ProcessSampleFolder(string sampleFolder, string[,] parameters, ref string[,] output)
        {
            string file;
            string column;
           
            for (int i = 0; i < sampleElementCount; i++)
            {
                file = parameters[i, 0];
                column = parameters[i, 1];

                if (File.Exists(file))
                {
                    ProcessSampleFile(file, column, i, ref output);
                    List<string> outputList = ConvertArrayToList(output);
                    ExportOutput(sampleFolder + file, outputList);
                }
                else
                {
                    Console.WriteLine($"ERROR: The file {file} located in {sampleFolder} was not found.");
                    Environment.Exit(ERROR_FILE_DOES_NOT_EXIST);
                }
            }
        }

        private static List<string> ConvertArrayToList(string[,] output)
        {
            List<string> outputFile = new List<string>();
            string line = string.Empty;

            for (int i = 0; i < output.GetLength(1); i++)
            {
                for (int j = 0; j < output.GetLength(0); j++)
                {
                    line += $"{output[i, j]}, "; 
                }
                if (line.Length > 0)
                {
                    line = line.Substring(0, line.Length - 2);
                }
                outputFile.Add(line);
            }
            return outputFile;
        }

        private static void ProcessSampleFile(string fileName, string column, int colNum, ref string[,] output)
        {
            //Open File for reading
            string[] records = File.ReadAllLines(fileName);

            // Get Header
            string[] header = records[3].Split(",");
            string[] line;
            int index = Array.IndexOf(header, column);

            // Header column -> output 
            output[0, colNum] = column;

            // Iterate through file, grab the
            // column and place into output array
            for (int i = 4; i < maxSampleRows; i++)
            {
                if (string.IsNullOrWhiteSpace(records[i]))
                {
                    break;
                }
                line = records[i].Split(",");
                output[i, i - 3] = line[index];
            }
        }

        private static void ExportOutput(string fullPath, List<string> outputList)
        {
            File.WriteAllLines(fullPath + ".csv", outputList);
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
    }
}
