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
namespace Assembler.Parser
{
	public enum EOPCode
	{
		Undefined = 0,
		MOV_RegReg = 1,
		MOV_RegAddr = 2,
		MOV_AddrReg = 3,
		MOV_RegDWord = 4,
		MOV_RegAddrInReg = 5,
		INC = 6,
		DEC = 7,
		SUB_RegReg = 8,
		SUB_RegAddr = 9,
		SUB_RegDWord = 10,
		ADD_RegReg = 11,
		ADD_RegAddr = 12,
		ADD_RegDWord = 13,
		MUL_RegReg = 14,
		MUL_RegAddr = 15,
		MUL_RegDWord = 16,
		DIV_RegReg = 17,
		DIV_RegAddr = 18,
		DIV_RegDWord = 19,
		AND_RegReg = 20,
		AND_RegAddr = 21,
		AND_RegDWord = 22,
		OR_RegReg = 23,
		OR_RegAddr = 24,
		OR_RegDWord = 25,
		MOD_RegReg = 26,
		MOD_RegAddr = 27,
		MOD_RegDWord = 28,
		RND = 29,
		RET = 30,
		CMP_RegAddr = 31,
		CMP_RegReg = 32,
		JE = 33,
		JNE = 34,
		JGE = 35,
		JG = 36,
		JLE = 37,
		JL = 38,
		CALL = 39,
		OP_CODE_PUSH = 40,
		OP_CODE_POP = 41,
		MOVS_RegStack = 42,
		MOVS_StackReg = 43
	}
}
