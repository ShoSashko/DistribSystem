﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Secondary1.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task<IActionResult> Append([FromBody]LogDto dto)
        {
            Thread.Sleep(5000);
            LogList.Add(dto.Message);
            
            return Ok();
        }
    }
}
