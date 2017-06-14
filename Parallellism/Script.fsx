#load "Samples.fs"
open Parallellism

let sites = ["http://www.google.com";
             "http://www.yahoo.com";
             "http://www.microsoft.com";
             "http://www.amazon.com";
             "http://www.bing.com"]

// Real: 00:00:02.636, CPU: 00:00:00.093, GC gen0: 1, gen1: 1, gen2: 0
#time
sites
|> List.map samples.fetchUrl
#time

// Real: 00:00:00.378, CPU: 00:00:00.031, GC gen0: 0, gen1: 0, gen2: 0 
#time
sites
|> List.map samples.fetchUrlAsync
|> Async.Parallel
|> Async.RunSynchronously
#time
