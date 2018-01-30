using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Harmony
{
	public class CodeInstruction
	{
		public OpCode opcode;
		public object operand;
		public Label? label;

		public int startException = 0;
		public int endException = 0;
		public Type catchType = null;
		public bool isStartCatch;
		public bool isStartFinally;
		public bool isStartFilter;

		public CodeInstruction(OpCode opcode, object operand = null)
		{
			this.opcode = opcode;
			this.operand = operand;
		}

		public CodeInstruction(CodeInstruction instruction)
		{
			opcode = instruction.opcode;
			operand = instruction.operand;
			label = instruction.label;
		}

		public override string ToString()
		{
			return string.Format(opcode + " " + operand);
		}
	}
}