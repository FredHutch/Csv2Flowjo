using System;
using System.IO;
using System.Linq;

namespace Csv2Flowjo
{
    class Program
    {
        const string parameterFileName = "parameterfile.txt";
        private static string path;
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
            var outFile = File.CreateText(path + outFileName);
            string[] data = new string[1];

            foreach(var item in items)
            {
                data = item.Split(',');
                AppendColumnToFile(outFile, data);
            }

        }

        private static void AppendColumnToFile(StreamWriter outFile, string[] data)
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
