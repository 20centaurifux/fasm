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
using System.Collections.Generic;

namespace Assembler.Parser
{
	public class Parser
	{
		private static Dictionary<String, ERegister> registers = new Dictionary<String, ERegister>()
		{
			{ "a0", ERegister.A0 }, { "a1", ERegister.A1 }, { "a2", ERegister.A2 }, { "a3", ERegister.A3 },
			{ "r", ERegister.R }, { "ip", ERegister.IP }, { "sp", ERegister.SP }, { "fl", ERegister.FL },
		};

		private static Dictionary<String, EOPCode> mnemonics = new Dictionary<String, EOPCode>()
		{
			{ "inc", EOPCode.INC }, { "dec", EOPCode.DEC }, { "je", EOPCode.JE }, { "jne", EOPCode.JNE },
			{ "jge", EOPCode.JGE }, { "jg", EOPCode.JG }, { "jle", EOPCode.JLE }, { "jl", EOPCode.JL },
			{ "call", EOPCode.CALL }, { "ret", EOPCode.RET }
		};

		private UInt32 line;
		private Int32 flags;
		private const Int32 FLAG_SEGMENT_DATA = 0x1;
		private const Int32 FLAG_SEGMENT_CODE = 0x2;
		private List<Int32> dataSegment = new List<Int32>();
		private List<String> variableNames = new List<String>();
		private Dictionary<String, Int32> labels = new Dictionary<String, Int32>();
		private int address;
		private List<Instruction> instructions = new List<Instruction>();

		public void Reset()
		{
			line = 0;
			flags = 0;
			dataSegment.Clear();
			variableNames.Clear();
			address = 0;
			labels.Clear();
			instructions.Clear();
		}

		public void Parse(String asm)
		{
			Scanner scanner;
			List<Token> token = new List<Token>();
			int i;
			String label;

			Reset();

			scanner = new Scanner();
			scanner.Feed(asm);

			// analyze & parse text:
			foreach(Token t in scanner.Next())
			{
				if(t.Type == EToken.Linebreak)
				{
					++line;

					if(token.Count > 0)
					{
						ParseInstruction(token.ToArray());
					}

					token.Clear();
				}
				else
				{
					token.Add(t);
				}
			}

			// replace labels:
			foreach(Instruction instruction in instructions)
			{
				for(i = 0; i < instruction.CountParamters(); ++i)
				{
					if(instruction.GetParameter(i) is LabelParam)
					{
						label = ((LabelParam)instruction.GetParameter(i)).Label;

						if(!labels.ContainsKey(label))
						{
							throw new ParserException(line, string.Format("Couldn't find label: \"{0}\"", label));
						}

						instruction.ReplaceParameter(i, new AddressParam(labels[label], ESegment.Code));
					}
				}
			}
		}

		public Int32[] GetDataSegment()
		{
			return dataSegment.ToArray();
		}

		public Instruction[] GetCodeSegment()
		{
			return instructions.ToArray();
		}

		private void ParseInstruction(Token[] token)
		{
			switch (token[0].Type)
			{
			case EToken.Section:
				ProcessSection(token);
				break;

			case EToken.Text:
				ProcessText(token);
				break;

			case EToken.Mnemonic:
				ProcessMnemonic(token);
				break;

			case EToken.Comment:
				break;

			default:
				throw new UnexeptedTokenException(line, 1, token[0].Text);
			}
		}

		private void ProcessSection(Token[] token)
		{
			if(token[0].Text == ".data")
			{
				// test if .data & .code segment have already been defined:
				if((flags & FLAG_SEGMENT_DATA) != 0)
				{
					throw new ParserException(line, ".data segement has already been defined.");
				}
				if((flags & FLAG_SEGMENT_CODE) != 0)
				{
					throw new ParserException(line, "Invalid .data segement location.");
				}

				flags |= FLAG_SEGMENT_DATA;
			}
			else if(token[0].Text == ".code")
			{
				// test if .code segment has already been defined:
				if((flags & FLAG_SEGMENT_CODE) != 0)
				{
					throw new ParserException(line, ".code segement has already been defined.");
				}

				flags |= FLAG_SEGMENT_CODE;
			}
			else
			{
				throw new ParserException(line, String.Format("Invalid section: \"{0}\"", token[0].Text));
			}
		}

