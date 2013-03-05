
module IRCbot.Util

open System.Text.RegularExpressions

let (|Match|_|) (pat:string) (inp:string) =
    let m = Regex.Match(inp, pat) in
    if m.Success then
        Some (List.tail [for g in m.Groups -> g.Value])
    else None