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
namespace Assembler.Parser
{
	public enum EOPCode
	{
		Undefined = 0,
		MOV_RegReg = 1,
		MOV_RegAddr = 2,
		MOV_AddrReg = 3,
		MOV_RegDWord = 4,
		INC = 5,
		DEC = 6,
		SUB_RegReg = 7,
		SUB_RegAddr = 8,
		SUB_RegDWord = 9,
		ADD_RegReg = 10,
		ADD_RegAddr = 11,
		ADD_RegDWord = 12,
		MUL_RegReg = 13,
		MUL_RegAddr = 14,
		MUL_RegDWord = 15,
		DIV_RegReg = 16,
		DIV_RegAddr = 17,
		DIV_RegDWord = 18,
		RET = 19,
		CMP_RegAddr = 20,
		CMP_RegReg = 21,
		JE = 22,
		JNE = 23,
		JGE = 24,
		JG = 25,
		JLE = 26,
		JL = 27,
		CALL = 28
	}
}
