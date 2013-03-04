
module IRCbot.Main

open IRCbot

[<EntryPoint>]
let main args = 
    let conn = new Connection("irc.quakenet.org", 6667, "itnas2")
    let a = conn.Connect
    0

