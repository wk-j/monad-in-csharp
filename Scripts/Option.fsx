open System

type 't Options  = { 
    Value:  't
    HasValue: bool
 } with
    static member None = { HasValue = false ; Value = Unchecked.defaultof<'t> }
    static member Some<'t> (t: 't) = { HasValue = true ; Value = t }

[<Runtime.CompilerServices.Extension>]
type Extensions =
    [<Runtime.CompilerServices.Extension>]
    static member Unit(_: 'target Options, t: 'target) = Options.Some<'target>(t)

    [<Runtime.CompilerServices.Extension>]
    static member Bind(target: 'target Options, action: 'target  -> 'result Options) = 
        if target.HasValue then
            action(target.Value)
        else
            Options.None

    [<Runtime.CompilerServices.Extension>]
    static member SelectMany(target: 'source Options, projector: 'source -> 'result Options) = 
        target.Bind(projector)

let parseIntOption(str) =
    let ok,rs = Int32.TryParse(str)
    if ok then Options.Some(rs)
    else Options.None

let devideByOption(op, n) = 
    Options.Some(op / n)

let options = Options.Some("1000")
let rs = 
    options
        .Bind(fun str -> parseIntOption(str))
        .Bind(fun n2 -> devideByOption(n2, 3))

rs |> printfn "%A"