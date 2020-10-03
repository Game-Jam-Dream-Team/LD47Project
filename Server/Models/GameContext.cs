using Microsoft.EntityFrameworkCore;

namespace Server.Models {
	public sealed class GameContext : DbContext {
		public DbSet<GameStartEvent>  StartEvents  { get; set; }
		public DbSet<GameFinishEvent> FinishEvents { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite("Data Source=game.db");
	}
}