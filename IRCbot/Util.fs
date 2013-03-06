
module IRCbot.Util

open System.Text.RegularExpressions

let (|Match|_|) (pat:string) (inp:string) =
    let m = Regex.Match(inp, pat) in
    if m.Success then
        Some (List.tail [for g in m.Groups -> g.Value])
    else None

type Message =
    | PING of string
    | ERROR of string                       // description
    | NOTICE of string
    | COMMAND of string * string * string   // number, who, text
    | PRIVMSG of string * string * string   // user, dest, content
    | JOIN of string * string               // user, channel
    | QUIT of string * string               // user, reason
    | OTHER

let (|Message|_|) (line: string) =
    match line with
    | Match @"PING :(.+)$" [what] -> Some(PING(what))
    | Match @"ERROR :(.+)$" [desc] -> Some(ERROR(desc))
    | Match @":[^ ]+ NOTICE [^ ]+ :(.+)$" [text] -> Some(NOTICE(text))
    | Match @":[^ ]+ (\d\d\d) ([^:]+)(?: :(.+))?$" [number; who; text] -> Some(COMMAND(number, who, text))
    | Match @":([^!]+)[^ ]+ PRIVMSG ([^ ]+) :(.+)$" [user; dest; content] -> Some(PRIVMSG(user, dest, content))
    | Match @":([^!]+)[^ ]+ JOIN ([^ ]+)$" [sender; channel] -> Some(JOIN(sender, channel))
    | Match @":([^!]+)[^ ]+ QUIT :(.+)$" [user; reason] -> Some(QUIT(user, reason))
    | _ -> None

// Formatting
  
module Format =

    let BOLD = "\x02"

    let UNDERLINE = "\x1f"

    let RESET = "\x0f"
    
    let colors = [| "white"; "black"; "blue"; "green"; "red"; "brown"; "purple"; "orange"; "yellow"; "light green"; "teal"; "cyan"; "light blue"; "pink"; "grey"; "light grey" |]
    
    let COLOR fore =
        match (colors |> Array.tryFindIndex (fun c -> c = fore)) with
        | Some(i) -> (sprintf "\x03%x" i)
        | _ -> RESET
        
    let BGCOLOR fore back =
        let color = COLOR fore
        match (colors |> Array.tryFindIndex (fun c -> c = fore)) with
        | Some(i) -> color + (sprintf ",%x" i)
        | _ -> RESET
