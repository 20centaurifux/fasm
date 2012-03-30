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
using Assembler.Parser;

namespace Assembler.Compiler
{
	public class DelvecchioCompiler : ACompiler
	{
		public static Byte[] FORMAT_MAGIC = { 70, 79, 78, 90 };
		public static Int32 DATA_SEGMENT_SIZE = 512;

		protected override Byte[] Compile(Int32[] dataSegment, Instruction[] codeSegment)
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);
			int i;
			Int32 address;
			AParameter parameter;

			if(dataSegment.Length * 4 > DATA_SEGMENT_SIZE)
			{
				throw new CompilerException("Data segment size exceeds maximum.");
			}

			// write magic:
			writer.Write(FORMAT_MAGIC);

			// write data segment:
			for(i = 0; i < dataSegment.Length; ++i)
			{
				writer.Write(SerializeDWord(dataSegment[i]));
			}

			for(i = dataSegment.Length; i < DATA_SEGMENT_SIZE / 4; ++i)
			{
				writer.Write((Int32)0);
			}

			// write code segment:
			foreach(Instruction instruction in codeSegment)
			{
				writer.Write ((Byte)instruction.OPCode);

				for(i = 0; i < instruction.CountParamters(); ++i)
				{
					parameter = instruction.GetParameter(i);

					if(parameter is AddressParam)
					{
						address = ((AddressParam)parameter).DWord;

						if(((AddressParam)parameter).Segment == ESegment.Code)
						{
							address += DATA_SEGMENT_SIZE;
						}

						writer.Write(SerializeDWord(address));
					}
					else if(parameter is DWordParam)
					{
						writer.Write(SerializeDWord(((DWordParam)parameter).DWord));
					}
					else if(parameter is RegisterParam)
					{
						writer.Write((Byte)((RegisterParam)parameter).Register);
					}
					else
					{
						throw new CompilerException("Found invalid instruction parameter.");
					}
				}
			}

			// write memory stream to byte array:
			stream.Seek(0, SeekOrigin.Begin);
			Byte[] binary = new Byte[stream.Length];
			stream.Read(binary, 0, (int)stream.Length);

			return binary;
		}

		private Byte[] SerializeDWord(Int32 value)
		{
			Byte[] bytes = new Byte[4];

			bytes = BitConverter.GetBytes(value);

			if(BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}

			return bytes;
		}
	}
}
