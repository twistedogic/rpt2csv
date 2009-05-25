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

								string[] columns = secondLine.Split(' ');
								List<string> columnNames = new List<string>();
								
								int charNo = 0;
								for (int i = 0; i < columns.Length; i++)
								{
									columnNames.Add(firstLine.Substring(charNo, Math.Min(columns[i].Length, firstLine.Length - charNo)).Trim());
									charNo += columns[i].Length + 1;
								}
								
								for (int i = 0; i < columnNames.Count; i++)
								{
									writer.Write(columnNames[i]);
									if (i < columnNames.Count - 1)
										writer.Write(",");
								}
								writer.WriteLine();
								int lineNo = 0;
								while (!reader.EndOfStream)
								{
									string line = reader.ReadLine();
									lineNo++;
									if (line.Trim().Length == 0)
										break;
									ProcessLine(line, reader, writer, columns);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				System.Console.Out.WriteLine("An error has occurred in processing this .rpt.  Exception info: " + e.ToString());
			}
		}

		private static void ProcessLine( string line, StreamReader reader, StreamWriter writer, string[] columns )
		{
			int charNo = 0;
			for (int i = 0; i < columns.Length; i++)
			{
				string str = string.Empty;
				bool success = false;
				while (!success)
				{
					try
					{
						str = line.Substring(charNo, Math.Min(columns[i].Length, line.Length - charNo)).Trim();
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
				if (i < columns.Length - 1)
					writer.Write(",");
				charNo += columns[i].Length + 1;
			}
			writer.WriteLine();
		}
	}


}
