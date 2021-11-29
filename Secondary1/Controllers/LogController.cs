using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Secondary1.Dto;
using Secondary1.Repositories;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secondary1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        public static readonly ConcurrentDictionary<int, string> Buffer = new ConcurrentDictionary<int, string>();

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
            if (Buffer.TryAdd(dto.Id, dto.Message))
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, added {dto.Message}.");
            }
            else
            {
                Console.WriteLine($"Thread={Thread.CurrentThread.ManagedThreadId}, could not add {dto.Message}. It's already added.");
            }
            await Task.Delay((int)Configuration.GetValue(typeof(int), "Delay"));

            TryAddToContext();
            
            var random = new Random();
            if(random.Next() % 2 == 0)
            {
                throw new Exception();
            }
            
            return Ok();
        }

        void TryAddToContext()
        {
            int i = 1;
            while (LogRepository.Context.Count + Buffer.Count >= i)
            {
                if (LogRepository.Context.ContainsKey(i)){
                    i++;
                    continue;
                }
                else if(Buffer.ContainsKey(i))
                {
                    Buffer.TryRemove(i, out string value);
                    LogRepository.Context.TryAdd(i, value);
                }
                else
                {
                    break;
                }

                i++;
            }
        }
    }
}
