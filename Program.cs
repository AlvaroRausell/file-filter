using LLama;
using LLama.Common;
using LLama.Sampling;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: file-filter <path> [-f \"filter prompt\"] [-o \"output file\"]");
            return;
        }

        string inputPath = args[0];
        string filterPrompt = "";
        string outputPath = "";
        string modelPath = @"models/DeepSeek-R1-0528-Qwen3-8B-IQ4_XS.gguf";
        
        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] == "-f" && i + 1 < args.Length)
            {
                filterPrompt = args[i + 1];
                i++; // Skip the next argument since we consumed it
            }
            else if (args[i] == "-o" && i + 1 < args.Length)
            {
                outputPath = args[i + 1];
                i++; // Skip the next argument since we consumed it
            }
        }

        if (!Directory.Exists(inputPath) && !File.Exists(inputPath))
        {
            Console.WriteLine($"Error: Path '{inputPath}' does not exist.");
            return;
        }

        Console.WriteLine($"Input path: {inputPath}");
        if (!string.IsNullOrEmpty(filterPrompt))
        {
            Console.WriteLine($"Filter prompt: {filterPrompt}");
        }
        if (!string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine($"Output path: {outputPath}");
        }


        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024, // The longest length of chat as memory.
            GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        // Add chat histories as prompt to tell AI how to act.
        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, $"You are a text classifier. You must evaluate if the current line matches the filter criteria and end the response with 'Final Answer: True' or 'Final Answer: False'");
        chatHistory.AddMessage(AuthorRole.User, $"Filter prompt: Find all entries that include animals. Current line: duck");
        chatHistory.AddMessage(AuthorRole.Assistant, $"Final Answer: True");
        chatHistory.AddMessage(AuthorRole.User, $"Filter prompt: Find all entries that include animals. Current line: house");
        chatHistory.AddMessage(AuthorRole.Assistant, $"Final Answer: False");
        chatHistory.AddMessage(AuthorRole.User, $"Filter prompt: Find all entries that include colors. Current line: red car");
        chatHistory.AddMessage(AuthorRole.Assistant, $"Final Answer: True");
        chatHistory.AddMessage(AuthorRole.User, $"Filter prompt: Find all entries that include colors. Current line: broken window");
        chatHistory.AddMessage(AuthorRole.Assistant, $"Final Answer: False");
        ChatSession session = new(executor, chatHistory);

        InferenceParams inferenceParams = new InferenceParams()
        {
            MaxTokens = 256, // Very short response - just true or false
            AntiPrompts = new List<string> { "User:" }, // Stop generation at newline or User:
            SamplingPipeline = new DefaultSamplingPipeline(),
        };

        using StreamReader streamReader = new(inputPath);
        string? line;
        List<string> matches = new();
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            string result = "";
            Console.WriteLine($"Inputting: {line}");

            // Format the input consistently with training examples
            string userMessage = string.IsNullOrEmpty(filterPrompt)
                ? $"Current line: {line}"
                : $"Filter prompt: {filterPrompt}. Current line: {line}";

            await foreach (var text in session.ChatAsync(
                new ChatHistory.Message(AuthorRole.User, userMessage),
                inferenceParams))
            {
                result += text;
            }
            session.RemoveLastMessage();
            session.RemoveLastMessage();
            // Clean up the result to extract just true/false
            result = result.Trim().ToLower();

            // Extract true/false from the response
            bool isMatch;
            if (result.Contains("final answer: true"))
            {
                isMatch = true;
            }
            else if (result.Contains("final answer: false"))
            {
                isMatch = false;
            }
            else
            {
                // If response is unclear, default to false and show warning
                Console.WriteLine($"Warning: Unclear response '{result}' for line: {line}");
                isMatch = false;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Model response: {result}");
            Console.ResetColor();

            if (isMatch)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ MATCH: {line}");
                matches.Add(line);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ NO MATCH: {line}");
            }
            Console.ResetColor();
        }
        
        // Output results
        Console.WriteLine($"\nFound {matches.Count} matches:");
        foreach(var match in matches) {
            Console.WriteLine(match);
        }
        
        // Write to output file if specified
        if (!string.IsNullOrEmpty(outputPath))
        {
            try
            {
                await File.WriteAllLinesAsync(outputPath, matches);
                Console.WriteLine($"\nResults written to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to output file: {ex.Message}");
            }
        }
    }

}