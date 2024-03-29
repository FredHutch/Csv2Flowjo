# Csv2Flowjo
Takes a set of experimental sample files in folders and prepares for use with Flowjo.

# Souce Code
* Csv2Flowjo is written in C# on the .NET Core platform. 
* Built on .NET Core version 2.2.
* Requires an install of Dotnet Core.

# Usage

 ## Command line parameters (delimited with a space):
  1. Path to parent folder containing parameter file and sample subfolders. REQUIRED.
  2. Name of parameter file. OPTIONAL. If not provided, defaults to 'parameterfile.txt'.
  3. Maximum number or rows in a sample file. OPTIONAL. If not provided, defaults to 10000.

 ## The parameter file contains a row for each file with each row containing 3 items delimited with commas:
  1. Name of csv input file (do not include '.csv' since it is assumed).
  2. Name of column in the csv input file. This is the name of the column in row 4 of the file.
  3. Name of the column to use in the output file.

 ## How to execute:
 ```dotnet csv2flowjo.dll <parameters>```

 ### Examples:
 * Run Csv2Flowjo for experiment found in /experiment folder.
 
 ```dotnet csv2flowjo.dll "/experiment"```
 * Run Csv2Flowjo for experiment found in /newexp folder using config.txt parameters file. Note: that path syntax may be dependent on the operating system.

 ```dotnet csv2flowjo.dll "../../newexp" config.txt```
 * Run Csv2Flowjo for experiment found in /newexp folder (located two folders up) using config.txt parameters file and increasing the max number of lines in any input file from default of 10000 to 50000

 ```dotnet csv2flowjo.dll "/newexp" config.txt 50000```
 
 * Get help on parameters
 
 ```dotnet csv2flowjo.dll``` -or- ```dotnet csv2flowjo.dll help```
