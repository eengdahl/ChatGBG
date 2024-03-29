using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class MyChatMessage
{
    // Creates an empty ChatMessage
    public MyChatMessage()
    {
        this.Role = ChatMessageRole.User;
    }
//Constructs a new mychatmessage with role and content provided
    public MyChatMessage(ChatMessageRole role, string content)
    {
        this.Role = role;
        this.Content = content;
    }

    [JsonProperty("role")]
    internal string rawRole { get; set; }

    /// The role of the message, which can be "system", "assistant" or "user"
    [JsonIgnore]
    public ChatMessageRole Role
    {
        get
        {
            return ChatMessageRole.FromString(rawRole);
        }
        set
        {
            rawRole = value.ToString();
        }
    }


    [JsonProperty("content")]
    public string Content { get; set; }


    [JsonProperty("name")]
    public string Name { get; set; }

}

public class MyChatRequest
{
    [JsonProperty("model")]
    public string Model { get; } = OpenAI_API.Models.Model.ChatGPTTurbo;

    [JsonProperty("messages")]
    public IList<MyChatMessage> Messages { get; set; }

    [JsonProperty("temperature")]
    public double? Temperature = 0.2f;

    [JsonProperty("top_p")]
    public double? TopP { get; set; }

    [JsonProperty("n")]
    public int? NumChoicesPerMessage = 1;

    /// Specifies where the results should stream and be returned at one time. 
    //thank the guys at JustGoDoIT for this part  
    [JsonProperty("stream")]
    public bool Stream { get; internal set; } = false;


    /// This is only used for serializing the request into JSON, 
    [JsonProperty("stop")]
    internal object CompiledStop
    {
        get
        {
            if (MultipleStopSequences?.Length == 1)
                return StopSequence;
            else if (MultipleStopSequences?.Length > 0)
                return MultipleStopSequences;
            else
                return null;
        }
    }

    /// One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
    [JsonIgnore]
    public string[] MultipleStopSequences { get; set; }

    /// The stop sequence where the API will stop generating further tokens. The returned text will not contain the stop sequence. 
    //For convenience, if you are only requesting a single stop sequence, set it here
    [JsonIgnore]
    public string StopSequence
    {
        get => MultipleStopSequences?.FirstOrDefault() ?? null;
        set
        {
            if (value != null)
                MultipleStopSequences = new string[] { value };
        }
    }

    /// How many tokens to complete to. Can return fewer if a stop sequence is hit.  Defaults to 16.
    [JsonProperty("max_tokens")]
    public int? MaxTokens = 16;

    /// The scale of the penalty for how often a token is used.  Should generally be between 0 and 1, 
    [JsonProperty("frequency_penalty")]
    public double? FrequencyPenalty { get; set; }


    /// The scale of the penalty applied if a token is already present at all.  
    [JsonProperty("presence_penalty")]
    public double? PresencePenalty { get; set; }

    /// Modify the likelihood of specified tokens appearing in the completion.
    /// Accepts a json object that maps tokens(specified by their token ID in the tokenizer) to an associated bias value from -100 to 100.
    /// Mathematically, the bias is added to the logits generated by the model prior to sampling.
    /// The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection
    [JsonProperty("logit_bias")]
    public IReadOnlyDictionary<string, float> LogitBias { get; set; }

    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    [JsonProperty("user")]
    public string user { get; set; }



    /// Creates a new, empty ChatRequest
    public MyChatRequest()
    { }

    /// Create a new chat request using the data from the input chat request.
    public MyChatRequest(MyChatRequest basedOn)
    {
        if (basedOn == null)
            return;

      //  this.Model = basedOn.Model;
        this.Messages = basedOn.Messages;
        this.Temperature = basedOn.Temperature;
        this.TopP = basedOn.TopP;
        this.NumChoicesPerMessage = basedOn.NumChoicesPerMessage;
        this.MultipleStopSequences = basedOn.MultipleStopSequences;
        this.MaxTokens = basedOn.MaxTokens;
        this.FrequencyPenalty = basedOn.FrequencyPenalty;
        this.PresencePenalty = basedOn.PresencePenalty;
        this.LogitBias = basedOn.LogitBias;
    }
}

