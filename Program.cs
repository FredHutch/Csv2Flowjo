using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Csv2Flowjo
{
    class Program
    {
        private const int ERROR_HELP = 10;
        private const int ERROR_MISSING_PARAMETER_FILE = 20;
        private const int ERROR_NO_SAMPLE_FOLDERS = 30;
        private const int ERROR_FILE_DOES_NOT_EXIST = 40;
        private const int ERROR_COLUMN_NOT_FOUND = 50;
        private const int ERROR_BAD_PARAMETER = 60;
        private const int ERROR_MAX_RECORD_COUNT_EXCEEDED = 60;

        private static int maxSampleRows = 10000;
        private static string parameterFileName = "parameterfile.txt";
        private static int sampleElementCount;
        private static string path;
        private static int recordCount;

        static void Main(string[] args)
        {
            int maxTemp;

            // Read in parameters from program start.
            if (args.Length < 1 || (args.Length == 1 && args[0].ToLower() == "help"))
            {
                Console.WriteLine("**************");
                Console.WriteLine("* Csv2Flowjo *");
                Console.WriteLine("**************");
                Console.WriteLine("Takes a set of experimental sample files in folders and prepares for use with Flowjo.");
                Console.WriteLine("Commmand line parameters (delimited with a space):");
                Console.WriteLine("  1. Path to parent folder containing parameter file and sample subfolders. REQUIRED.");
                Console.WriteLine("  2. Name of parameter file. OPTIONAL. If not provided, defaults to 'parameterfile.txt'.");
                Console.WriteLine("  3. Maximum number or rows in a sample file. OPTIONAL. If not provided, defaults to 10000.");
                Console.WriteLine("");
                Console.WriteLine("The parameter file contains a row for each file with each row containing 3 items delimited with commas:");
                Console.WriteLine("  1. Name of csv input file (do not include '.csv' since it is assumed).");
                Console.WriteLine("  2. Name of column in the csv input file. This is the name of the column in row 4 of the file");
                Console.WriteLine("  3. Name of the column to use in the output file.");
                Environment.Exit(ERROR_HELP);
            }
            if (args.Length >= 2 && !string.IsNullOrWhiteSpace(args[1]))
            {
                parameterFileName = args[1];
            }
            if (args.Length == 3 && int.TryParse(args[2], out maxTemp))
            {
                maxSampleRows = maxTemp;
            }

            path = args[0];
            Console.WriteLine($"Processing files for the experiment at {path}.");

            // 1. Read parameters into parameters array
            string[,] parameters = ReadParameters(path);
            sampleElementCount = parameters.GetLength(0);

            //Now create the output array
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
            int i = 0;
            foreach (string sampleFolder in sampleFolders)
            {
                Console.WriteLine($"Processing files found in {sampleFolder}.");
                Array.Clear(output, 0, sampleElementCount*maxSampleRows);
                ProcessSampleFolder(sampleFolder, parameters, ref output);
                i++;
            }
            Console.WriteLine($"Processed {i} sample folders for the experiment.");
        }
        private static void ProcessSampleFolder(string sampleFolder, string[,] parameters, ref string[,] output)
        {
            string file;
            string inputColumn;
            string outputColumn;
            string completeFilePath;
           
            for (int i = 0; i < sampleElementCount; i++)
            {
                file = parameters[i, 0];
                inputColumn = parameters[i, 1].Trim();
                outputColumn = parameters[i, 2].Trim();
                // Use Path.Combine instead of 
                //completeFilePath = sampleFolder + "\\" + file + ".csv";
                completeFilePath = Path.Combine(sampleFolder, file) + ".csv";

                if (File.Exists(completeFilePath))
                {
                    ProcessSampleFile(completeFilePath, inputColumn, i, outputColumn, ref output);
                }
                else
                {
                    Console.WriteLine($"ERROR: The file {file} located in {sampleFolder} was not found.");
                    Environment.Exit(ERROR_FILE_DOES_NOT_EXIST);
                }
            }

            List<string> outputList = ConvertArrayToList(output);
            ExportOutput(sampleFolder, outputList);
        }

        private static List<string> ConvertArrayToList(string[,] output)
        {
            List<string> outputFile = new List<string>();
            string line = string.Empty;

            for (int i = 0; i < output.GetLength(1); i++)
            {
                // Ignore extra empty rows in array
                if (i < recordCount - 3)
                { 
                    for (int j = 0; j < output.GetLength(0); j++)
                    {
                        line += $"{output[j, i]}, "; 
                    }
                    if (line.Length > 0)
                    {
                        line = line.Substring(0, line.Length - 2);
                    }
                    outputFile.Add(line);
                    line = string.Empty;
                }
            }
            return outputFile;
        }

        private static void ProcessSampleFile(string fileName, string inputColumn, int colNum, string outputColumn, ref string[,] output)
        {
            //Open File for reading
            string[] records = File.ReadAllLines(fileName);
            recordCount = records.Count();

            if (recordCount > maxSampleRows)
            {
                Console.WriteLine($"ERROR: The number of rows in {fileName} exceeds the specified max value of {maxSampleRows}.");
                Environment.Exit(ERROR_MAX_RECORD_COUNT_EXCEEDED);
            }

            // Get Header
            string[] header = records[3].Split(",");
            string[] line;
            int index = Array.IndexOf(header, inputColumn);
            if (index < 0)
            {
                Console.WriteLine($"ERROR: No column named {inputColumn} was found in file {fileName}.");
                Environment.Exit(ERROR_COLUMN_NOT_FOUND);
            }

            // Header column -> output 
            output[colNum, 0] = outputColumn;

            // Iterate through file, grab the
            // column and place into output array
            for (int i = 4; i < recordCount; i++)
            {
                if (string.IsNullOrWhiteSpace(records[i]))
                {
                    break;
                }
                line = records[i].Split(",");
                output[colNum, i - 3] = line[index];
            }
        }

        private static void ExportOutput(string fullPath, List<string> outputList)
        {
            File.WriteAllLines(fullPath + ".csv", outputList);
        }

        private static string[,] ReadParameters(string path)
        {
            // Read parameters from Parameterfile.txt located in path folder.
            string fullPath = Path.Combine(path,parameterFileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"ERROR: Missing {parameterFileName} file at {path}. Note that this error could be caused by a bad path.");
                Environment.Exit(ERROR_MISSING_PARAMETER_FILE);
            }
            var items = File.ReadAllLines(fullPath);
            int numItems = 0;
            foreach(var item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                else if(item.ToString().Count(x => x == ',') == 2)
                {
                    numItems++;
                }
                else
                {
                    Console.WriteLine($"ERROR: {parameterFileName} contains incomplete parameter in row {numItems+1}.");
                    Environment.Exit(ERROR_BAD_PARAMETER);
                }
            }

            string[] _parameter = new string[3];
            string[,] _parameters = new string[numItems, 3];
            int i = 0;

            foreach(var item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    break;
                }
                _parameter = item.Split(',');
                _parameters[i, 0] = _parameter[0];
                _parameters[i, 1] = _parameter[1];
                _parameters[i, 2] = _parameter[2];
                i++;
            }
            return _parameters;
        }
    }
}
