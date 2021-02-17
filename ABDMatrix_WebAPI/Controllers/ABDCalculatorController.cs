using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using ABDMatrix_WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ABDMatrix_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ABDCalculatorController : ControllerBase
    {
        private readonly ABDMatrix_Calculator _calculator;

        public ABDCalculatorController(ABDMatrix_Calculator Calculator)
        {
            _calculator = Calculator;
        }

        [HttpPost]
        public IActionResult ABDCalculator([FromBody] Laminate laminate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (laminate is null || laminate.Material is null || laminate.Plies is null)
                return BadRequest();

            var results = Newtonsoft.Json.JsonConvert.SerializeObject(_calculator.Calculate_ABDMatrix(laminate.Plies, laminate.Material, out double laminateThickness), Newtonsoft.Json.Formatting.Indented);
            Request.HttpContext.Response.Headers.Add("Laminate_Thickness", laminateThickness.ToString());

            return Ok(results);
        }
    }
}
