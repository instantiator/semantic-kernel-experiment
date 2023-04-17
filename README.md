# semantic-kernel-experiment

The [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/whatissk) is a .NET SDK that simplifies a number of machine learning tasks:

![](https://learn.microsoft.com/en-us/semantic-kernel/media/fullview.png)

This solution is fully commented, and illustrates a simple uses of the Semantic Kernel:

* Define 1 or more skills, each with 1 or more functions
* Create a plan from an ask presented to the Kernel
* Execute that plan, to get to a final result

_NB. The solution is currently set up to use text completion services from OpenAI. Semantic Kernel can also access similar services through Azure OpenAI - you'll need to modify the code a little to do that._

## Configuration

Config is stored in 2 files:

* `SKE/.env` - configuration specifying `serviceId` and `modelId`
* `SKE/.secret.env` - configuration specifying your secret `apiKey`

You'll need to create `SKE/.secret.env` file and provide your API key from OpenAI.

## Getting started

* Place an API key from your OpenAI subscription in `.secret.env`

  ```env
  apiKey=<your-key-goes-here>
  ```

* Now run the application



## Skills, Functions, and the Planner

You can define any number of [semantic functions](https://learn.microsoft.com/en-us/semantic-kernel/howto/semanticfunctions) and [native functions](https://learn.microsoft.com/en-us/semantic-kernel/howto/nativefunctions), and these are bundled into groups called skills.

Each skill is found in its own directory, and in this solution these are all grouped inside the `Skills/` directory.

You can invoke functions directly through the Kernel, or use the Planner - a special skill that can determine which functions to use to fulfil an ask you make of it.

`KernelSkillsExtensions.cs` provides a number of helpful utility methods for importing skills, importing the planner, and then using it to create a plan and execute it.

## See also

* [microsoft/semantic-kernel](https://github.com/microsoft/semantic-kernel) (GitHub repository)
