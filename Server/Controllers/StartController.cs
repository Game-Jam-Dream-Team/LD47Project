using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class StartController : ControllerBase {
		readonly ILogger     _logger;
		readonly GameContext _context;

		public StartController(ILogger<StartController> logger, GameContext context) {
			_logger  = logger;
			_context = context;
		}

		[HttpPost]
		public void Post(string sessionId) {
			_logger.LogInformation($"Post: {sessionId}");
			_context.StartEvents.Add(new GameStartEvent {
				SessionId = sessionId
			});
			_context.SaveChanges();
		}
	}
}