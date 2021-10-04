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
            var secondariesConfig = new Dictionary<string, string>();

#if DEBUG
            secondariesConfig = new Dictionary<string, string>()
            {
                { "http://localhost:5011", "Secondary1" },
                { "http://localhost:5022", "Secondary2" },
            };

#else
            secondariesConfig = new Dictionary<string, string>()
            {
                { "http://secondary1:80", "Secondary1" },
                { "http://secondary2:80", "Secondary2" },
            };
#endif
            var taskList = new List<Task>();
            foreach (var secondaryConfig in secondariesConfig)
            {
                taskList.Add(Task.Run(async () =>
                {
                    try
                    {
                        var result = await Client.PostAsJsonAsync($"{secondaryConfig.Key}/log", message);
                        if (result.IsSuccessStatusCode)
                        {
                            _logger.LogInformation($"Received OK from {secondaryConfig.Value}");
                        }
                        else
                        {
                            _logger.LogError($"Response failed: {result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Error occurred while sending request {e.Message}");
                    }

                }));
            }
            await Task.WhenAll(taskList);
        }
    }
}
