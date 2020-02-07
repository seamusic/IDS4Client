using IdentityModel.Client;
using System;
using System.Net.Http;

namespace ConsoleClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //admin
            //Pa$$word123
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //注意，如果添加scope，不能添加无效的scope
            var request = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "consoleclient",
                ClientSecret = "secret"
            };
            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(request);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.ReadKey();
        }
    }
}
