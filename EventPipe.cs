using System.Collections.Generic;

namespace FestivalKit
{
	public class EventPipe
	{
		protected SortedList<EventOrder, List<ListenerMethod>> Callbacks { get; } = new SortedList<EventOrder, List<ListenerMethod>>();

		public void Add(ListenerMethod listenerMethod)
		{
			var order = listenerMethod.Order;
			lock (Callbacks)
			{
				List<ListenerMethod> methods;
				if (!Callbacks.TryGetValue(order, out methods))
				{
					methods = new List<ListenerMethod>(5);
					Callbacks[order] = methods;
				}

				methods.Add(listenerMethod);
			}
		}

		public void Remove(ListenerMethod listenerMethod)
		{
			lock (Callbacks)
			{
				foreach (var order in Callbacks.Values)
				{
					order.RemoveAll(m =>
						m.CallbackMethod.Equals(listenerMethod.CallbackMethod) &&
						ReferenceEquals(m.Listener, listenerMethod.Listener));
				}
			}
		}
		
		public void Run(IEvent e)
		{
			lock (Callbacks)
			{
				var callbacks = Callbacks.Values;
				for (int j = 0; j < callbacks.Count; ++j)
				{
					var methods = callbacks[j];
					for (int i = 0; i < methods.Count; ++i)
						methods[i].Invoke(e);
				}
			}
		}
	}
}