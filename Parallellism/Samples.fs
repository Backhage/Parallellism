namespace Parallellism

open System
open System.IO
open System.Net

module samples =
    let fetchUrl url =
        let req = WebRequest.Create(Uri(url))
        use resp = req.GetResponse()
        use stream = resp.GetResponseStream()
        use reader = new IO.StreamReader(stream)
        let html = reader.ReadToEnd()
        printfn "Finished downloading %s" url

    let fetchUrlAsync url =
        async {
            let req = WebRequest.Create(Uri(url))
            use! resp = req.AsyncGetResponse()
            use stream = resp.GetResponseStream()
            use reader = new IO.StreamReader(stream)
            let html = reader.ReadToEnd()
            printfn "Finshed downloading %s" url
        }