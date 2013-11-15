/***************************************************************************
    begin........: March 2012
    copyright....: Sebastian Fedrau
    email........: lord-kefir@arcor.de
 ***************************************************************************/

/***************************************************************************
    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License v3 as published by
    the Free Software Foundation.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
    General Public License v3 for more details.
 ***************************************************************************/
using Gnu.Getopt;
using System;
using System.IO;
using System.Text;
using Assembler.Compiler;
using Assembler.Parser;

namespace Assembler
{
	class Assembler
	{
		public const string APP_NAME      = "fasm";
		public const long APP_MAJOR       = 0;
		public const long APP_MINOR       = 2;
		public const long APP_PATCHLEVEL  = 0;

		private const int OPTION_HELP     = 1;
		private const int OPTION_VERSION  = 2;
		private const int OPTION_OUTPUT   = 4;
		private const int OPTION_INPUT    = 8;
		private const int OPTION_DATAFILE = 16;

		private Assembler() { }

		private class Options
		{
		    public bool Help { get; set; }
		    public bool Version { get; set; }
		    public string Output { get; set; }
		    public string Input { get; set; }
		    public string DataFile { get; set; }
			public int Flags { get; set; }
		}

		static void Main (string[] args)
		{
			Options opts;
			Assembler instance;
			Byte[] bin;

			// parse options:
			opts = GetOptions(args);

			if(opts.Help)
			{
				PrintUsageAndExit(0);
			}
			else if(opts.Version)
			{
				PrintVersionAndExit();
			}

			if((opts.Flags & OPTION_INPUT) == 0 || (opts.Flags & OPTION_OUTPUT) == 0)
			{
				Console.Error.WriteLine("Missing arguments. Type in ''{0} --help'' to show usage.", APP_NAME);
				Environment.Exit(-1);
			}

			// compile program:
			try
			{
				instance = new Assembler();
				bin = instance.Compile(opts.Input, opts.DataFile);
				instance.Write(bin, opts.Output);
			}
			catch(ParserException e)
			{
				Console.WriteLine(String.Format("Error in line {0}", e.Line));
				Console.Error.WriteLine(e.ToString());
			}
			catch(CompilerException e)
			{
				Console.Error.WriteLine(e.ToString());
			}
			catch(Exception e)
			{
				Console.Error.WriteLine(e.ToString());
			}
		}

		public Byte[] Compile(String codeFile, String dataFile=null)
		{
			ACompiler compiler;

			compiler = new DelvecchioCompiler();

			return dataFile == null ?
				compiler.CompileFile(codeFile) : compiler.CompileAndMergeFiles(dataFile, codeFile);
		}

		public void Write(Byte[] bytes, String filename)
		{
			using(FileStream stream = new FileStream(filename, FileMode.Create))
			{
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write (bytes, 0, bytes.Length);
				writer.Flush();
				writer.Close ();
			}
		}

		private static Getopt BuildOptionParser(string[] args)
		{
			LongOpt[] longopts = new LongOpt[5];

			longopts[0] = new LongOpt("help", Argument.No, null, OPTION_HELP);
			longopts[1] = new LongOpt("version", Argument.No, null, OPTION_VERSION);
			longopts[2] = new LongOpt("out", Argument.Required, null, OPTION_OUTPUT); 
			longopts[3] = new LongOpt("in", Argument.Required, null, OPTION_INPUT);
			longopts[4] = new LongOpt("datafile", Argument.Required, null, OPTION_DATAFILE);

			return new Getopt(APP_NAME, args, "vho:i:d:", longopts);
		}

		private static Options GetOptions(string[] args)
		{
			var g = BuildOptionParser(args);
			int c;
			var opts = new Options() { Help = false, Flags = 0 };

			while((c = g.getopt()) != -1)
			{
				switch (c)
				{
					case OPTION_HELP:
						case 'h':
						opts.Help = true;
						opts.Flags |= OPTION_HELP;
						break;

					case OPTION_VERSION:
						case 'v':
						opts.Version = true;
						opts.Flags |= OPTION_VERSION;
						break;

					case OPTION_INPUT:
						case 'i':
						opts.Input = g.Optarg;
						opts.Flags |= OPTION_INPUT;
						break;

					case OPTION_OUTPUT:
						case 'o':
						opts.Output = g.Optarg;
						opts.Flags |= OPTION_OUTPUT;
						break;

					case OPTION_DATAFILE:
						case 'd':
						opts.DataFile = g.Optarg;
						opts.Flags |= OPTION_DATAFILE;
						break;
				}
			}

			return opts;
		}

		private static void PrintUsageAndExit(int exitCode = 0)
		{
			Console.WriteLine(string.Format("Usage: {0} [OPTION]...", APP_NAME));
			Console.WriteLine("-h, --help                 show this text and exit");
			Console.WriteLine("-v, --version              show version and exit");
			Console.WriteLine("-i, --in=FILENAME          source file to compile");
			Console.WriteLine("-o, --out=FILENAME         name of the built assembly");
			Console.WriteLine();

			Environment.Exit(exitCode);
		}

		private static void PrintVersionAndExit()
		{
		    Console.WriteLine(string.Format("{0}, version {1}.{2}.{3}", APP_NAME, APP_MAJOR, APP_MINOR, APP_PATCHLEVEL));
		    Environment.Exit(0);
		}
	}
}
