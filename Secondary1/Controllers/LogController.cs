using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Secondary1.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secondary1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private static readonly ConcurrentDictionary<int, string> LogDict = new ConcurrentDictionary<int, string>();

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult(LogDict));
        }

        [HttpPost]
        public async Task<IActionResult> Append([FromBody]LogDto dto)
        {
            Thread.Sleep(10000);
            if (LogDict.TryAdd(dto.Id, dto.Message + $". Receive at: {DateTime.Now}"))
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, added {dto.Message}.");
            }
            else
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, could not add {dto.Message}. It's already added.");
            }
            
            return Ok();
        }
    }
}
