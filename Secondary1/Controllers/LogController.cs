using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Secondary1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private static readonly List<string> LogList = new List<string>();

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult(LogList));
        }

        [HttpPost]
        public async Task<IActionResult> Append(string message)
        {
            LogList.Add(message);
            return Ok();
        }
    }
}
