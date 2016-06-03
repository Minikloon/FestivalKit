using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Festival
{
	public class MethodExtractor
	{
		public virtual List<ListenerMethod> ExtractListenerMethods(IListener listener)
		{
			var listenerMethods = new List<ListenerMethod>(5);
			foreach (var method in listener.GetType().GetMethods())
			{
				var listenerMethod = GetListenerMethod(listener, method);
				if (listenerMethod == null) continue;
				listenerMethods.Add(listenerMethod);
			}

			return listenerMethods;
		}

		protected ListenerMethod GetListenerMethod(IListener o, MethodInfo method)
		{
			EventHandlerAttribute attribute;
			if (!TryGetListenerAttribute(method, out attribute))
				return null;
			var order = attribute.Order;

			if (method.ReturnType != typeof(void))
				throw new Exception("Listener method must return void");

			var parameters = method.GetParameters();
			if (!parameters.Any() || !typeof(IEvent).IsAssignableFrom(parameters[0].ParameterType))
				throw new Exception("Can't register listener method without an event parameter");
			var eventType = parameters[0].ParameterType;
			
			return new ListenerMethod(eventType, order, o, method);
		}

		protected bool TryGetListenerAttribute(MethodInfo method, out EventHandlerAttribute attribute)
		{
			attribute = null;
			var attributes = (EventHandlerAttribute[])method.GetCustomAttributes(typeof(EventHandlerAttribute), true);
			if (!attributes.Any())
				return false;

			attribute = attributes[0];
			return true;
		}
	}
}