namespace Tempo.Std

module Function =
    type F =
        static member MergeEffects(a: unit -> unit, b: unit -> unit) : unit -> unit =
            fun () ->
                a ()
                b ()

        static member MergeEffects(a: 'T -> unit, b: 'T -> unit) : 'T -> unit =
            fun (v: 'T) ->
                a v
                b v
