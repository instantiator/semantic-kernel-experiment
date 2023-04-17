using System;
using Microsoft.SemanticKernel.Orchestration;

namespace SKE
{
	public class PlanExecutionException : Exception
	{
		public IEnumerable<string> PartialResults;
		public SKContext Plan;
		public int FailedStep;

		public PlanExecutionException(string message, SKContext plan, int failedStep, IEnumerable<string> partialResults) : base(message)
		{
			this.FailedStep = failedStep;
			this.Plan = plan;
			this.PartialResults = partialResults;
		}
    }
}

