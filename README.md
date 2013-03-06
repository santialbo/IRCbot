IRCbot
=======

IRCbot is a small IRC bot implementation which handles all the TCP stuff. You just have to define a message handler function which will be called uupon each message received by the TCP client. See the usage example for clarification.

Usage:
------
```FSharp
open IRCbot.Bot
open IRCbot.Util

let server = "irc.quakenet.org"
let port = 6667
let nick = "Botijo"
let channels = ["#testchannel"]

let msgHandler (line: string) (write: string -> unit) =

    let say channel text = write (sprintf "PRIVMSG %s :%s" channel text)
    
    match line with
    | Message (JOIN(user, channel)) when user = nick -> say channel "Hello world!"
    | _ -> ()
    
let bot = new SimpleBot(server, port, nick, channels, msgHandler)
bot.Start()
```
