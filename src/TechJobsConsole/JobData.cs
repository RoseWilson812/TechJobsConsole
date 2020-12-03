﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Reflection;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;

        public static List<Dictionary<string, string>> FindAll()
        {
            LoadData();

            List<Dictionary<string, string>> AllJobsCopy = new List<Dictionary<string, string>>();

            foreach (var dict in AllJobs)
            {
                AllJobsCopy.Add(new Dictionary<string, string>(dict));
            }

            return AllJobsCopy;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[column];
                
                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }
            values.Sort();
            return values;
        }

        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();
            string valueLowerCase = value.ToLower();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[column]; //column = "location" or "employer" aValue = "Portland" or the value in the file
                string aValueLowerCase = aValue.ToLower();
                

                if (aValueLowerCase.Contains(valueLowerCase))
                {
                    jobs.Add(row);
                }
            }
            
            return jobs;
        }

        public static List<Dictionary<string, string>> FindByValue(List<string> allColumns, string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();
            string column = "";
            string valueLowerCase = value.ToLower();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                for (int i = 0; i < allColumns.Count; i++)
                {
                    column = allColumns[i];
                    string aValue = row[column];
                    string aValueLowerCase = aValue.ToLower();
                    
                    if (aValueLowerCase.Contains(valueLowerCase))
                    {
                        jobs.Add(row);
                        break;
                    }
                } 
            }

            return jobs;
        }
        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }
    }
}
