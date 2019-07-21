using System;
using System.IO;
using System.Linq;

namespace Csv2Flowjo
{
    class Program
    {
        const string parameterFileName = "parameterfile.txt";
        private static string path;
        private static string[,] parameters;

        static void Main(string[] args)
        {
            // Read in parameters from program start.
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("ERROR: No folder path provided.");
                return;
            }
            path = args[0];
            string outFileName = args[1];
            ReadParameters(outFileName);
        }

        private static void ReadParameters(string outFileName)
        {
            // Read parameters from Parameterfile.txt located in path folder.
            var items = File.ReadAllLines(path + parameterFileName);
            int numItems = items.Count();

            var outFile = File.CreateText(path + outFileName);
            string[] itemData = new string[2];
            parameters = new string[numItems, 2];
            int i = 0;

            foreach(var item in items)
            {
                itemData = item.Split(',');
                parameters[i, 0] = itemData[0];
                parameters[i, 1] = itemData[1];
                i++;
            }
        }

        private static void AppendColumnToArray(StreamWriter outFile, string[] data)
        {
            bool stop = false;
            string file = data[0];

            if (File.Exists(path + file))
            {
                while (!stop)
                {

                }
            }
            else
            {
                outFile
            }


        }
    }
}
