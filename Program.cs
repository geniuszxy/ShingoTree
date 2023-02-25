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
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

			if (args.Length == 0)
				goto ERROR;

			List<string> pathes = new List<string>();
			Runner runner = null;
			try { runner = BuildRunner(args, pathes); }
			catch { goto ERROR; }

			if (pathes.Count == 0)
				pathes.Add(Environment.CurrentDirectory);

			runner.Lookup(pathes);
			return;

			ERROR:
			PrintHelp();
		}

		private static void PrintHelp()
		{
			Console.WriteLine("以图形显示驱动器或路径的文件夹结构。");
			Console.WriteLine();
			Console.WriteLine("SHINGOTREE [/F [/E ext[,...]]] [/O output [/V]] [path, ...]");
			Console.WriteLine();
			Console.WriteLine("   /F   显示每个文件夹中文件的名称。");
			Console.WriteLine("   /E   过滤输出的文件名称。");
			Console.WriteLine("   /O   将结果输出到文件。");
			Console.WriteLine("   /V   显示当前进度。");
			Console.WriteLine();
		}

		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			PrintHelp();
		}

		private static Runner BuildRunner(string[] args, List<string> pathes)
		{
			bool printFile = false, progress = false;
			string outputFile = null;
			List<string> extensionFilter = null;

			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];

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
						outputFile = args[++i];
						break;

					case "/V":
					case "/v":
						progress = true;
						break;

					case "/E":
					case "/e":
						extensionFilter = new List<string>(args[++i].Split(','));
						break;

					default:
						pathes.Add(arg);
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
				extensionFilter = extensionFilter,
			};
		}
	}
}