public class MyConversation
{
    /// Allows setting the parameters to use when calling the ChatGPT API.  Can be useful for setting temperature, presence_penalty, and more.
    public MyChatEndPoint _endpoint;
    public MyChatRequest RequestParameters { get; private set; }
    public OpenAI_API.Models.Model Model;
    public MyChatResult MostResentAPIResult { get; private set; }

    public MyConversation(MyChatEndPoint endpoint, OpenAI_API.Models.Model model = null, MyChatRequest defaultChatRequestArgs = null)
    {
        RequestParameters = new MyChatRequest(defaultChatRequestArgs);


        _Messages = new List<MyChatMessage>();
        _endpoint = endpoint;
        RequestParameters.NumChoicesPerMessage = 1;
        RequestParameters.Stream = false;
    }

    public IReadOnlyList<MyChatMessage> Messages { get => _Messages; }
    private List<MyChatMessage> _Messages;

    public void AppendMessage(MyChatMessage message)
    {
        _Messages.Add(message);
    }
    //compiles the message for HTTP uses
    public void AppendMessage(ChatMessageRole role, string content) => this.AppendMessage(new MyChatMessage(role, content));

    public void AppendUserInput(string content) => this.AppendMessage(new MyChatMessage(ChatMessageRole.User, content));

    public void AppendUserInputWithName(string userName, string content) => this.AppendMessage(new MyChatMessage(ChatMessageRole.User, content) { Name = userName });

    public void AppendSystemMessage(string content) => this.AppendMessage(new MyChatMessage(ChatMessageRole.System, content));

    public void AppendExampleChatbotOutput(string content) => this.AppendMessage(new MyChatMessage(ChatMessageRole.Assistant, content));

    //MainFunction
    public async Task<string> GetResponseFromChatbotAsync()
    {
        MyChatRequest req = new MyChatRequest(RequestParameters);
        req.Messages = _Messages.ToList();

        var res = await _endpoint.CreateChatCompletionAsync(req);
        MostResentAPIResult = res;

        if (res.Choices.Count > 0)
        {
            var newMsg = res.Choices[0].Message;
            //  AppendMessage(newMsg);
            return newMsg.Content;
        }
        return null;
    }

    public Task<string> GetResponseFromChatbot() => GetResponseFromChatbotAsync();

    public async Task StreamResponseFromChatbotAsync(Action<string> resultHandler)
    {
        await foreach (string res in StreamResponseEnumerableFromChatbotAsync())
        {
            resultHandler(res);
        }
    }

    public async Task StreamResponseFromChatbotAsync(Action<int, string> resultHandler)
    {
        int index = 0;
        await foreach (string res in StreamResponseEnumerableFromChatbotAsync())
        {
            resultHandler(index++, res);
        }
    }


    public async IAsyncEnumerable<string> StreamResponseEnumerableFromChatbotAsync()
    {
        MyChatRequest req = new MyChatRequest(RequestParameters);
        req.Messages = _Messages.ToList();

        StringBuilder responseStringBuilder = new StringBuilder();
        ChatMessageRole responseRole = null;

        await foreach (var res in _endpoint.StreamChatEnumerableAsync(req))
        {
            if (res.Choices.FirstOrDefault()?.Delta is MyChatMessage delta)
            {
                if (delta.Role != null)
                    responseRole = delta.Role;

                string deltaContent = delta.Content;

                if (!string.IsNullOrEmpty(deltaContent))
                {
                    responseStringBuilder.Append(deltaContent);
                    yield return deltaContent;
                }
            }
            MostResentAPIResult = res;
        }

        if (responseRole != null)
        {
            AppendMessage(responseRole, responseStringBuilder.ToString());
        }
    }

}