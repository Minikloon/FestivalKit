namespace Festival
{
	public class CancellableEvent : IEvent
	{
		public bool Cancelled { get; set; } 
	}
}