
module IRCbot.Main

open IRCbot
open IRCbot.Util

let msgHandler (line: string) (write: string -> unit) =

    let say channel text = write (sprintf "PRIVMSG %s :%s" channel text)
    let ping what = write (sprintf "PONG :%s" what)
    let join channel = write (sprintf "JOIN %s" channel)
    
    match line with
    | Match @"^PING :(.+)$" [what] -> ping what
    | Match @"^:[^:]+JOIN (#[^\s]+)$" [channel] -> say channel "Hello world!"
    | _ -> ()

[<EntryPoint>]
let main args = 
    let bot = new SimpleBot("irc.quakenet.org", 6667, "itnas2", ["#holaquetalsoycolosal"; "#holaquetalsoycolosal2"], msgHandler)
    bot.Start()
    0
