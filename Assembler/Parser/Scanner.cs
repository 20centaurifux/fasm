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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Parser
{
	public class Scanner
	{
		private String asm;

		public void Feed(string asm)
		{
			this.asm = asm;
		}

		public IEnumerable<Token> Next()
		{
			foreach(String line in asm.Split('\n'))
			{
				foreach(Token token in ScanLine(line))
				{
					yield return token;
				}

				yield return new Token(EToken.Linebreak, "\n");
			}
		}

		private IEnumerable<Token> ScanLine(String line)
		{
			int offset;

			// find comment:
			if((offset = line.IndexOf(';')) == -1)
			{
				// comment not found => set offset to last character of string
				offset = line.Length;
			}

			// separate line by white space:
			foreach(String t in from String t in line.Substring(0, offset).Split(' ') where t.Trim().Length > 0 select t.Trim())
			{
				// process found token:
				foreach(Token token in ScanText(t))
				{
					yield return token;
				}
			}

			// return found comment:
			if(offset != line.Length)
			{
				yield return new Token(EToken.Comment, line.Substring(offset).Trim());
			}
		}

		private IEnumerable<Token> ScanText(String text)
		{
			StringBuilder builder = new StringBuilder();

			// find separator (',', '=' or ':'):
			foreach(Char c in text)
			{
				// separator found => return token & separator:
				if(Validators.IsSeparator(c.ToString()))
				{
					yield return StringToToken(builder.ToString());
					yield return StringToToken(c.ToString());
					builder.Length = 0;
				}
				else
				{
					// separator not found => append character to buffer
					builder.Append(c);
				}
			}

			// return last token (if exists):
			if(builder.Length > 0)
			{
				yield return StringToToken(builder.ToString());
			}
		}

		private Token StringToToken(String text)
		{
			if(Validators.IsSection(text))
			{
				return new Token(EToken.Section, text);
			}
			else if(Validators.IsSeparator(text))
			{
				return new Token(EToken.Separator, text);
			}
			else if(Validators.IsMnemonic(text))
			{
				return new Token(EToken.Mnemonic, text);
			}
			else if(Validators.IsRegister(text))
			{
				return new Token(EToken.Register, text);
			}
			else if(Validators.IsNumber(text))
			{
				return new Token(EToken.Number, text);
			}
			else if(Validators.IsAddressFromRegister(text))
			{
				return new Token(EToken.AddressFromRegister, text);
			}
			else if(Validators.IsAddress(text))
			{
				return new Token(EToken.Address, text);
			}
			else if(Validators.IsStackAddress(text))
			{
				return new Token(EToken.StackAddress, text);
			}
			else
			{
				return new Token(EToken.Text, text);
			}
		}
	}
}
