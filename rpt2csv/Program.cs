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
			if (args.Length != 2)
			{
				System.Console.Out.WriteLine("Usage: rpt2csv [inputfile] [outputfile]");
				return;
			}
			try
			{
				using (FileStream fs = File.OpenRead(args[0]))
				{
					using (StreamReader reader = new StreamReader(fs, Encoding.Default))
					{
						using (FileStream outfs = File.OpenWrite(args[1]))
						{
							using (StreamWriter writer = new StreamWriter(outfs))
							{
								string firstLine = reader.ReadLine();
								string secondLine = reader.ReadLine();

								string[] columns = secondLine.Split(' ');
								List<string> columnNames = new List<string>();
								int charNo = 0;
								int minLineLength = 0;
								for (int i = 0; i < columns.Length; i++)
								{
									columnNames.Add(firstLine.Substring(charNo, Math.Min(columns[i].Length, firstLine.Length - charNo)).Trim());
									charNo += columns[i].Length + 1;
								}
								minLineLength = charNo - columns[columns.Length - 1].Length + 2;
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
									charNo = 0;
									if (line.Trim().Length == 0)
										break;

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
										writer.Write(str);
										if (i < columns.Length - 1)
											writer.Write(",");
										charNo += columns[i].Length + 1;
									}
									writer.WriteLine();
								}
							}
						}
					}
				}
			}
			catch(Exception e)
			{
				System.Console.Out.WriteLine("An error has occurred in processing this .rpt.  Exception info: " + e.ToString());
			}
			System.Console.Out.WriteLine("Success!");
		}
	}
}
