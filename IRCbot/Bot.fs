
module IRCbot.Bot

open System
open System.IO
open System.Net.Sockets

open IRCbot.Util

type Connection(server: string, port: int, nick: string) =
    
    let client = new TcpClient()
    
    member this.Connect() =
        client.Connect(server, port)
        let reader = new StreamReader(client.GetStream())
        let writer = new StreamWriter(client.GetStream())
        writer.AutoFlush <- true
        
        let getLine = fun () ->
            let line = reader.ReadLine()
            printfn "< %s" line
            line
        
        let putLine (line: string) =
            writer.WriteLine line
            printfn "> %s" line
            
        do
            // Connect to the server
            putLine (sprintf "USER %s %s %s %s" nick nick nick nick)
            putLine (sprintf "NICK %s" nick)
            
            let rec connect_ = fun () ->
                match getLine() with
                | Message (PING(what)) -> putLine (sprintf "PONG %s" what); connect_() // If server pings we pong
                | Message (COMMAND("001", _,  _)) -> () // If server welcomes we are done
                | _ -> connect_()
            do connect_()
        
        let connected = fun () -> not reader.EndOfStream
        
        (getLine, putLine, connected)
    
    member x.Close() =
        client.Close()
        

type SimpleBot(server: string, port: int, nick: string, channels: string list,
               msgHandler: string -> (string -> unit) -> unit) =
    
    let conn = new Connection(server, port, nick)
    
    member this.Start = fun () ->
        let read, write, connected = conn.Connect()
        channels
        |> List.iter (fun channel -> write (sprintf "JOIN %s" channel))
        
        while (connected()) do
            msgHandler (read()) write


