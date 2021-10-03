using MasterNode.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterNode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private static readonly List<string> LogList = new List<string>();
        private readonly LogService _logService;
        public LogController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult(LogList));
        }

        [HttpPost]
        public async Task<IActionResult> Append()
        {
            var message = $"Added log {DateTime.Now}";
            LogList.Add(message);
            await _logService.AppendMessage(message);
            return Ok();
        }
    }
}
