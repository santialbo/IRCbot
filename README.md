IRCbot
=======

IRCbot is a small IRC bot implementation which handles all the TCP stuff. You just have to define a message handler function which will be called uupon each message received by the TCP client. See the usage example for clarification.

Usage:
------
```FSharp
open IRCbot.Bot
open IRCbot.Util

let msgHandler (line: string) (write: string -> unit) =

    let say channel text = write (sprintf "PRIVMSG %s :%s" channel text)
    let ping what = write (sprintf "PONG :%s" what)
    
    match line with
    | Match @"^PING :(.+)$" [what] -> ping what
    | Match @"^:[^:]+JOIN (#[^\s]+)$" [channel] -> say channel "Hello world!"
    | _ -> ()

let bot = new SimpleBot("irc.quakenet.org", 6667, "itnas2", ["#holaquetalsoycolosal"; "#holaquetalsoycolosal2"], msgHandler)
bot.Start()
```
