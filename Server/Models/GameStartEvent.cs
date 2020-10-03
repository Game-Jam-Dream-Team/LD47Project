namespace Server.Models {
	public sealed class GameStartEvent {
		public int    Id        { get; set; }
		public string SessionId { get; set; }
	}
}