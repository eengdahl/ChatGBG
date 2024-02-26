using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IMyChatEndPoint
{
   
    ChatRequest DefaultChatRequestArgs { get; set; }

    Conversation CreateConversation(MyChatRequest defaultChatRequestArgs = null);
    Task<MyChatResult> CreateChatCompletionAsync(MyChatRequest request);


    /// numOutputs Overrides  ChatRequest.NumChoicesPerMessage as a convenience.
    Task<MyChatResult> CreateChatCompletionAsync(MyChatRequest request, int numOutputs = 5);

    Task<MyChatResult> CreateChatCompletionAsync(IList<MyChatMessage> messages, Model model = null, double? temperature = null, double? top_p = null, int? numOutputs = null, int? max_tokens = null, double? frequencyPenalty = null, double? presencePenalty = null, IReadOnlyDictionary<string, float> logitBias = null, params string[] stopSequences);

    Task<MyChatResult> CreateChatCompletionAsync(params MyChatMessage[] messages);


    Task<MyChatResult> CreateChatCompletionAsync(params string[] userMessages);

    Task StreamChatAsync(MyChatRequest request, Action<MyChatResult> resultHandler);

    IAsyncEnumerable<MyChatResult> StreamChatEnumerableAsync(MyChatRequest request);

    IAsyncEnumerable<MyChatResult> StreamChatEnumerableAsync(IList<MyChatMessage> messages, Model model = null, double? temperature = null, double? top_p = null, int? numOutputs = null, int? max_tokens = null, double? frequencyPenalty = null, double? presencePenalty = null, IReadOnlyDictionary<string, float> logitBias = null, params string[] stopSequences);

    Task StreamCompletionAsync(ChatRequest request, Action<int, MyChatResult> resultHandler);
}
