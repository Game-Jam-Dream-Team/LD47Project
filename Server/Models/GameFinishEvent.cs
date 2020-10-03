namespace Server.Models {
	public sealed class GameFinishEvent {
		public int    Id        { get; set; }
		public string SessionId { get; set; }
		public string Variant   { get; set; }
	}
}