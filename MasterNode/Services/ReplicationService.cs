using MasterNode.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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

        // sync
        public void AppendMessageIfNotYetAppended(LogDto message, Dictionary<string, (string NodeName, bool IsExecuted)> secondariesConfig, int w)
        {
            var taskList = new List<Task>();

            var countDownEvent = new CountdownEvent(w);

            foreach (var secondaryConfig in secondariesConfig)
            {
                taskList.Add(Task.Run(async () =>
                {
                    int currentRetry = 0;
                    for (; ; )
                    {
                        try
                        {
                            var cts = new CancellationTokenSource();
                            cts.CancelAfter(TimeSpan.FromSeconds(300));
                            var result = await Client.PostAsJsonAsync($"{secondaryConfig.Key}/log", message, cts.Token);
                            if (result.IsSuccessStatusCode)
                            {
                                secondariesConfig[secondaryConfig.Key] = (secondaryConfig.Value.NodeName, true);
                                _logger.LogInformation($"Received OK from {secondaryConfig.Value.NodeName}");
                                if (!countDownEvent.IsSet)
                                {
                                    countDownEvent.Signal();
                                }
                                break;
                            }
                            else
                            {
                                _logger.LogError($"Response failed: {result.ReasonPhrase}");
                                throw new Exception();
                            }
                        }
                        catch (TaskCanceledException e)
                        {
                            if (e.CancellationToken.IsCancellationRequested)
                            {
                                _logger.LogError("Cancellation requested");
                                break;
                            }
                            currentRetry++;
                            _logger.LogCritical($"Error occurred while sending request d with message {e.Message}.\n" +
                                $" Service Name {secondaryConfig.Value.NodeName}.\n" +
                                $" Retry Count: {currentRetry}" +
                                $" Timeout Delay: {Math.Pow(2, currentRetry * 1000)}");
                        }
                        catch (WebException e)
                        {
                            currentRetry++;
                            _logger.LogError($"Error occurred while sending request with f message {e.Message}.\n" +
                                $" Service Name {secondaryConfig.Value.NodeName}.\n" +
                                $" Retry Count: {currentRetry}" +
                                $" Timeout Delay: {Math.Pow(2, currentRetry * 1000)}");
                        }
                        catch (Exception e)
                        {
                            currentRetry++;
                            _logger.LogError($"Error occurred while sending request with message {e.Message}.\n" +
                                $" Service Name {secondaryConfig.Value.NodeName}.\n" +
                                $" Retry Count: {currentRetry}" +
                                $" Timeout Delay: {Math.Pow(2, currentRetry * 1000)}");
                        }
                        //2^1 = 2
                        //2^2 = 4
                        //2^3 = 8
                        _logger.LogError($"Delay: {Math.Pow(2, currentRetry * 1000)}");
                        await Task.Delay(currentRetry * 1000);
                    }
                }));
            }

            var allTask = Task.WhenAll(taskList);
            countDownEvent.Wait(100000);
        }
    }
}
