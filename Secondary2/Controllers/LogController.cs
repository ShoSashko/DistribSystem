using Microsoft.AspNetCore.Mvc;
using Secondary2.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Secondary2.Controllers
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
            LogList.Add(dto.Message);
            
            return Ok();
        }
    }
}