		private void ProcessText(Token[] token)
		{
			// check if we've found a variable:
			if(((flags & FLAG_SEGMENT_DATA) != 0) && ((flags & FLAG_SEGMENT_CODE) == 0))
			{
				if(!Validators.IsVariable(token[0].Text))
				{
					throw new ParserException(line, String.Format("Invalid data declaration: \"{0}\"", token[0].Text));
				}

				if(token.Length < 3 || token.Length > 4)
				{
					throw new ParserException(line, "Wrong number of arguments.");
				}

				if(token[1].Type != EToken.Separator || token[1].Text != "=")
				{
					throw new UnexeptedTokenException(line, 2, token[1].Text);
				}

				if(token[2].Type != EToken.Number)
				{
					throw new UnexeptedTokenException(line, 2, token[2].Text);
				}

				if(token.Length == 4 && token[3].Type != EToken.Comment)
				{
					throw new UnexeptedTokenException(line, 4, token[3].Text);
				}

				if(variableNames.Contains(token[0].Text))
				{
					throw new ParserException(line, String.Format("Variable \"{0}\" has already been assigned.", token[0].Text));
				}

				// store variable name & value:
				variableNames.Add(token[0].Text);
				dataSegment.Add(Convert.ToInt32(token[2].Text));
			}
			else if((flags & FLAG_SEGMENT_CODE) != 0)
			{
				// check if we've found a label:
				if(token.Length < 2 || token.Length > 3)
				{
					throw new ParserException(line, "Wrong number of arguments.");
				}

				if(token[1].Type != EToken.Separator && token[1].Text != ":")
				{
					throw new UnexeptedTokenException(line, 3, token[1].Text);
				}

				if(token.Length == 3 && token[2].Type != EToken.Comment)
				{
					throw new UnexeptedTokenException(line, 3, token[2].Text);
				}

				if(!Validators.isLabel(token[0].Text))
				{
					throw new UnexeptedTokenException(line, 1, token[0].Text);
				}

				if(variableNames.Contains(token[0].Text) || labels.ContainsKey(token[0].Text))
				{
					throw new ParserException(line, String.Format("\"{0}\" has already been defined.", token[0].Text));
				}

				labels.Add(token[0].Text, address);
			}
			else
			{
				throw new UnexeptedTokenException(line, 1, token[0].Text);
			}
		}

		private void ProcessMnemonic(Token[] token)
		{
			if(token[0].Text == "mov")
			{
				ProcessMov(token);
			}
			else if(token[0].Text == "inc" || token[0].Text == "dec")
			{
				ProcessIncDec(token);
			}
			else if(token[0].Text == "add" || token[0].Text == "sub" || token[0].Text == "mul" || token[0].Text == "div")
			{
				ProcessAddSubMulDiv(token);
			}
			else if(token[0].Text == "cmp")
			{
				ProcessCmp(token);
			}
			else if(Validators.IsJumpMnemonic(token[0].Text))
			{
				ProcessJump(token);
			}
			else
			{
				ProcessSingleByteMnemonic(token);
			}
		}

