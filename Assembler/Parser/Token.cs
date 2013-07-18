﻿/***************************************************************************
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

namespace Assembler.Parser
{
	public class Token
	{
		public Token(EToken type, String text)
		{
			this.Type = type;
			this.Text = text;
		}

		public EToken Type
		{
			get;
			set;
		}

		public String Text
		{
			get;
			set;
		}
	}
}
