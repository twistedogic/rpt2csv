using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace rpt2csv
{
	class Program
	{
		static void Main( string[] args )
		{
			if (args.Length < 1 || args.Length > 2)
			{
				System.Console.Out.WriteLine("Usage: rpt2csv [inputfile] [outputfile]");
				return;
			}
			string inputPath = args[0];
			string outputPath = String.Empty;
			if (args.Length > 1)
				outputPath = args[1];
			else
				outputPath = inputPath.Substring(0, inputPath.Length - 3) + ".csv";

			ProcessRPT(inputPath, outputPath);

			System.Console.Out.WriteLine("Success!");
		}

		private static void ProcessRPT(string inputPath, string outputPath)
		{
			try
			{
				using (FileStream fs = File.OpenRead(inputPath))
				{
					using (StreamReader reader = new StreamReader(fs, Encoding.Default))
					{
						using (FileStream outfs = File.OpenWrite(outputPath))
						{
							using (StreamWriter writer = new StreamWriter(outfs))
							{
								// The first line contains the column names; the second line is used to determine column length
								string firstLine = reader.ReadLine();
								string secondLine = reader.ReadLine();

								int[] columnLengths = secondLine.Split(' ').Select(s => s.Length).ToArray();
								List<string> columnNames = new List<string>();

								int charNo = 0;
								foreach (int columnLength in columnLengths)
								{
									columnNames.Add(firstLine.Substring(charNo, Math.Min(columnLength, firstLine.Length - charNo)).Trim());
									charNo += columnLength + 1;
								}

								WriteColumnNames(writer, columnNames);

								while (!reader.EndOfStream)
								{
									string line = reader.ReadLine();
									if (line.Trim().Length == 0)
										break;
									ProcessLine(line, reader, writer, columnLengths);
								}
							}
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
				System.Console.Out.WriteLine(inputPath + " not found!");
			}
			catch (UnauthorizedAccessException)
			{
				System.Console.Out.WriteLine("You are not authorized to access this file.");
			}
			catch (ArgumentException)
			{
				System.Console.Out.WriteLine("Invalid filename.");
			}
			catch (Exception e)
			{
				System.Console.Out.WriteLine("An error has occurred in processing this .rpt.  Exception info: " + e.ToString());
			}
		}

		private static void WriteColumnNames(StreamWriter writer, List<string> columnNames)
		{
			for (int i = 0; i < columnNames.Count; i++)
			{
				writer.Write(columnNames[i]);
				if (i < columnNames.Count - 1)
					writer.Write(",");
			}
			writer.WriteLine();
		}

		private static void ProcessLine( string line, StreamReader reader, StreamWriter writer, int[] columnLengths )
		{
			int charNo = 0;
			for (int i = 0; i < columnLengths.Length; i++)
			{
				string str = string.Empty;
				bool success = false;
				while (!success)
				{
					try
					{
						str = line.Substring(charNo, Math.Min(columnLengths[i], line.Length - charNo)).Trim();
						success = true;
					}
					catch
					{
						line = line + reader.ReadLine();
					}
				}

				str = str.Replace("\"", "\"\"");
				str = '"' + str + '"';
				writer.Write(str);
				if (i < columnLengths.Length - 1)
					writer.Write(",");
				charNo += columnLengths[i] + 1;
			}
			writer.WriteLine();
		}
	}
}
