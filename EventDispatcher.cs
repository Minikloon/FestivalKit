using System;
using System.Collections.Generic;

namespace FestivalKit
{
	public class EventDispatcher
	{
		public MethodExtractor MethodExtractor { get; set; } = new MethodExtractor();

		protected readonly Dictionary<Type, EventPipe> Pipes = new Dictionary<Type, EventPipe>();

		public void Register(IListener listener)
		{
			var listenerMethods = MethodExtractor.ExtractListenerMethods(listener);
			lock (Pipes)
			{
				foreach (var method in listenerMethods)
				{
					EventPipe pipe;
					if (!Pipes.TryGetValue(method.EventType, out pipe))
					{
						pipe = new EventPipe();
						Pipes[method.EventType] = pipe;
					}
					pipe.Add(method);
				}
			}
		}

		public void Unregister(IListener listener)
		{
			var listenerMethods = MethodExtractor.ExtractListenerMethods(listener);
			lock (Pipes)
			{
				foreach (var method in listenerMethods)
				{
					EventPipe pipe;
					if (!Pipes.TryGetValue(method.EventType, out pipe)) continue;
					pipe.Remove(method);
				}
			}
		}

		public void Dispatch(IEvent e)
		{
			var type = e.GetType();
			EventPipe pipe;
			lock (Pipes)
			{
				if (!Pipes.TryGetValue(type, out pipe))
					return;
			}
			pipe.Run(e);
		}

		// returns true if execution can go on
		public bool Dispatch(CancellableEvent e)
		{
			Dispatch((IEvent) e);
			return !e.Cancelled;
		}
	}
}