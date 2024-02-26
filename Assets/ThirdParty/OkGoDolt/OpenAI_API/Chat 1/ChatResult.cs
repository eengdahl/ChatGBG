using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI_API.Chat
{
	/// Represents a result from calling the Chat API
	public class ChatResult : ApiResultBase
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
		/// <returns>The content of the message, not including <see cref="ChatMessageRole"/>.</returns>
		public override string ToString()
		{
			if (Choices != null && Choices.Count > 0)
				return Choices[0].ToString();
			else
				return null;
		}
	}

	/// A message received from the API, including the message text, index, and reason why the message finished.
	public class ChatChoice
	{
		/// The index of the choice in the list of choices
		[JsonProperty("index")]
		public int Index { get; set; }

		/// The message that was presented to the user as the choice
		[JsonProperty("message")]
		public MyChatMessage Message { get; set; }

		/// The reason why the chat interaction ended after this choice was presented to the user
		[JsonProperty("finish_reason")]
		public string FinishReason { get; set; }

		/// Partial message "delta" from a stream. For example, the result from <see cref="ChatEndpoint.StreamChatEnumerableAsync(ChatRequest)">StreamChatEnumerableAsync.</see>
		/// If this result object is not from a stream, this will be null
		[JsonProperty("delta")]
		public MyChatMessage Delta { get; set; }

		/// A convenience method to return the content of the message in this response
		/// <returns>The content of the message in this response, not including <see cref="ChatMessageRole"/>.</returns>
		public override string ToString()
		{
			return Message.Content;
		}
	}

	/// How many tokens were used in this chat message.
	public class ChatUsage : Usage
	{
		/// The number of completion tokens used during the chat interaction
		[JsonProperty("completion_tokens")]
		public int CompletionTokens { get; set; }
	}
}
