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
using System;
using System.IO;
using System.Text;
using Assembler;
using Assembler.Parser;

namespace Assembler.Compiler
{
	public abstract class ACompiler
	{
		protected abstract Byte[] Compile(Int32[] dataSegment, Instruction[] codeSegment);

		public Byte[] CompileFile(String filename)
		{
			var asm = Encoding.ASCII.GetString(System.IO.File.ReadAllBytes(filename));

			return CompileText(asm);
		}

		public Byte[] CompileAndMergeFiles(string dataFile, string codeFile)
		{
			var dataSegment = ReadDataSegmentFromFile(dataFile);
			var asm = Encoding.ASCII.GetString(System.IO.File.ReadAllBytes(codeFile));

			return CompileAndMerge(dataSegment, asm);
		}

		public Byte[] CompileText(String asm)
		{
			var parser = ParseASM(asm);

			return Compile(parser.GetDataSegment(), parser.GetCodeSegment());
		}

		public Byte[] CompileAndMerge(Int32[] dataSegment, String asm)
		{
			var parser = new Parser.Parser();
			parser.Parse(asm);

			return Compile(dataSegment, parser.GetCodeSegment());
		}

		private Int32[] ReadDataSegmentFromFile (string filename)
		{
			var bytes = System.IO.File.ReadAllBytes (filename);
			var dataSegment = new int[bytes.Length / 4];
			var intBytes = new byte[4];
			Int32 value;

			for(int i = 0; i < dataSegment.Length; i++)
			{
				Array.Copy(bytes, i * 4, intBytes, 0, 4);

				if(BitConverter.IsLittleEndian)
				{
					Array.Reverse(intBytes);
				}

				value = BitConverter.ToInt32(intBytes, 0);
				dataSegment[i] = value;
			}

			return dataSegment;
		}

		private Parser.Parser ParseASM(string asm)
		{
			var parser = new Parser.Parser();
			parser.Parse(asm);

			return parser;
		}
	}
}

