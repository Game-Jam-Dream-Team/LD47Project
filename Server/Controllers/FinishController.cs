using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class FinishController : ControllerBase {
		readonly ILogger     _logger;
		readonly GameContext _context;

		public FinishController(ILogger<FinishController> logger, GameContext context) {
			_logger  = logger;
			_context = context;
		}

		[HttpPost]
		public void Post(string sessionId, string variant) {
			_logger.LogInformation($"Finish: {sessionId}, {variant}");
			_context.FinishEvents.Add(new GameFinishEvent {
				SessionId = sessionId,
				Variant   = variant
			});
			_context.SaveChanges();
		}
	}
}