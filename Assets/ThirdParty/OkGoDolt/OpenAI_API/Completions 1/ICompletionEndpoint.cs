using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI_API.Models;

namespace OpenAI_API.Completions
{
	/// An interface for CompletionEndpoint, for ease of mock testing, etc
	public interface ICompletionEndpoint
    {
        /// This allows you to set default parameters for every request, for example to set a default temperature or max tokens. 
        //For every request, if you do not have a parameter set on the request but do have it set here as a default, the request will automatically pick up the default value.
        CompletionRequest DefaultCompletionRequestArgs { get; set; }

        /// Ask the API to complete the prompt(s) using the specified request.  This is non-streaming, so it will wait until the API returns the full result.

        Task<CompletionResult> CreateCompletionAsync(CompletionRequest request);

 
        /// promptThe prompt to generate from
        /// modelThe model to use. You can use ModelsEndpoint.GetModelsAsync() to see all of your available models, or use a standard model like Model.DavinciText
        /// max_tokens How many tokens to complete to. Can return fewer if a stop sequence is hit.
        /// temperature What sampling temperature to use. Higher values means the model will take more risks. 
        /// top_p An alternative to sampling with temperature,
        /// numOutputs How many different choices to request for each prompt.
        /// presencePenalty The scale of the penalty applied if a token is already present at all. 
        /// frequencyPenalty The scale of the penalty for how often a token is used.  
        /// logProbs Include the log probabilities on the logprobs most likely tokens, which can be found in CompletionResult.Completions  
        /// echo Echo back the prompt in addition to the completion.
        /// stopSequences One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// <returns>Asynchronously returns the completion result.  Look in its see CompletionResult.Completions property for the completions.
        Task<CompletionResult> CreateCompletionAsync(string prompt,
            Model model = null,
            int? max_tokens = null,
            double? temperature = null,
            double? top_p = null,
            int? numOutputs = null,
            double? presencePenalty = null,
            double? frequencyPenalty = null,
            int? logProbs = null,
            bool? echo = null,
            params string[] stopSequences
        );

        /// Ask the API to complete the prompt(s) using the specified promptes, with other paramets being drawn from default values specified in DefaultCompletionRequestArgs if present. 
        ///prompts One or more prompts to generate from
        Task<CompletionResult> CreateCompletionAsync(params string[] prompts);

        /// Ask the API to complete the prompt(s) using the specified request and a requested number of outputs.  T
        /// request The request to send to the API.  This does not fall back to default values specified in DefaultCompletionRequestArgs
        /// numOutputs Overrides CompletionRequest.NumChoicesPerPrompt as a convenience.
        /// returns Asynchronously returns the completion result.  Look in its CompletionResult.Completions property for the completions, which should have a length equal to numOutputs
        Task<CompletionResult> CreateCompletionsAsync(CompletionRequest request, int numOutputs = 5);

        /// Ask the API to complete the prompt(s) using the specified request, and stream the results to the resultHandler as they come in.
        Task StreamCompletionAsync(CompletionRequest request, Action<int, CompletionResult> resultHandler);

        /// Ask the API to complete the prompt(s) using the specified request, and stream the results to the resultHandleras they come in.
        Task StreamCompletionAsync(CompletionRequest request, Action<CompletionResult> resultHandler);

        /// Ask the API to complete the prompt(s) using the specified request, and stream the results as they come in.
        /// request The request to send to the API.  This does not fall back to default values specified in DefaultCompletionRequestArgs
        /// An async enumerable with each of the results as they come in.
        IAsyncEnumerable<CompletionResult> StreamCompletionEnumerableAsync(CompletionRequest request);

        /// Ask the API to complete the prompt(s) using the specified parameters. 
        /// Any non-specified parameters will fall back to default values specified in DefaultCompletionRequestArgs
        /// prompt The prompt to generate from
        /// model The model to use. You can use ModelsEndpoint.GetModelsAsync() to see all of your available models, or use a standard model like Model.DavinciText
        /// max_tokens How many tokens to complete to. 
        /// temperature What sampling temperature to use.  
        /// top_p   An alternative to sampling with temperature,  
        /// numOutputs How many different choices to request for each prompt. 
        /// presencePenalty The scale of the penalty applied if a token is already present at all.   
        /// frequencyPenalty The scale of the penalty for how often a token is used.   
        /// logProbs  Include the log probabilities on the logprobs most likely tokens, which can be found inCompletionResult.Completions  
        /// echo Echo back the prompt in addition to the completion. 
        /// stopSequences One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence 
        IAsyncEnumerable<CompletionResult> StreamCompletionEnumerableAsync(string prompt,
            Model model = null,
            int? max_tokens = null,
            double? temperature = null,
            double? top_p = null,
            int? numOutputs = null,
            double? presencePenalty = null,
            double? frequencyPenalty = null,
            int? logProbs = null,
            bool? echo = null,
            params string[] stopSequences);

        /// Simply returns a string of the prompt followed by the best completion
        ///  request The request to send to the API.  This does not fall back to default values specified in  DefaultCompletionRequestArgs 
        /// <returns>A string of the prompt followed by the best completion 
        Task<string> CreateAndFormatCompletion(CompletionRequest request);


        /// Simply returns the best completion
        ///  prompt The prompt to complete 
        /// <returns>The best completion 
        Task<string> GetCompletion(string prompt);
    }
}