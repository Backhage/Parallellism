namespace Parallellism

open System
open System.IO
open System.Net

module samples =
    // The fetch functions are only used for showing the time difference
    // between running requests sequentially vs in parallel, they do not
    // return anything useful.
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