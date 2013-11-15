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
using System.Text.RegularExpressions;
using System.Linq;

namespace Assembler.Parser
{
	public static class Validators
	{
		private static Regex isNumberRegex = new Regex("^[0-9]+$");
		private static Regex isAddressRegex = new Regex("^\\[[a-zA-Z_]+[a-zA-Z0-9_]*\\]$");
		private static Regex extractAddressRegex = new Regex("^\\[(.*)\\]$");
		private static Regex isVariableRegex = new Regex("^[a-zA-Z_]+[a-zA-Z0-9_]*$");
		private static Regex isLabelRegex = isVariableRegex;
		private static Regex isStackAddress = new Regex("^\\[sp\\-\\d+\\]$");
		private static String[] registers = { "a0", "a1", "a2", "a3", "r", "ip", "sp", "fl" };
		private static String[] writeProtectedRegisters = { "ip", "sp", "fl" };
		private static String[] mnemonics = { "mov", "inc", "dec", "sub", "add", "mul", "div", "rnd",
		                                      "and", "or", "mod", "ret", "cmp", "je", "jne", "jge",
		                                      "jg", "jle", "jl", "call", "ret", "push", "pop", "movs" };
		private static String[] jumpMnemonics = { "je", "jne", "jge", "jg", "jle", "jl", "call" };
		private static String[] stackMnemonics = { "push", "pop" };
		private static String[] singleMnemonics = { "rnd" };
		private static String[] separators = { "=", ",", ":" };
		private static String[] sections = { ".data", ".code" };

		public static Boolean IsNumber(String text)
		{
			return isNumberRegex.IsMatch(text);
		}

		public static Boolean IsAddress(String text)
		{
			if(isAddressRegex.IsMatch(text))
			{
				if(IsVariable(text.Substring(1, text.Length - 2)))
				{
					return true;
				}
			}

			return false;
		}

		public static Boolean IsStackAddress(String text)
		{
			return isStackAddress.IsMatch(text);
		}

		public static Boolean IsVariable(String text)
		{
			if(IsRegister(text) || IsMnemonic(text))
			{
				return false;
			}

			if(isVariableRegex.IsMatch(text))
			{
				if(text.Length <= 16)
				{
					return true;
				}
			}

			return false;
		}

		public static Boolean IsRegister(String text)
		{
			return registers.Contains(text);
		}

		public static Boolean IsAddressFromRegister(String text)
		{
			Match m = extractAddressRegex.Match(text);

			if(m.Success)
			{
				return registers.Contains(m.Groups[1].ToString());
			}

			return false;
		}

		public static Boolean IsWriteProtectedRegister(String text)
		{
			return writeProtectedRegisters.Contains(text);
		}

		public static Boolean IsMnemonic(String text)
		{
			return mnemonics.Contains(text);
		}

		public static Boolean IsJumpMnemonic(String text)
		{
			return jumpMnemonics.Contains(text);
		}

		public static Boolean IsStackMnemonic(String text)
		{
			return stackMnemonics.Contains(text);
		}

		public static Boolean IsSingleMnemonic (String text)
		{
			return singleMnemonics.Contains(text);
		}

		public static Boolean IsSeparator(String text)
		{
			return separators.Contains(text);
		}

		public static Boolean IsSection(String text)
		{
			return sections.Contains(text);
		}

		public static Boolean isLabel(String text)
		{
			return isLabelRegex.IsMatch(text);
		}
	}
}
