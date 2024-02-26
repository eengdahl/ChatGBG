using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MyChatEndPoint : HTTPCalls
{
    public MyChatRequest DefaultChatRequestArgs { get; set; } = new MyChatRequest();

      // string Endpoint= "chat/completions";

    public MyConversation CreateConversation(MyChatRequest defaultChatRequestArgs = null)
    {
        return new MyConversation(this, defaultChatRequestArgs: defaultChatRequestArgs ?? DefaultChatRequestArgs);
    }

    public async Task<MyChatResult> CreateChatCompletionAsync(MyChatRequest request)
    {
        return await HttpPost<MyChatResult>(postData: request);
    }

    public Task<MyChatResult> CreateChatCompletionAsync(MyChatRequest request, int numOutputs = 5)
    {
        request.NumChoicesPerMessage = numOutputs;
        return CreateChatCompletionAsync(request);
    }

    public Task<MyChatResult> CreateChatCompletionAsync(IList<MyChatMessage> messages,
       // Model model = null,
        double? temperature = null,
        double? top_p = null,
        int? numOutputs = null,
        int? max_tokens = null,
        double? frequencyPenalty = null,
        double? presencePenalty = null,
        IReadOnlyDictionary<string, float> logitBias = null,
        params string[] stopSequences)
    {
        MyChatRequest request = new MyChatRequest(DefaultChatRequestArgs)
        {
            Messages = messages,
            Temperature = temperature ?? DefaultChatRequestArgs.Temperature,
            TopP = top_p ?? DefaultChatRequestArgs.TopP,
            NumChoicesPerMessage = numOutputs ?? DefaultChatRequestArgs.NumChoicesPerMessage,
            MultipleStopSequences = stopSequences ?? DefaultChatRequestArgs.MultipleStopSequences,
            MaxTokens = max_tokens ?? DefaultChatRequestArgs.MaxTokens,
            FrequencyPenalty = frequencyPenalty ?? DefaultChatRequestArgs.FrequencyPenalty,
            PresencePenalty = presencePenalty ?? DefaultChatRequestArgs.PresencePenalty,
            LogitBias = logitBias ?? DefaultChatRequestArgs.LogitBias
        };
        return CreateChatCompletionAsync(request);
    }

    public Task<MyChatResult> CreateChatCompletionAsync(params MyChatMessage[] messages)
    {
        MyChatRequest request = new MyChatRequest(DefaultChatRequestArgs)
        {
            Messages = messages
        };
        return CreateChatCompletionAsync(request);
    }

    public Task<MyChatResult> CreateChatCompletionAsync(params string[] userMessages) => CreateChatCompletionAsync(userMessages.Select(m => new MyChatMessage(ChatMessageRole.User, m)).ToArray());

    public IAsyncEnumerable<MyChatResult> StreamChatEnumerableAsync(MyChatRequest request)
    {
        request = new MyChatRequest(request) { Stream = true };
        return HttpStreamingRequest<MyChatResult>(Url, HttpMethod.Post, request);
    }
    public IAsyncEnumerable<MyChatResult> StreamChatEnumerableAsync(IList<MyChatMessage> messages,
        Model model = null,
        double? temperature = null,
        double? top_p = null,
        int? numOutputs = null,
        int? max_tokens = null,
        double? frequencyPenalty = null,
        double? presencePenalty = null,
        IReadOnlyDictionary<string, float> logitBias = null,
        params string[] stopSequences)
    {
        MyChatRequest request = new MyChatRequest(DefaultChatRequestArgs)
        {
            Messages = messages,
            Temperature = temperature ?? DefaultChatRequestArgs.Temperature,
            TopP = top_p ?? DefaultChatRequestArgs.TopP,
            NumChoicesPerMessage = numOutputs ?? DefaultChatRequestArgs.NumChoicesPerMessage,
            MultipleStopSequences = stopSequences ?? DefaultChatRequestArgs.MultipleStopSequences,
            MaxTokens = max_tokens ?? DefaultChatRequestArgs.MaxTokens,
            FrequencyPenalty = frequencyPenalty ?? DefaultChatRequestArgs.FrequencyPenalty,
            PresencePenalty = presencePenalty ?? DefaultChatRequestArgs.PresencePenalty,
            LogitBias = logitBias ?? DefaultChatRequestArgs.LogitBias
        };
        return StreamChatEnumerableAsync(request);
    }
}


public class MyChatResult : ApiResultBase
{
    /// The identifier of the result, which may be used during troubleshooting
    [JsonProperty("id")]
    public string Id { get; set; }

    /// The list of choices that the user was presented with during the chat interaction
    [JsonProperty("choices")]
    public IReadOnlyList<ChatChoice> Choices { get; set; }

    /// The usage statistics for the chat interaction
    [JsonProperty("usage")]
    public ChatUsage Usage { get; set; }

    /// A convenience method to return the content of the message in the first choice of this response
    /// returns The content of the message, not including ChatMessageRole
    public override string ToString()
    {
        if (Choices != null && Choices.Count > 0)
            return Choices[0].ToString();
        else
            return null;
    }
}
