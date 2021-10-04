using MasterNode.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MasterNode.Services
{
    public class LogService
    {
        private readonly ILogger _logger;

        public HttpClient Client { get; }

        public LogService(HttpClient client, ILogger<LogService> logger)
        {
            Client = client;
            _logger = logger;
        }

        public async Task AppendMessage(LogDto message)
        {
            List<string> uris = new List<string>()
            {
                "http://secondary1:80",
                "http://secondary2:80",
            };

            //List<string> uris = new List<string>()
            //{
            //    "http://localhost:5012",
            //    "http://localhost:5022",
            //};

            var taskList = new List<Task>();
            foreach (var secondaryUri in uris)
            {
                taskList.Add(Client.PostAsJsonAsync($"{secondaryUri}/log", message));
            }
            try
            {
                await Task.WhenAll(taskList);
            }
            catch(Exception e)
            {
                _logger.LogError($"Error occurred while sending request {e.Message}");
            }
        }
    }
}
