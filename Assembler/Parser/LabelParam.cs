﻿/***************************************************************************
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
	public class LabelParam : AParameter
	{
		public LabelParam(String label)
		{
			Label = label;
		}

		public override int Size
		{
			get
			{
				return 4;
			}
		}

		public String Label
		{
			get;
			set;
		}
	}
}