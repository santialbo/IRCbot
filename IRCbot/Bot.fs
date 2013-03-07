
module IRCbot.Bot

open System
open System.IO
open System.Net.Sockets

open IRCbot.Util

type Connection(server: string, port: int, nick: string) =
    
    let client = new TcpClient()
    
    member this.Start() =
        client.Connect(server, port)
        let reader = new StreamReader(client.GetStream())
        let writer = new StreamWriter(client.GetStream())
        writer.AutoFlush <- true
        
        let read = fun () ->
            let line = reader.ReadLine()
            printfn "< %s" line
            line
        
        let write (line: string) =
            writer.WriteLine line
            printfn "> %s" line
            
        let connected = fun () -> not reader.EndOfStream
        
        (read, write, connected)
    
    member x.Close() =
        client.Close()
        

type SimpleBot(server: string, port: int, nick: string, channels: string list,
               msgHandler: string -> (string -> unit) -> unit) =
    
    let rec connect (read: unit -> string) (write: string -> unit) =
        write (sprintf "USER %s %s %s %s" nick nick nick nick)
        write (sprintf "NICK %s" nick)
        
        let rec connect_ = fun () ->
            match read() with
            | Message (PING(what)) -> write (sprintf "PONG %s" what); connect_() // If server pings we pong
            | Message (COMMAND("001", _,  _)) -> // If server welcome we join the channels
                channels |> List.iter (fun channel -> write (sprintf "JOIN %s" channel))
            | Message (COMMAND("433", _,  _)) -> // If nickname in use keep trying
                connect read write
            | _ -> connect_()
        do connect_()

    member this.Start = fun () ->
        let conn = new Connection(server, port, nick)
        let read, write, connected = conn.Start()
        connect read write
        
        while (connected()) do
            let line = read()
            match line with
            | Message (PING(what)) -> write (sprintf "PONG :%s" what)
            | Message (ERROR(desc)) -> conn.Close(); this.Start()
            | _ -> msgHandler line write
