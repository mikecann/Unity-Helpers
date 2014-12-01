using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityHelpers
{

    public class CSVReader
    {
        /// <summary>
        /// Parses a given CSV text doc into a list of rows.
        /// Use: ParseCSV("")[5][6] // returns row 5 column 7
        /// </summary>
        /// <param name="csvText">The text to parse</param>
        /// <returns></returns>
        static public List<List<string>> ParseCSV(string csvText)
        {
            var lines = new List<List<string>>();

            foreach(var line in csvText.Split("\n"[0]))
            {
                lines.Add(SplitCsvLine(line).ToList());
            }

            return lines;
        }

        // splits a CSV row 
        static public string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }
    }
}
