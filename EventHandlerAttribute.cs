using System;

namespace Festival
{
	[AttributeUsage(AttributeTargets.Method)]
	public class EventHandlerAttribute : Attribute
	{
		public EventOrder Order { get; }

		public EventHandlerAttribute(EventOrder order = EventOrder.Third)
		{
			Order = order;
		}
	}
}