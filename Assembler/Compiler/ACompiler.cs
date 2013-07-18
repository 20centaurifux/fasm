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
		protected abstract Byte[] Compile(Int32[] dateSegment, Instruction[] codeSegment);

		public Byte[] Compile(String asm)
		{
			Parser.Parser parser;

			parser = new Parser.Parser();
			parser.Parse (asm);

			return Compile(parser.GetDataSegment(), parser.GetCodeSegment());
		}

		public Byte[] CompileFile(String filename)
		{
			using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Byte[] buffer = new Byte[stream.Length];
				stream.Read(buffer, 0, (int)stream.Length);

				return Compile(Encoding.ASCII.GetString(buffer));
			}
		}
	}
}

