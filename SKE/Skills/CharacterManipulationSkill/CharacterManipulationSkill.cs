using System;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SKE.Skills.CharacterManipulationSkill
{
	public class CharacterManipulationSkill
	{
		[SKFunction("Return the text in all uppercase (aka capitals)")]
		public string Uppercase(string input)
			=> input.ToUpper();
	}
}

