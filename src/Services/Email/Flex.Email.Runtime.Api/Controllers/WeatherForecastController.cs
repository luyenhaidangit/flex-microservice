using Microsoft.AspNetCore.Mvc;
using FastReport;
using FastReport.Export.PdfSimple;
using Flex.Email.Runtime.Api.Models;

namespace Flex.Email.Runtime.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("InvestorReport")]
        public IActionResult GetInvestorReport()
        {
            var reportsFolder = "Report/";

            var report = new Report();
            var reportPath = @"C:\Users\Admin\Desktop\FlexTool\flex-microservice\src\Services\Email\Flex.Email.Runtime.Api\Reports\PersonsReport.frx";
            report.Load(reportPath);

            report.RegisterData(new Person().GetPersons(), "Persons");

            report.Prepare();

            var pdfExport = new PDFSimpleExport();

            pdfExport.Export(report, $"PersonsReport_{DateTime.Now.ToString("HHmmss")}.pdf");

            // Trả về file PDF
            return Ok("OKHEHE");
        }
    }
}
