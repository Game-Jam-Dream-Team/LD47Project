using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Server.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class HelloWorldController : ControllerBase {
		readonly ILogger _logger;

		public HelloWorldController(ILogger<HelloWorldController> logger) {
			_logger = logger;
		}

		[HttpGet]
		public string Get() {
			_logger.LogInformation("Called");
			return "Hello World!";
		}
	}
}