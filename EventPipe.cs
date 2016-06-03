using System.Collections.Generic;

namespace FestivalKit
{
	public class EventPipe
	{
		protected SortedList<EventOrder, List<ListenerMethod>> CallbacksByOrder { get; } = new SortedList<EventOrder, List<ListenerMethod>>();

		public void Add(ListenerMethod listenerMethod)
		{
			var order = listenerMethod.Order;
			lock (CallbacksByOrder)
			{
				List<ListenerMethod> methods;
				if (!CallbacksByOrder.TryGetValue(order, out methods))
				{
					methods = new List<ListenerMethod>(5);
					CallbacksByOrder[order] = methods;
				}

				methods.Add(listenerMethod);
			}
		}

		public void Remove(ListenerMethod listenerMethod)
		{
			lock (CallbacksByOrder)
			{
				foreach (var order in CallbacksByOrder.Values)
				{
					order.RemoveAll(m =>
						m.CallbackMethod.Equals(listenerMethod.CallbackMethod) &&
						ReferenceEquals(m.Listener, listenerMethod.Listener));
				}
			}
		}
		
		public void Run(IEvent e)
		{
			LinkedList<ListenerMethod> toInvoke = new LinkedList<ListenerMethod>();
			lock (CallbacksByOrder)
			{
				var callbacks = CallbacksByOrder.Values;
				for (int j = 0; j < callbacks.Count; ++j)
				{
					var methods = callbacks[j];
					for (int i = 0; i < methods.Count; ++i)
						toInvoke.AddLast(methods[i]);
				}
			}
			foreach(var method in toInvoke)
				method.Invoke(e);
		}
	}
}