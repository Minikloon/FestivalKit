# ![asd](http://i.imgur.com/gdID576.png) FestivalKit 
[![Build status](https://ci.appveyor.com/api/projects/status/f1qkojjfjwpg107f?svg=true)](https://ci.appveyor.com/project/Minikloon/festivalkit)

FestivalKit is a small and simple C# eventing library aimed to be familiar to Bukkit developers. 
This can also be useful in a general purpose plugin architecture where unrelated components need to step on each other in a somewhat orderly fashion.
The library is available under [MIT License](https://tldrlegal.com/license/mit-license).

# Install

FestivalKit is available on NuGet.
~~~~
Install-Package FestivalKit.dll
~~~~

# Usage

First let's declare an event type.
~~~~
public class SomethingEvent : IEvent
{
  public string Text { get; set; }
}
~~~~

Then declare a listener for that type.

A Listener class can have multiple EventHandler methods.
~~~~
public class SomethingListener : IListener
{
  [EventHandler]
  public void OnSomething(SomethingEvent e)
  {
    Console.WriteLine(e.Text);
  }
}
~~~~

Finally use a dispatcher to register your listener and dispatch your events.

*You usually only need a single dispatcher in your application.*

~~~~
var dispatcher = new EventDispatcher();
dispatcher.Register(listener);

dispatcher.Dispatch(new SomethingEvent() {Text = "Hello World!"});
~~~~


# EventOrder

You can specify an order to a listener method. 
The order is between 1 and 7, the default is 3. The highest order is executed last.

~~~~
public class SomethingListener : IListener
{
	[EventHandler(EventOrder.Second)]
	public void OnSomething(SomethingEvent e)
	{
		Console.WriteLine($"This is written first: {e.Text}");
	}
	
	[EventHandler(EventOrder.Fifth)]
	public void OnSomething2(SomethingEvent e)
	{
		Console.WriteLine($"This is written second: {e.Text}");
	}
}
~~~~


# Event Cancellation


Some events within your application may be cancellable by its listeners. For example if an event is fired before broadcasting a chat 
message in an IRC server, a word filter listener might want to cancel it. In turn an user whitelist listener might permit the message if it is from an admin user and un-cancel it.


To use this facility, simply derive your event class from CancellableEvent.

~~~~
public class ChatEvent : CancellableEvent
{
	public string Username { get; set; }
	public string Message { get; set; }
}
~~~~

Then listeners can toggle the Cancelled boolean property of your event.
~~~~
public class WhitelistListener : IListener
{
	[EventHandler(EventOrder.Sixth)]
	public void OnChat(ChatEvent e)
	{
		if (e.Username == "Minikloon")
			e.Cancelled = false;
	}
}
~~~~

You can also use EventDispatcher's Dispatch overload for CancellableEvent with this nifty pattern.
~~~~
var e = new ChatEvent() {Username = "Minikloon", Message = "Yo!"};
if (dispatcher.Dispatch(e))
{
	Server.Broadcast($"{e.Username}: {e.Message}");
}
~~~~
