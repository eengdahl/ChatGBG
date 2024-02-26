using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Embedding;
using OpenAI_API.Files;
using OpenAI_API.Images;
using OpenAI_API.Models;
using OpenAI_API.Moderation;

namespace OpenAI_API
{
	public interface IOpenAIAPI
    {
        /// Base url for OpenAI
        /// for OpenAI, should be "https://api.openai.com/{0}/{1}"
        string ApiUrlFormat { get; set; }

        /// Version of the Rest Api
        string ApiVersion { get; set; }

        /// The API authentication information to use for API calls
        APIAuthentication Auth { get; set; }

        /// Text generation in the form of chat messages. This interacts with the ChatGPT API.
        IChatEndpoint Chat { get; }

        /// Classify text against the OpenAI Content Policy.
        IModerationEndpoint Moderation { get; }

        /// Text generation is the core function of the API. You give the API a prompt, and it generates a completion. The way you “program” the API to do a task is by simply describing the task in plain english or providing a few written examples. This simple approach works for a wide range of use cases, including summarization, translation, grammar correction, question answering, chatbots, composing emails, and much more (see the prompt library for inspiration).
        ICompletionEndpoint Completions { get; }

        /// The API lets you transform text into a vector (list) of floating point numbers. The distance between two vectors measures their relatedness. Small distances suggest high relatedness and large distances suggest low relatedness.
        IEmbeddingEndpoint Embeddings { get; }

        /// The API endpoint for querying available Engines/models
        IModelsEndpoint Models { get; }

        /// The API lets you do operations with files. You can upload, delete or retrieve files. Files can be used for fine-tuning, search, etc.
        IFilesEndpoint Files { get; }

        /// The API lets you do operations with images. You can Given a prompt and/or an input image, the model will generate a new image.
        IImageGenerationEndpoint ImageGenerations { get; }
    }
}