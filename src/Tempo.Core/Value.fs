namespace Tempo

module Value =
    type Value<'S, 'V> =
        | Literal of 'V
        | Derived of ('S -> 'V)

    type Value =
        static member Of<'V>(v: 'V) = Literal v
        static member Of<'S, 'V>(f: 'S -> 'V) = Derived f
        static member Of<'S>() = Derived id<'S>

        static member Resolve<'S, 'V> (v: Value<'S, 'V>) (s: 'S) =
            match v with
            | Literal v -> v
            | Derived f -> f s

        static member Map<'S, 'V1, 'V2> (map: 'V1 -> 'V2) (v: Value<'S, 'V1>) : Value<'S, 'V2> =
            match v with
            | Literal v -> Literal <| map v
            | Derived f -> Derived(f >> map)

        static member MapState<'S1, 'S2, 'V> (map: 'S1 -> 'S2) (v: Value<'S2, 'V>) : Value<'S1, 'V> =
            match v with
            | Literal v -> Literal v
            | Derived f -> Derived(map >> f)

        static member inline MapOption<'S, 'V1, 'V2>
            (map: 'V1 -> 'V2)
            (v: Value<'S, 'V1 option>)
            : Value<'S, 'V2 option> =
            Value.Map<'S, 'V1 option, 'V2 option>(fun v -> Option.map map v) v

        static member Combine<'S, 'A, 'B, 'C>(f: 'A -> 'B -> 'C, va: Value<'S, 'A>, vb: Value<'S, 'B>) : Value<'S, 'C> =
            match (va, vb) with
            | (Literal a, Literal b) -> Literal <| f a b
            | (Derived fa, Derived fb) -> Derived <| fun s -> f (fa s) (fb s)
            | (Literal a, Derived fb) -> Derived <| fun s -> f a (fb s)
            | (Derived fa, Literal b) -> Derived <| fun s -> f (fa s) b

        static member Sequence<'S, 'V>(ls: List<Value<'S, 'V>>) : Value<'S, List<'V>> =
            Derived(fun s -> List.map (fun v -> Value.Resolve v s) ls)
