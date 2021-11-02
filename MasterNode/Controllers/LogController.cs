using MasterNode.Dto;
using MasterNode.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace MasterNode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private static readonly ConcurrentDictionary<int, string> LogDict = new ConcurrentDictionary<int, string>();
        private readonly LogService _logService;
        public LogController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult(LogDict));
        }

        [HttpPost]
        public async Task<IActionResult> Append(LogDto dto)
        {
            var secondariesConfig = new Dictionary<string, (string, bool)>();

#if DEBUG
            secondariesConfig = new Dictionary<string, (string, bool)>()
            {
                { "http://localhost:5011", ("Secondary1", false) },
                { "http://localhost:5022", ("Secondary2", false) },
            };

#else
            secondariesConfig = new Dictionary<string, (string, bool)>()
            {
                { "http://secondary1:80", ("Secondary1", false) },
                { "http://secondary2:80", ("Secondary2", false) },
            };
#endif


            var message = $"Added log {DateTime.Now} with message: {dto.Message}";

            if (LogDict.TryAdd(LogDict.Count + 1, message))
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, added {message}.");
            }
            else
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, could not add {message}. It's already added.");
            }
            var logDto = new LogDto
            {
                Id = LogDict.Count,
                Message = message
            };

            await _logService.AppendMessageToNodeAsync(logDto, secondariesConfig, dto.W);
            _logService.AppendMessageIfNotYetAppended(logDto, secondariesConfig);

            return Ok();
        }
    }
}
