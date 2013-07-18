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
using System.Collections.Generic;

namespace Assembler.Parser
{
	public class Instruction
	{
		private List<AParameter> parameters = new List<AParameter>();

		public Instruction(EOPCode code)
		{
			OPCode = code;
		}

		public void AddParameter(AParameter parameter)
		{
			parameters.Add(parameter);
		}

		public AParameter GetParameter(int index)
		{
			return parameters[index];
		}

		public void ReplaceParameter(int index, AParameter parameter)
		{
			parameters[index] = parameter;
		}

		public void ClearParameters()
		{
			parameters.Clear();
		}

		public int CountParamters()
		{
			return parameters.Count;
		}

		public int Size
		{
			get
			{
				int size = 0;

				foreach(AParameter p in parameters)
				{
					size += p.Size;
				}

				return size + 1;
			}
		}

		public EOPCode OPCode
		{
			set;
			get;
		}
	}
}
