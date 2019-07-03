# broadcast-http
A simple, feature-light, thread-safe, performant HTTP and WebSocket library for Unity

## FAQ

### Why use this instead of another, competing HTTP library for Unity? 

Here's the thing. .NET 4.7 includes everything under the hood that you need to do HTTP and WebSockets. 
You probably don't much of a library at all. You certainly don't need to spend big bucks. 
You can make it all work with just System.Net.HttpClient and System.Net.WebSocket. 

However, the default system libraries come with just enough weird gotchas that, if you do use them, 
you're going to run into a bunch of subtle pitfalls, which I have already run into for you.

Let me save you about 9 days of subtle pitfalls. 

Broadcast HTTP is:

* Completely open-source. You can look at the source right now, if you want to. 
* Built using .NET system primitives that do 90% of the work, so the library is extremely minimal.
* Opinionated - if you're building your software along the golden path of "trading JSON messages around" 
   it's very easy to use.
* Opinionated - if you're building your software to communicate entirely through subtle variations of 
   .PNG data, it's very hard to use
* Extensively and aggressively documented.
* Thread-safe - it doesn't matter how many writes you queue up at the same time, they all go into the 
   same queue and are executed one-at-a-time like good little requests.

### Does this library do caching or serialization for me, automatically? 

No. Why would it do that? Write your own cache and serialization logic.

### Does this library work in .NET 3.5?

No, it does not!

### Does it Support Socket.io or SignalR?

It does not! Only vanilla websockets! 

### Can I Use It In My Game?

Only if your game is released under an Open Source license. 

This library, as it stands on GitHub, is released as AGPLv3.0 &mdash; with all of the terrible restrictions that implies.

If you would like to use it in a commercial, closed-source product, you'll need to buy it through 
the Unity Store.
