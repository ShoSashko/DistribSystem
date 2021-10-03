using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MasterNode.Services
{
    public class LogService
    {
        public HttpClient Client { get; }

        public LogService(HttpClient client)
        {
            client.BaseAddress = new Uri("http://web:80");
            Client = client;
        }

        public async Task AppendMessage(string message)
        {
            var response = await Client.PostAsJsonAsync($"/log?message={message}", string.Empty);
        }
    }
}
