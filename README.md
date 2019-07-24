# Csv2Flowjo
Takes a set of experimental sample files in folders and prepares for use with Flowjo.

# Usage
 ## Command line parameters (delimited with a space):
  1. Path to parent folder containing parameter file and sample subfolders. REQUIRED.
  2. Name of parameter file. OPTIONAL. If not provided, defaults to 'parameterfile.txt'.
  3. Maximum number or rows in a sample file. OPTIONAL. If not provided, defaults to 10000.

 ## The parameter file contains a row for each file with each row containing 3 items delimited with commas:
  1. Name of csv input file (do not include '.csv' since it is assumed).
  2. Name of column in the csv input file. This is the name of the column in row 4 of the file.
  3. Name of the column to use in the output file.
