namespace Parallelism

module samples =
    open System
    open System.IO
    open System.Net
    open System.Threading
    open System.Diagnostics

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

    type Utility() =
        static let rand = new Random()

        static member RandomSleep() =
            let ms = rand.Next(1, 10)
            Thread.Sleep ms

    // An implementation of a shared counter using locks and mutable variables (normal way in C#)
    type LockedCounter() =
        static let _lock = new Object()

        static let mutable count = 0
        static let mutable sum = 0

        static let updateState i =
            sum <- sum + i
            count <- count + 1
            printfn "Count is: %i. Sum is: %i" count sum

            // emulate a short delay
            Utility.RandomSleep()

        // Public interface to hide the state
        static member Add i =
            let stopwatch = new Stopwatch()
            stopwatch.Start()

            // Same as C# lock { ... }
            lock _lock (fun () ->
                stopwatch.Stop()
                printfn "Client waited %i ms" stopwatch.ElapsedMilliseconds
                updateState i
                )

    // A functional approach to the above, using an agent to serialize the inputs and a recursive function to update the state
    type MessageBasedCounter() =
        static let updateState (count, sum) msg =
            let newSum = sum + msg
            let newCount = count + 1
            printfn "Count is: %i. Sum is: %i" newCount newSum

            Utility.RandomSleep()

            (newCount, newSum) // Return the new state

        static let agent = MailboxProcessor.Start(fun inbox ->
            // the message processing function
            let rec messageLoop oldState = async {
                let! msg = inbox.Receive()
                let newState = updateState oldState msg
                return! messageLoop newState
                }
            messageLoop (0,0)
            )
        
        // Public interface to hide the implementation
        static member Add i = agent.Post i

    let makeCountingTask addFunction taskId = async {
        let name = sprintf "Task%i" taskId
        for i in [1..3] do
            addFunction i
    }
