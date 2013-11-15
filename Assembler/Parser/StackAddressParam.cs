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

namespace Assembler.Parser
{
	public class StackAddressParam : AParameter
	{
		private static Regex addressRegex = new Regex("^\\[sp\\-(\\d+)\\]$");

		public StackAddressParam(int offset)
		{
			Offset = offset;
		}

		public static StackAddressParam Parse(string text)
		{
			var m = addressRegex.Match(text);

			return new StackAddressParam(Convert.ToInt32(m.Groups[1].ToString()));
		}

		public override int Size
		{
			get
			{
				return 4;
			}
		}

		public Int32 Offset
		{
			get;
			set;
		}
	}
}
