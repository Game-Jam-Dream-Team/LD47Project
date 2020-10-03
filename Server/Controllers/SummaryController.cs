using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Dto;
using Server.Models;

namespace Server.Controllers {
	[ApiController]
	[Route("[controller]")]
	public sealed class SummaryController : ControllerBase {
		readonly ILogger     _logger;
		readonly GameContext _context;

		public SummaryController(ILogger<FinishController> logger, GameContext context) {
			_logger  = logger;
			_context = context;
		}

		[HttpGet]
		public async Task<Summary> Get() {
			var startEvents  = await _context.StartEvents.ToArrayAsync();
			var finishEvents = await _context.FinishEvents.ToArrayAsync();

			var summary = new Summary {
				Results = new Dictionary<string, int>()
			};
			var nonFinishedSessions = startEvents.Count(fe => finishEvents.All(se => fe.SessionId != se.SessionId));
			summary.Results[string.Empty] = nonFinishedSessions;
			var finishVariants = finishEvents.Select(fe => fe.Variant).Distinct().ToArray();
			foreach ( var finishVariant in finishVariants ) {
				summary.Results[finishVariant] = finishEvents.Count(fe => fe.Variant == finishVariant);
			}

			_logger.LogInformation(
				$"Returns summary: {string.Join("; ", summary.Results.Select(p => $"{p.Key} => {p.Value}"))}");
			return summary;
		}
	}
}