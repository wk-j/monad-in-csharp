open System

type 'target Options  = { 
    Value:  'target
    HasValue: bool
 } with
    static member None = { HasValue = false ; Value = Unchecked.defaultof<'target> }
    static member Some (t: 'target) = { HasValue = true ; Value = t }
    static member Unit(t: 'target) = Options.Some(t)

[<Runtime.CompilerServices.Extension>]
type Extensions =

    [<Runtime.CompilerServices.Extension>]
    static member Bind(target: 'target Options, action: Func<'target,'result Options>) = 
        if target.HasValue then
            action.Invoke(target.Value)
        else
            Options.None

    [<Runtime.CompilerServices.Extension>]
    static member SelectMany(target: 'source Options, projector: Func<'source,'result Options>) = 
        target.Bind(projector)

    [<Runtime.CompilerServices.Extension>]
    static member SelectMany(target: 'source Options, projector: Func<'source,'result Options>, selector: Func<'source,'option,'target>) = 
        target.Bind(fun source -> projector.Invoke(source).Bind(fun task -> Options.Unit(selector.Invoke(source, task))))

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