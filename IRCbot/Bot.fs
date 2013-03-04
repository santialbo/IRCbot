
namespace IRCbot

open System
open System.IO
open System.Net
open System.Net.Sockets

type Connection(server: string, port: int, nick: string) =
    
    let client = new TcpClient()
    
    let getMessage (line: string) = line.Substring(line.IndexOf(":", 1) + 1) 
    
    member x.Connect =
        client.Connect(server, port)
        let writer = new StreamWriter(client.GetStream())
        let reader = new StreamReader(client.GetStream())
        writer.AutoFlush <- true
        
        // Connection to the server
        writer.WriteLine (sprintf "USER %s %s %s %s" nick nick nick nick)
        writer.WriteLine (sprintf "NICK %s" nick)
        
        let rec connect_ = fun () ->
            let line = reader.ReadLine()
            printfn "%s" line
            if line.StartsWith "PING :"  then
                // If server pings we pong
                writer.WriteLine (sprintf "PONG %s" (line.Substring 5))
            elif (getMessage line).StartsWith "Welcome" then
                () // If server welcomes we are connected.
            else connect_()
        do connect_()
        
        (writer, reader)
           
