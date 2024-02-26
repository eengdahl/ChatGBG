using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Embedding;
using OpenAI_API.Files;
using OpenAI_API.Images;
using OpenAI_API.Models;
using OpenAI_API.Moderation;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http.Headers;



namespace OpenAI_API
{
    /// Entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
    public class OpenAIAPI : IOpenAIAPI
    {
        /// for OpenAI, should be "https://api.openai.com/{0}/{1}"
        /// for Azure, should be "https://(your-resource-name.openai.azure.com/openai/deployments/(deployment-id)/{1}?api-version={0}"
        public string ApiUrlFormat { get; set; } = "https://api.openai.com/{0}/{1}";


        public string ApiVersion { get; set; } = "v1";

        public APIAuthentication Auth { get; set; }


        /// Creates a new entry point to the OpenAPI API, handling auth and allowing access to the various API endpoints
        /// apiKeys The API authentication information to use for API calls, orlangword="null to attempt to use the APIAuthentication.Default 
        //potentially loading from environment vars or from a config file
        public OpenAIAPI(APIAuthentication apiKeys = null)
        {
            this.Auth = apiKeys.ThisOrDefault();
            Completions = new CompletionEndpoint(this);
            Models = new ModelsEndpoint(this);
            Files = new FilesEndpoint(this);
            Embeddings = new EmbeddingEndpoint(this);
            Chat = new ChatEndpoint(this);
            Moderation = new ModerationEndpoint(this);
            ImageGenerations = new ImageGenerationEndpoint(this);
        }

        /// Instantiates a version of the API for connecting to the Azure OpenAI endpoint instead of the main OpenAI endpoint.
        /// YourResourceName The name of your Azure OpenAI Resource
        /// eploymentId The name of your model deployment. You're required to first deploy a model before you can make calls.
        /// apiKey The API authentication information to use for API calls, or langword="null  to attempt to use the APIAuthentication.Default
        //potentially loading from environment vars or from a config file.  Currently this library only supports the api-key flow, not the AD-Flow
        public static OpenAIAPI ForAzure(string YourResourceName, string deploymentId, APIAuthentication apiKey = null)
        {
            OpenAIAPI api = new OpenAIAPI(apiKey);
            api.ApiVersion = "2022-12-01";
            api.ApiUrlFormat = $"https://{YourResourceName}.openai.azure.com/openai/deployments/{deploymentId}/" + "{1}?api-version={0}";
            return api;
        }

        /// Text generation is the core function of the API. You give the API a prompt, and it generates a completion. 
        //The way you “program” the API to do a task is by simply describing the task in plain english or providing a few written examples. 
        //This simple approach works for a wide range of use cases, including summarization, translation, grammar correction, question answering, chatbots, composing emails
        public ICompletionEndpoint Completions { get; }

        /// The API lets you transform text into a vector (list) of floating point numbers. The distance between two vectors measures their relatedness. 
        //Small distances suggest high relatedness and large distances suggest low relatedness.
        public IEmbeddingEndpoint Embeddings { get; }

        /// Text generation in the form of chat messages. This interacts with the ChatGPT API.
        public IChatEndpoint Chat { get; }

        /// Classify text against the OpenAI Content Policy.
        public IModerationEndpoint Moderation { get; }

        /// The API endpoint for querying available Engines/models
        public IModelsEndpoint Models { get; }

        /// The API lets you do operations with files. You can upload, delete or retrieve files. Files can be used for fine-tuning, search, etc.
        public IFilesEndpoint Files { get; }

        /// The API lets you do operations with images. Given a prompt and/or an input image, the model will generate a new image.
        public IImageGenerationEndpoint ImageGenerations { get; }
    }
}
