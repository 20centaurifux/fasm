/***************************************************************************
    begin........: March 2012
    copyright....: Sebastian Fedrau
    email........: lord-kefir@arcor.de
 ***************************************************************************/

/***************************************************************************
    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
    General Public License for more details.
 ***************************************************************************/
using System;
using System.IO;
using Assembler.Compiler;
using Assembler.Parser;

namespace Assembler
{
	class Assembler
	{
		private Assembler() { }

		static void Main(string[] args)
		{
			Assembler instance;
			Byte[] bin;

			if(args.Length != 2)
			{
				Console.WriteLine("Wrong number of arguments.");
				Environment.Exit(-1);
			}

			try
			{
				instance = new Assembler();
				bin = instance.Compile(args[0]);
				instance.Write(bin, args[1]);
			}
			catch(ParserException e)
			{
				Console.Error.WriteLine(e.ToString());
			}
			catch(CompilerException e)
			{
				Console.Error.WriteLine(e.ToString());
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public Byte[] Compile(String filename)
		{
			ACompiler compiler;

			compiler = new DelvecchioCompiler();

			return compiler.CompileFile(filename);
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
	}
}
