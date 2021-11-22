using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Secondary1.Dto;
using Secondary1.Repositories;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Secondary1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        public IConfiguration Configuration { get; private set; }
        public LogController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult(LogRepository.Context));
        }

        [HttpPost]
        public async Task<IActionResult> Append([FromBody]LogDto dto)
        {
            await Task.Delay((int)Configuration.GetValue(typeof(int), "Delay"));
            if (LogRepository.Context.TryAdd(dto.Id, dto.Message + $". Receive at: {DateTime.Now}"))
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
