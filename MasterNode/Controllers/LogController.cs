using MasterNode.Dto;
using MasterNode.Repositories;
using MasterNode.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly ReplicationService _logService;

        public LogController(ReplicationService logService, IConfiguration configuration)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult(LogRepository.Context));
        }

        [HttpPost]
        public async Task<IActionResult> Append(LogDto dto)
        {
            var secondariesConfig = new Dictionary<string, (string, bool)>();
#if DEBUG
            secondariesConfig = new Dictionary<string, (string, bool)>()
            {
                { "http://localhost:5011", ("Secondary1", false) }
            };

#else
            secondariesConfig = new Dictionary<string, (string, bool)>()
            {
                { "http://secondary1:80", ("Secondary1", false) },
                { "http://secondary2:80", ("Secondary2", false) },
            };
#endif


            var message = $"Added log {DateTime.Now} with message: {dto.Message}";

            if (LogRepository.Context.TryAdd(LogRepository.Context.Count + 1, message))
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, added {message}.");
            }
            else
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, could not add {message}. It's already added.");
            }
            var logDto = new LogDto
            {
                Id = LogRepository.Context.Count,
                Message = message
            };

            _logService.AppendMessageIfNotYetAppended(logDto, secondariesConfig, dto.W - 1); 

            return Ok();
        }
    }
}
