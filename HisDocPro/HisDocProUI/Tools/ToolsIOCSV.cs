using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KozzionCore.IO.CSV;

namespace HisDocProUI.Tools
{
    public class ToolsIOCSV
    {
        public static string[,] ReadCSVFile(
            string file_path)
        {
            return ReadCSVFile(file_path, Delimiter.Comma, Delimiter.None);
        }

        public static string[,] ReadCSVFile(
            string file_path,
            Delimiter field_delimiter)
        {
            return ReadCSVFile(file_path, field_delimiter, Delimiter.None);
        }

        public static string[,] ReadCSVFile(
            string file_path,
            Delimiter field_delimiter,
            Delimiter text_sparator)
        {
            return ReadCSVLines(System.IO.File.ReadAllLines(file_path), field_delimiter, text_sparator);
        }

        public static string[,] ReadCSVStream(
            Stream input_stream)
        {
            return ReadCSVStream(input_stream, Delimiter.Comma, Delimiter.None);
        }

        public static string[,] ReadCSVStream(
            Stream input_stream,
                Delimiter field_delimiter)
        {
            return ReadCSVStream(input_stream, field_delimiter, Delimiter.None);
        }

        public static string[,] ReadCSVStream(
            Stream input_stream,
            Delimiter field_delimiter,
            Delimiter text_sparator)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(input_stream))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            return ReadCSVLines(lines.ToArray(), field_delimiter, text_sparator);
        }

        public static string[,] ReadCSVLines(
            List<string> lines)
        {
            return ReadCSVLines(lines.ToArray(), Delimiter.Comma, Delimiter.None);
        }

        public static string[,] ReadCSVLines(
            List<string> lines,
            Delimiter field_delimiter)
        {
            return ReadCSVLines(lines.ToArray(), field_delimiter, Delimiter.None);
        }

        public static string[,] ReadCSVLines(
            List<string> lines,
            Delimiter field_delimiter,
            Delimiter text_delimiter)
        {
            return ReadCSVLines(lines.ToArray(), field_delimiter, text_delimiter);
        }

        public static string[,] ReadCSVLines(
            string[] lines)
        {
            return ReadCSVLines(lines, Delimiter.Comma, Delimiter.None);
        }

        public static string[,] ReadCSVLines(
            string[] lines,
            Delimiter field_delimiter)
        {
            return ReadCSVLines(lines, field_delimiter, Delimiter.None);
        }


        public static string[,] ReadCSVLines(
            string[] lines,
            Delimiter field_separator,
            Delimiter text_sparator)
        {
            CheckDelimitors(field_separator, text_sparator);
            string field_sparator_string = GetDelimiterString(field_separator);
            if (text_sparator == Delimiter.None)
            {
                return ReadTable(lines, field_sparator_string);
            }
            else
            {
                string text_sparator_string = GetDelimiterString(text_sparator);
                return ReadTable(lines, field_sparator_string, text_sparator_string);
            }
        }

        public static string[,] ReadTable(string[] lines, string field_sparator)
        {
            string[] field_sparator_array = new string[] { field_sparator };
            List<string[]> splitlines = new List<string[]>();
            int maxsize = 0;
            for (int index = 0; index < lines.Length; index++)
            {
                if (0 < lines[index].Length)
                {
                    splitlines.Add(lines[index].Split(field_sparator_array, StringSplitOptions.None));
                    int size = splitlines[index].Length;
                    if (size > maxsize)
                    {
                        maxsize = size;
                    }
                }
            }
            return ConvertToTable(lines, splitlines, maxsize);
        }

        private static string[,] ConvertToTable(string[] lines, List<string[]> splitlines, int maxsize)
        {
            string[,] table = new string[splitlines.Count, maxsize];
            for (int index_row = 0; index_row < lines.Length; index_row++)
            {
                for (int index_column = 0; index_column < maxsize; index_column++)
                {
                    if (index_column < splitlines[index_row].Length)
                    {
                        table[index_row, index_column] = splitlines[index_row][index_column];
                    }
                    else
                    {
                        table[index_row, index_column] = "";
                    }
                }
            }
            return table;
        }



        private static string[,] ReadTable(string[] lines, string field_sparator, string text_sparator)
        {
            List<string[]> splitlines = new List<string[]>();

            int maxsize = 0;
            for (int index = 0; index < lines.Length; index++)
            {
                splitlines.Add(SplitLine(lines[index], field_sparator, text_sparator));
                int size = splitlines[index].Length;
                if (size > maxsize)
                {
                    maxsize = size;
                }
            }
            return ConvertToTable(lines, splitlines, maxsize);
        }

        private static string[] SplitLine(
            string line,
            string field_sparator,
            string text_sparator)
        {
            List<string> splitline = new List<string>();
            int index_current = 0;
            while (index_current < line.Length)
            {
                if (line.IndexOf(text_sparator, index_current) == index_current)
                {
                    //If this is a text separated string
                    index_current = index_current + 1;
                    int index_close = line.IndexOf(text_sparator, index_current);
                    if (index_close == -1)
                    {
                        throw new Exception("Ill-formed CSV: unclosed text separator at index: " + index_current);
                    }
                    else
                    {
                        splitline.Add(line.Substring(index_current, index_close - index_current));
                        index_current = index_close + 2;
                    }
                }
                else
                {
                    int index_close = line.IndexOf(field_sparator, index_current);
                    if (index_close == -1)
                    {
                        splitline.Add(line.Substring(index_current));
                        index_current = line.Length;
                    }
                    else
                    {
                        splitline.Add(line.Substring(index_current, index_close - index_current));
                        index_current = index_close + 1;
                    }
                }
            }
            return splitline.ToArray();
        }

        public static string GetDelimiterString(Delimiter delimiter)
        {
            switch (delimiter)
            {
                case Delimiter.Tab:
                    return new string(new char[] { (char)9 });
                case Delimiter.Space:
                    return " ";
                case Delimiter.SemiColon:
                    return ";";
                case Delimiter.DoubleQoute:
                    char[] doublequote = { (char)34 };
                    return new string(new char[] { (char)34 });
                case Delimiter.SingleQoute:
                    return "'";
                case Delimiter.Comma:
                    return ",";
                case Delimiter.None:
                    return "";
                default:
                    throw new Exception("unknown delimitor");

            }

        }

        private static void CheckDelimitors(Delimiter field_separator, Delimiter text_sparator)
        {
            if (field_separator == text_sparator)
            {
                throw new Exception("ERROR identical delimitors");
            }

            if (field_separator == Delimiter.None)
            {
                throw new Exception("ERROR No field delimitors");
            }
        }

        public static void WriteCSVFile(
            string file_path,
            string[,] table)
        {
            WriteCSVFile(file_path, table, Delimiter.Comma, Delimiter.None);
        }

        public static void WriteCSVFile(
            string file_path,
            string[,] table,
            Delimiter field_delimiter)
        {
            WriteCSVFile(file_path, table, field_delimiter, Delimiter.None);
        }

        public static void WriteCSVFile(
            string file_path,
            string[,] table,
            Delimiter field_delimiter,
            Delimiter text_sparator)
        {
            using (StreamWriter stream = new StreamWriter(new FileStream(file_path, FileMode.Create)))
            {
                WriteCSVStream(stream, table, field_delimiter, text_sparator);
                stream.Close();
            }
        }

        private static void WriteCSVStream(StreamWriter stream, string[,] table, Delimiter field_delimiter, Delimiter text_delimiter)
        {
            string field_delimiter_string = GetDelimiterString(field_delimiter);
            string text_delimiter_string = GetDelimiterString(text_delimiter);
            for (int index_0 = 0; index_0 < table.GetLength(0); index_0++)
            {
                StringBuilder builder = new StringBuilder(text_delimiter_string + table[index_0, 0] + text_delimiter_string);
                for (int index_1 = 1; index_1 < table.GetLength(1); index_1++)
                {
                    builder.Append(field_delimiter_string + text_delimiter_string + table[index_0, index_1] + text_delimiter_string);
                }
                stream.WriteLine(builder.ToString());
            }
        }
    }

}
