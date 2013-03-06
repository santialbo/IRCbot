
module IRCbot.Util

open System.Text.RegularExpressions

let (|Match|_|) (pat:string) (inp:string) =
    let m = Regex.Match(inp, pat) in
    if m.Success then
        Some (List.tail [for g in m.Groups -> g.Value])
    else None

type Message =
    | PING of string
    | NOTICE of string
    | COMMAND of string * string * string   // number, who, text
    | PRIVMSG of string * string * string   // user, dest, content
    | JOIN of string * string               // user, channel
    | QUIT of string * string               // user, reason
    | OTHER

let (|Message|_|) (line: string) =
    match line with
    | Match @"PING :(.+)$" [what] -> Some(PING(what))
    | Match @":[^ ]+ NOTICE [^ ]+ :(.+)$" [text] -> Some(NOTICE(text))
    | Match @":[^ ]+ (\d\d\d) ([^:]+)(?: :(.+))?$" [number; who; text] -> Some(COMMAND(number, who, text))
    | Match @":([^!]+)[^ ]+ PRIVMSG ([^ ]+) :(.+)$" [user; dest; content] -> Some(PRIVMSG(user, dest, content))
    | Match @":([^!]+)[^ ]+ JOIN ([^ ]+)$" [sender; channel] -> Some(JOIN(sender, channel))
    | Match @":([^!]+)[^ ]+ QUIT :(.+)$" [user; reason] -> Some(QUIT(user, reason))
    | _ -> None
