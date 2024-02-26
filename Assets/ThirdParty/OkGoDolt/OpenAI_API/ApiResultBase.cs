using Newtonsoft.Json;
using OpenAI_API.Models;
using System;

namespace OpenAI_API
{
	/// Represents a result from calling the OpenAI API, with all the common metadata returned from every endpoint
	abstract public class ApiResultBase
	{
		[JsonIgnore]
		public DateTime? Created => CreatedUnixTime.HasValue ? (DateTime?)(DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime.Value).DateTime) : null;

		/// The time when the result was generated in unix epoch format
		[JsonProperty("created")]
		public long? CreatedUnixTime { get; set; }

		/// Which model was used to generate this result.
		[JsonProperty("model")]
		public Model Model { get; set; }

		/// Object type, ie: text_completion, file, fine-tune, list, etc
		[JsonProperty("object")]
		public string Object { get; set; }

		/// The organization associated with the API request, as reported by the API.
		[JsonIgnore]
		public string Organization { get; internal set; }

		/// The server-side processing time as reported by the API.  This can be useful for debugging where a delay occurs.
		[JsonIgnore]
		public TimeSpan ProcessingTime { get; internal set; }

		/// The request id of this API call, as reported in the response headers.  This may be useful for troubleshooting or when contacting OpenAI support in reference to a specific request.
		[JsonIgnore]
		public string RequestId { get; internal set; }

		[JsonIgnore]
		public string OpenaiVersion { get; internal set; }
	}
}