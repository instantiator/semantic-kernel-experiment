using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SKE
{
    public static class KernelSkillsExtensions
    {
        /// <summary>
        /// Imports all skills found in the directory provided.
        /// </summary>
        /// <param name="kernel">This kernel</param>
        /// <param name="skillsDirectory">Directory to scan</param>
        public static void ImportAllSemanticSkills(this IKernel kernel, string skillsDirectory)
        {
            Directory.EnumerateDirectories(skillsDirectory)
                .Select(d => Path.GetFileName(d)).ToList()
                .ForEach(skill => kernel.ImportSemanticSkillFromDirectory(skillsDirectory, skill));
        }

        /// <summary>
        /// Scans the executing assembly for skills (classes with methods that have the SKFunction attribute), and imports them.
        /// </summary>
        /// <param name="kernel">This kernel</param>
        public static void ImportAllNativeSkills(this IKernel kernel)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var isSkill = type.GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(SKFunctionAttribute), false).Length > 0)
                    .Count() > 0;

                if (isSkill)
                {
                    Console.WriteLine($"Importing {type.FullName!}");
                    var skillObject = Activator.CreateInstance(type);
                    kernel.ImportSkill(skillObject!, type.Name!);
                }
            }
        }

        /// <summary>
        /// Imports the planner skill (which, in turn, knows about the kernel and all its skills).
        /// </summary>
        /// <param name="kernel">This kernel</param>
        /// <returns>The planner skill (a dictionary of strings to SKFunctions)</returns>
        public static IDictionary<string, ISKFunction> ImportPlannerSkill(this IKernel kernel)
            => kernel.ImportSkill(new PlannerSkill(kernel));

        /// <summary>
        /// Creates a plan to meet the ask provided, using the skills known to the planner.
        /// </summary>
        /// <param name="kernel">This kernel</param>
        /// <param name="planner">The planner functions</param>
        /// <param name="ask">The ask - what the planner is being asked to do</param>
        /// <returns>An SKContext containing the new plan</returns>
        public static async Task<SKContext> CreatePlanAsync(this IKernel kernel, IDictionary<string, ISKFunction> planner, string ask)
            => await kernel.RunAsync(ask, planner["CreatePlan"]);

        /// <summary>
        /// Executes the plan provided.
        /// </summary>
        /// <param name="kernel">This kernel</param>
        /// <param name="planner">The planner functions</param>
        /// <param name="plan">An SKContext containing the plan</param>
        /// <param name="maxSteps">The maximum number of steps to execute (optional, default = 10)</param>
        /// <returns>The final result from executing the plan</returns>
        public static async Task<string?> ExecutePlanAsync(this IKernel kernel, IDictionary<string, ISKFunction> planner, SKContext plan, int maxSteps = 10)
        {
            string? result = null;
            var executionResults = plan;
            var partialResults = new List<string>();

            int step = 1;
            while (!executionResults.Variables.ToPlan().IsComplete && step < maxSteps)
            {
                // execute the next step found in execution results
                var results = await kernel.RunAsync(executionResults.Variables, planner["ExecutePlan"]);
                if (results.Variables.ToPlan().IsSuccessful)
                {
                    result = results.Variables.ToPlan().Result;
                    partialResults.Add(result);
                }
                else
                {
                    throw new PlanExecutionException($"Step {step} execution failed.", plan, step, partialResults);
                }

                // iterate - using the execution results as the input for the next step
                executionResults = results;
                step++;
            }

            return result;
        }
    }
}
