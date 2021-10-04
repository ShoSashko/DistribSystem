using MasterNode.Dto;
using MasterNode.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Append(LogDto dto)
        {
            var message = $"Added log {DateTime.Now} with message: {dto.Message}";
            var logDto = new LogDto
            {
                Message = message
            };
            LogList.Add(message);
            await _logService.AppendMessage(logDto);
            return Ok();
        }
    }
}
