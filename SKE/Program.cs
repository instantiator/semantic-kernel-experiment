using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace SKE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine(string.Join("\n", args));

            // configure and create the semantic kernel
            var config = RetrieveConfiguration();
            var kernel = BuildAndConfigureKernel(config);

            // import defined skills, and the planner
            kernel.ImportAllSemanticSkills("Skills");
            kernel.ImportAllNativeSkills();
            var planner = kernel.ImportPlannerSkill();

            // provide a simple input and ask as an example
            var input = "Yesterday I went to the London Prompt Engineers jam at Newspeak House. Brilliant to meet folks who want to experiment with the new technologies, and a great opportunity to muck out with Semantic Kernel. Can't recommend highly enough!";
            var ask = $"Reverse the following and then deliver it as cockney rhyming slag in all caps: {input}";

            // create plan for the given ask
            var plan = await kernel.CreatePlanAsync(planner, ask);
            Console.WriteLine($"Plan:\n\n{plan.Variables.ToPlan().PlanString}");

            // execute the plan
            var result = await kernel.ExecutePlanAsync(planner, plan);
            Console.WriteLine($"Result:\n\n{result}");
        }

        /// <summary>
        /// Retrieves .env files, loads them as environment variables, reads the environment into config.
        /// </summary>
        /// <returns>The configuration</returns>
        private static IConfigurationRoot RetrieveConfiguration()
        {
            DotEnv.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env"));
            DotEnv.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".secret.env"));
            return new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        /// Builds a kernel, configured with details of the model to use.
        /// </summary>
        /// <param name="config">Kernel configuration</param>
        /// <returns>The new configured kernel</returns>
        private static IKernel BuildAndConfigureKernel(IConfigurationRoot config)
        {
            var kernel = Kernel.Builder.Build();
            kernel.Config.AddOpenAITextCompletionService(
                config["serviceId"]!,
                config["modelId"]!,
                config["apiKey"]!);
            return kernel;
        }
    }
}
