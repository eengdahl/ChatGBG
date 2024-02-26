using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace OpenAI_API.Chat
{
    /// Represents the Role of a ChatMessage  
    /// Typically, a conversation is formatted with a system message first, 
    //followed by alternating user and assistant messages.  
    public class ChatMessageRole : IEquatable<ChatMessageRole>
    {
        private ChatMessageRole(string value) { Value = value; }

        /// Gets the singleton instance of ChatMessageRole based on the string value.
        public static ChatMessageRole FromString(string roleName)
        {
            switch (roleName)
            {
                case "system":
                    return ChatMessageRole.System;
                case "user":
                    return ChatMessageRole.User;
                case "assistant":
                    return ChatMessageRole.Assistant;
                default:
                    return null;
            }
        }

        private string Value { get; }


        /// The system message helps set the behavior of the assistant. 
        public static ChatMessageRole System { get; } = new ChatMessageRole("system");

        /// The user messages help instruct the assistant.
        public static ChatMessageRole User { get; } = new ChatMessageRole("user");

        /// The assistant messages help store prior responses.
        public static ChatMessageRole Assistant { get; } = new ChatMessageRole("assistant");

        /// Gets the string value for this role to pass to the API
        public override string ToString()
        {
            return Value;
        }


       // If obj is null, the method returns false. ergo the same as it was constructed with 
        public override bool Equals(object obj)
        {
            return Value.Equals((obj as ChatMessageRole).Value);
        }

        /// Returns the hash code for this object
        /// returns A 32-bit signed integer hash code
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// Determines whether this instance and a specified object have the same value.
        /// other = The ChatMessageRole to compare to this instance
        /// returns =true if other's value is the same as this instance;
        //otherwise, false. If other is null, the method returns false
        public bool Equals(ChatMessageRole other)
        {
            return Value.Equals(other.Value);
        }

        /// Gets the string value for this role to pass to the API
        /// value= The ChatMessageRole to convert
        public static implicit operator String(ChatMessageRole value) { return value.Value; }

    }
}
