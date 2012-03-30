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

namespace Assembler.Parser
{
	class ParserException : Exception
	{
		public ParserException(UInt32 line)
		{
			Line = line;
		}

		public ParserException(UInt32 line, String message) : base(message)
		{
			Line = line;
		}

		public UInt32 Line
		{
			get;
			set;
		}
	}
}
