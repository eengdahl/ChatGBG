using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OpenAI_API
{
    /// Represents authentication to the OpenAPI API endpoint
    public class APIAuthentication
    {
        /// The API key, required to access the API endpoint.
        public string ApiKey { get; set; }
        /// The Organization ID to count API requests against.  This can be found at https://beta.openai.com/account/org-settings.
        public string OpenAIOrganization { get; set; }

        /// Allows implicit casting from a string, so that a simple string API key can be provided in place of an instance of APIAuthentication
        /// key The API key to convert into a APIAuthentication

        public static implicit operator APIAuthentication(string key)
        {
            return new APIAuthentication(key);
        }

        /// Instantiates a new Authentication object with the given apiKey, which may be langword="null".
        /// apiKey The API key, required to access the API endpoint.
        public APIAuthentication(string apiKey)
        {
            this.ApiKey = apiKey;
        }


        /// Instantiates a new Authentication object with the given apiKey, which may be langword=null 
        /// apiKey The API key, required to access the API endpoint
        /// openAIOrganization The Organization ID to count API requests against.
        public APIAuthentication(string apiKey, string openAIOrganization)
        {
            this.ApiKey = apiKey;
            this.OpenAIOrganization = openAIOrganization;
        }

        private static APIAuthentication cachedDefault = null;

        /// The default authentication to use when no other auth is specified. 
        public static APIAuthentication Default
        {
            get
            {
                if (cachedDefault != null)
                    return cachedDefault;

                APIAuthentication auth = LoadFromEnv();
                if (auth == null)
                    auth = LoadFromPath();
                if (auth == null)
                    auth = LoadFromPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

                cachedDefault = auth;
                return auth;
            }
            set
            {
                cachedDefault = value;
            }
        }
        //returns api loaded from env
        public static APIAuthentication LoadFromEnv()
        {
            string key = Environment.GetEnvironmentVariable("OPENAI_KEY");

            if (string.IsNullOrEmpty(key))
            {
                key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

                if (string.IsNullOrEmpty(key))
                    return null;
            }

            string org = Environment.GetEnvironmentVariable("OPENAI_ORGANIZATION");

            return new APIAuthentication(key, org);
        }


        public static APIAuthentication LoadFromPath(string directory = null, string filename = ".openai", bool searchUp = true)
        {
            if (directory == null)
                directory = Environment.CurrentDirectory;

            string key = null;
            string org = null;
            var curDirectory = new DirectoryInfo(directory);

            while (key == null && curDirectory.Parent != null)
            {
                if (File.Exists(Path.Combine(curDirectory.FullName, filename)))
                {
                    var lines = File.ReadAllLines(Path.Combine(curDirectory.FullName, filename));
                    foreach (var l in lines)
                    {
                        var parts = l.Split('=', ':');
                        if (parts.Length == 2)
                        {
                            switch (parts[0].ToUpper())
                            {
                                case "OPENAI_KEY":
                                    key = parts[1].Trim();
                                    break;
                                case "OPENAI_API_KEY":
                                    key = parts[1].Trim();
                                    break;
                                case "OPENAI_ORGANIZATION":
                                    org = parts[1].Trim();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (searchUp)
                {
                    curDirectory = curDirectory.Parent;
                }
                else
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(key))
                return null;

            return new APIAuthentication(key, org);
        }


        /// Tests the api key against the OpenAI API, to ensure it is valid.  This hits the models endpoint so should not be charged for usage.
        public async Task<bool> ValidateAPIKey()
        {
            if (string.IsNullOrEmpty(ApiKey))
                return false;

            var api = new OpenAIAPI(this);

            List<Models.Model> results;

            try
            {
                results = await api.Models.GetModelsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

            return (results.Count > 0);
        }

    }

    internal static class AuthHelpers
    {
        /// A helper method to swap out langword="null see cref=APIAuthentication objects with the  cref="APIAuthentication.Default authentication,
        //possibly loaded from ENV or a config file.
        public static APIAuthentication ThisOrDefault(this APIAuthentication auth)
        {
            if (auth == null)
                auth = APIAuthentication.Default;

            return auth;
        }
    }
}