		private void ProcessMov(Token[] token)
		{
			Instruction instruction = null;
			String varName;

			if(token.Length < 4 || token.Length > 5)
			{
				throw new ParserException(line, "Wrong number of arguments.");
			}

			if(token[2].Type != EToken.Separator || token[2].Text != ",")
			{
				throw new UnexeptedTokenException(line, 3, token[1].Text);
			}

			if(token.Length == 5 && token[4].Type != EToken.Comment)
			{
				throw new UnexeptedTokenException(line, 5, token[3].Text);
			}

			if(token[1].Type == EToken.Register)
			{
				if(Validators.IsWriteProtectedRegister(token[1].Text))
				{
					throw new ParserException(line, "Cannot access write protected register.");
				}
			}

			if(token[1].Type == EToken.Register && token[3].Type == EToken.Register)
			{
				instruction = new Instruction(EOPCode.MOV_RegReg);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new RegisterParam(registers[token[3].Text]));
			}
			else if(token[1].Type == EToken.Register && token[3].Type == EToken.Address)
			{
				varName = GetVarFromAddress(token[3].Text);

				if(!variableNames.Contains(varName))
				{
					throw new ParserException(line, String.Format("Couldn't find variable: \"{0}\"", varName));
				}

				instruction = new Instruction(EOPCode.MOV_RegAddr);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new AddressParam(variableNames.IndexOf(varName) * 4, ESegment.Data));
			}
			else if(token[1].Type == EToken.Register && token[3].Type == EToken.Number)
			{
				instruction = new Instruction(EOPCode.MOV_RegDWord);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new DWordParam(Convert.ToInt32(token[3].Text)));
			}
			else if(token[1].Type == EToken.Address && token[3].Type == EToken.Register)
			{
				if(!variableNames.Contains(token[1].Text.Substring(1, token[1].Text.Length - 2)))
				{
					throw new ParserException(line, String.Format("Invalid address: \"{0}\"", token[1].Text));
				}

				instruction = new Instruction(EOPCode.MOV_AddrReg);
				instruction.AddParameter(new AddressParam(variableNames.IndexOf(token[1].Text.Substring(1, token[1].Text.Length - 2)) * 4, ESegment.Data));
				instruction.AddParameter(new RegisterParam(registers[token[3].Text]));
			}
			else
			{
				throw new ParserException(line, "Invalid mov command.");
			}

			if(instruction != null)
			{
				instructions.Add(instruction);
				address += instruction.Size;
			}
		}

		private void ProcessIncDec(Token[] token)
		{
			Instruction instruction;

			if(token.Length < 2 || token.Length > 3)
			{
				throw new ParserException(line, "Wrong number of arguments.");
			}

			if(token[1].Type != EToken.Register)
			{
				throw new UnexeptedTokenException(line, 2, token[1].Text);
			}

			if(token.Length == 3 && token[2].Type != EToken.Comment)
			{
				throw new UnexeptedTokenException(line, 3, token[2].Text);
			}

			if(Validators.IsWriteProtectedRegister(token[1].Text))
			{
				throw new ParserException(line, "Cannot access write protected register.");
			}

			instruction = new Instruction(mnemonics[token[0].Text]);
			instructions.Add(instruction);
			instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
			address += instruction.Size;
		}

		private void ProcessAddSubMulDiv(Token[] token)
		{
			Instruction instruction;
			int offset = 0;
			String varName;

			if(token[0].Text == "add")
			{
				offset = 3;
			}
			else if(token[0].Text == "mul")
			{
				offset = 6;
			}
			else if(token[0].Text == "div")
			{
				offset = 9;
			}

			if(token.Length < 4 || token.Length > 5)
			{
				throw new ParserException(line, "Wrong number of arguments.");
			}

			if(token[1].Type != EToken.Register)
			{
				throw new UnexeptedTokenException(line, 2, token[1].Text);
			}

			if(Validators.IsWriteProtectedRegister(token[1].Text))
			{
				throw new ParserException(line, "Cannot access write protected register.");
			}

			if(token[2].Type != EToken.Separator && token[2].Text != ",")
			{
				throw new UnexeptedTokenException(line, 3, token[2].Text);
			}

			if(token.Length == 5 && token[4].Type != EToken.Comment)
			{
				throw new UnexeptedTokenException(line, 3, token[2].Text);
			}

			if(token[3].Type == EToken.Register)
			{
				instruction = new Instruction(EOPCode.SUB_RegReg + offset);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new RegisterParam(registers[token[3].Text]));
			}
			else if(token[3].Type == EToken.Number)
			{
				instruction = new Instruction(EOPCode.SUB_RegDWord + offset);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new DWordParam(Convert.ToInt32(token[3].Text)));
			}
			else if(token[3].Type == EToken.Address)
			{
				varName = GetVarFromAddress(token[3].Text);

				if(!variableNames.Contains(varName))
				{
					throw new ParserException(line, String.Format("Couldn't find variable: \"{0}\"", varName));
				}

				instruction = new Instruction(EOPCode.SUB_RegAddr + offset);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new AddressParam(variableNames.IndexOf(varName) * 4, ESegment.Data));
			}
			else
			{
				throw new UnexeptedTokenException(line, 4, token[3].Text);
			}

			if(instruction != null)
			{
				instructions.Add(instruction);
				address += instruction.Size;
			}
		}

		private void ProcessCmp(Token[] token)
		{
			Instruction instruction;
			String varName;

			if(token.Length < 4 || token.Length > 5)
			{
				throw new ParserException(line, "Wrong number of arguments.");
			}

			if(token[1].Type != EToken.Register)
			{
				throw new ParserException(line, "Invalid cmp command.");
			}

			if(Validators.IsWriteProtectedRegister(token[1].Text))
			{
				throw new ParserException(line, "Cannot access write protected register.");
			}

			if(token[2].Type != EToken.Separator || token[2].Text != ",")
			{
				throw new UnexeptedTokenException(line, 3, token[2].Text);
			}

			if(token[3].Type != EToken.Address && token[3].Type != EToken.Register)
			{
				throw new ParserException(line, "Invalid cmp command.");
			}

			if(token.Length == 5 && token[4].Type != EToken.Comment)
			{
				throw new UnexeptedTokenException(line, 5, token[4].Text);
			}

			if(token[3].Type == EToken.Register)
			{
				instruction = new Instruction(EOPCode.CMP_RegReg);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new RegisterParam(registers[token[3].Text]));
			}
			else
			{
				varName = GetVarFromAddress(token[3].Text);

				if(!variableNames.Contains(varName))
				{
					throw new ParserException(line, String.Format("Couldn't find variable: \"{0}\"", varName));
				}

				instruction = new Instruction(EOPCode.CMP_RegAddr);
				instruction.AddParameter(new RegisterParam(registers[token[1].Text]));
				instruction.AddParameter(new AddressParam(variableNames.IndexOf(varName) * 4, ESegment.Data));
			}

			if(instruction != null)
			{
				instructions.Add(instruction);
				address += instruction.Size;
			}
		}

		private void ProcessJump(Token[] token)
		{
			Instruction instruction;

			if(token.Length < 2 || token.Length > 3)
			{
				throw new ParserException(line, "Wrong number of arguments.");
			}

			if(token[1].Type != EToken.Text || !Validators.isLabel(token[1].Text) || variableNames.Contains(token[1].Text))
			{
				throw new UnexeptedTokenException(line, 2, token[1].Text);
			}

			if(token.Length == 3 && token[2].Type != EToken.Comment)
			{
				throw new UnexeptedTokenException(line, 3, token[2].Text);
			}

			instruction = new Instruction(mnemonics[token[0].Text]);
			instruction.AddParameter(new LabelParam(token[1].Text));
			instructions.Add(instruction);
			address += instruction.Size;
		}

		private String GetVarFromAddress(String address)
		{
			return address.Substring(1, address.Length - 2);
		}

		private void ProcessSingleByteMnemonic(Token[] token)
		{
			Instruction instruction;

			if(token.Length > 2)
			{
				throw new ParserException(line, "Wrong number of arguments.");
			}

			if(token.Length == 2 && token[1].Type != EToken.Comment)
			{
				throw new UnexeptedTokenException(line, 2, token[1].Text);
			}

			instruction = new Instruction(mnemonics[token[0].Text]);
			instructions.Add(instruction);
			address += instruction.Size;
		}
	}
}
