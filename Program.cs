using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShingoTree
{
	class Program
	{
		static void Main(string[] args)
		{
			if(args.Length == 0)
			{
				PrintHelp();
				return;
			}

			List<string> pathes = new List<string>();
			Runner runner = BuildRunner(args, pathes);

			if (pathes.Count == 0)
				pathes.Add(Environment.CurrentDirectory);

			runner.Lookup(pathes);
		}

		private static void PrintHelp()
		{
			Console.WriteLine("以图形显示驱动器或路径的文件夹结构。");
			Console.WriteLine();
			Console.WriteLine("SHINGOTREE [/F] [/O output [/V]] [path, ...]");
			Console.WriteLine();
			Console.WriteLine("   /F   显示每个文件夹中文件的名称。");
			Console.WriteLine("   /O   将结果输出到文件。");
			Console.WriteLine("   /V   显示当前进度。");
			Console.WriteLine();
		}

		private static Runner BuildRunner(string[] args, List<string> pathes)
		{
			bool printFile = false, output = false, progress = false;
			string outputFile = null;

			foreach (var arg in args)
			{
				switch (arg)
				{
				case "/F":
				case "/f":
					printFile = true;
					break;
				case "/O":
				case "/o":
					if (outputFile != null)
					{
						Console.WriteLine("参数错误，指定了多个输出文件。");
						return null;
					}
					output = true;
					break;
				case "/V":
				case "/v":
					progress = true;
					break;

				default:
					if (output)
					{
						output = false;
						outputFile = arg;
					}
					else
					{
						pathes.Add(arg);
					}
					break;
				}
			}

			TextWriter outputStream;
			if (outputFile == null)
			{
				progress = false;
				outputStream = Console.Out;
			}
			else
				outputStream = new StreamWriter(outputFile, false, Encoding.UTF8);

			return new Runner
			{
				printFile = printFile,
				output = outputStream,
				progress = progress,
			};
		}
	}
}
