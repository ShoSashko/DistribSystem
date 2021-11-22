using MasterNode.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MasterNode.Services
{
    public class ReplicationService
    {
        private readonly ILogger _logger;

        public HttpClient Client { get; }

        public ReplicationService(HttpClient client, ILogger<ReplicationService> logger)
        {
            Client = client;
            _logger = logger;
        }

       
        
        public async Task AppendMessageIfNotYetAppended(LogDto message, Dictionary<string, (string NodeName, bool IsExecuted)> secondariesConfig, int w)
        {
            var taskList = new List<Task>();

            var countDownEvent = new CountdownEvent(w);

            foreach (var secondaryConfig in secondariesConfig)
            {
                if (!secondaryConfig.Value.IsExecuted)
                {
                    taskList.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var result = await Client.PostAsJsonAsync($"{secondaryConfig.Key}/log", message);
                            if (result.IsSuccessStatusCode)
                            {
                                secondariesConfig[secondaryConfig.Key] = (secondaryConfig.Value.NodeName, true);
                                _logger.LogInformation($"Received OK from {secondaryConfig.Value.NodeName}");
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
                        finally
                        {
                            countDownEvent.Signal();
                        }
                    }));
                }
            }

            var allTask = Task.WhenAll(taskList);
            while (countDownEvent.CurrentCount > 0)
            {
                await Task.Delay(500);
                _logger.LogInformation("Delayed 0.5 seconds");
            }
            //ALSO working example:
            //while (!allTask.IsCompleted)
            //{
            //    if (w <= 1)
            //    {
            //        break;
            //    }
            //    var completedTaskIndex = Task.WaitAny(taskList.ToArray());
            //    taskList.RemoveAt(completedTaskIndex);
            //    w--;
            //}
        }
    }
}
