using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Parses CSV files
/// </summary>
public static class CSVParser
{
    private const string RegularExpression = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)"; // Regex for splitting CSV by commas but not including commas within quotes
    private static readonly Encoding Encoding = Encoding.GetEncoding("ISO-8859-1");
        
    /// <summary>
    /// Reads CSV file at given path and returns a matrix string array for each column and row
    /// </summary>
    public static List<string[]> ReadCSV(string path)
    {
        var columns = new List<string[]>();

        using var reader = new StreamReader(path, Encoding);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
                
            if (line == null)
                continue;
                
            var row = Regex.Split(line, RegularExpression);
                
            for (var i = 0; i < row.Length; i++)
            {
                if (row[i].Length <= 2) 
                    continue;
                    
                if (row[i].Substring(0, 3) == "\"\"\"") // If string starts with triple quotes, remove double quotes on either end
                {
                    row[i] = row[i].Substring(2, row[i].Length - 4);
                }
                else if (row[i][0] == '"') // If string starts with single quote, remove single quote on either end
                {
                    row[i] = row[i].Substring(1, row[i].Length - 2);
                }
            }
                
            columns.Add(row);
        }

        return columns;
    }
}