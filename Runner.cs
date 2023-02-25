using System;
using System.Collections.Generic;
using System.IO;

namespace ShingoTree
{
	class Runner
	{
		public bool printFile;
		public bool progress;
		public TextWriter output;
		public List<string> extensionFilter;

		private double _progress;
		private int _lastReportProgress;

		void LookUpDirectory(string path, string pprefix, double maxprog)
		{
			string[] directories;
			try { directories = Directory.GetDirectories(path); }
			catch (UnauthorizedAccessException) { Console.Error.WriteLine("Cannot access: " + path); return; }
			int dirLength = directories.Length;

			if (printFile)
			{
				string[] files = Directory.GetFiles(path);
				if (files.Length > 0)
				{
					string prefix = pprefix + (dirLength > 0 ? "│  " : "    ");
					if (extensionFilter != null)
					{
						foreach (var file in files)
						{
							if (!CompareExtension(file))
								continue;
							output.Write(prefix);
							output.WriteLine(file.Substring(path.Length + 1));
						}
					}
					else
					{
						foreach (var file in files)
						{
							output.Write(prefix);
							output.WriteLine(file.Substring(path.Length + 1));
						}
					}
					output.WriteLine(prefix);
				}
			}

			if (dirLength > 0)
			{
				double prog = _progress;
				double progAdv = (maxprog - prog) / dirLength;

				int dirLengthM1 = dirLength - 1;

				if (dirLengthM1 > 0)
				{
					string prefix = pprefix + "├─";
					string subPrefix = pprefix + "│  ";
					for (int i = 0; i < dirLengthM1; i++)
						OutputDirectory(path, directories[i], prog += progAdv, prefix, subPrefix);
				}
				OutputDirectory(path, directories[dirLengthM1], prog + progAdv,
					pprefix + "└─", pprefix + "    ");
			}
		}

		void OutputDirectory(string path, string directory, double maxprog, string prefix, string subPrefix)
		{
			output.Write(prefix);
			output.WriteLine(directory.Substring(path.Length + 1));
			LookUpDirectory(directory, subPrefix, maxprog);
			ReportProgress(maxprog);
		}

		void ReportProgress(double p)
		{
			const int PROGRESS_BAR_WIDTH = 70;

			if (progress)
			{
				int intProg = (int)(p * 1000);
				if (intProg != _lastReportProgress)
				{
					_lastReportProgress = intProg;
					Console.Write("\r[");
					int w = (int)Math.Round(p * PROGRESS_BAR_WIDTH);
					Console.Write(new string('=', w));
					Console.Write(new string(' ', PROGRESS_BAR_WIDTH - w));
					Console.Write("] " + p.ToString("#.0%"));
				}
			}
			_progress = p;
		}

		bool CompareExtension(string file)
		{
			var ext = Path.GetExtension(file);
			foreach (var extf in extensionFilter)
				if (string.Compare(ext, extf, ignoreCase: true) == 0)
					return true;
			return false;
		}

		internal void Lookup(List<string> pathes)
		{
			_progress = 0.0;
			_lastReportProgress = 0;
			int count = pathes.Count;
			double maxprog = 0.0;

			for (int i = 0; i < count; i++)
			{
				string path = Path.GetFullPath(pathes[i]);
				output.WriteLine(path);
				maxprog += 1.0 / count;
				LookUpDirectory(path, "", maxprog);
				ReportProgress(maxprog);
				output.WriteLine();
			}

			output.Close();
		}
	}
}
