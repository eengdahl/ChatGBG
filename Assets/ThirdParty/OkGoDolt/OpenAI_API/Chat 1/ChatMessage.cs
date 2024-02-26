using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI_API.Chat
{
    /// Chat message sent or received from the API. Includes who is speaking in the "role" and the message text in the "content"
    public class ChatMessage
    {
        // Creates an empty ChatMessage, with Role defaulting to ChatMessageRole.User
        public ChatMessage()
        {
            this.Role = ChatMessageRole.User;
        }

        /// Constructor for a new Chat Message
        /// The role of the message, which can be "system", "assistant" or "user"
        ///content =The text to send in the message
        public ChatMessage(ChatMessageRole role, string content)
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

        /// The content of the message
        [JsonProperty("content")]
        public string Content { get; set; }

        /// An optional name of the user in a multi-user chat 
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
