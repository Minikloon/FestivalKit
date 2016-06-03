using System;
using System.Reflection;

namespace FestivalKit
{
	public class ListenerMethod
	{
		public Type EventType { get; }
		public EventOrder Order { get; }
		public IListener Listener { get; }
		public MethodInfo CallbackMethod { get; }
		protected Action<object> Callback { get; }

		public ListenerMethod(Type eventType, EventOrder order, IListener listener, MethodInfo callbackMethod)
		{
			Listener = listener;
			Order = order;
			EventType = eventType;
			CallbackMethod = callbackMethod;
			Callback = CreateDelegate(Listener, CallbackMethod);
		}

		public void Invoke(IEvent e)
		{
			Callback(e);
		}

		// https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
		protected Action<object> CreateDelegate<T>(T instance, MethodInfo method) where T : class
		{
			var genericHelper = typeof(ListenerMethod).GetMethod("DelegateHelper", BindingFlags.Static | BindingFlags.NonPublic);
			var constructedHelper = genericHelper.MakeGenericMethod (method.GetParameters()[0].ParameterType);
			object ret = constructedHelper.Invoke(null, new object[] { instance, method });
			return (Action<object>) ret;
		}

		private static Action<object> DelegateHelper<TParam>(object instance, MethodInfo method)
		{
			Action<TParam> func = (Action<TParam>) Delegate.CreateDelegate(typeof(Action<TParam>), instance, method);
			Action<object> ret = param => func((TParam)param);
			return ret;
		}
	}
}