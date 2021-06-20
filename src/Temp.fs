module Tempo.Core

type Value<'V, 'S> =
    | Literal of 'V
    | Derived of ('S -> 'V)

(*
3 projects

- 1 Modello : BEaaS: HTTP/GraphQL/Data File Storage/Proxy
- 2 Decision Matrix
- 3 Tempo


match state with
| Some v -> (1, DOMElement(name = "div"; text = s -> string), v)
| None -> (2, _ -> span("no luck"), ())


{ Image: Option<string> }


let teamplate =
    let (release, capture) = holdState(....)
    dom [] [
        capture [
            ...
            release [
                div [] []
            ]
        ]
    ]
}

div [] [
    bind (s ->
        match s.Image with
        | Some url -> (1, img [src <| derived url], url)
        | None -> (2, span [] [], ())
    )

    map<S, S1> (
        s -> s.Length,

    )

    oneOf2 ( // WIN!!!! fold, caseOf, case, oneOf2
       s -> Either<s, s>
       Template<T1>
       Template<T2>
    )

]

match

TEMPO V8

Problems:
- make tempo generic
- BindState -> OneOf
- Lifecycle
- Capture capture/release state

    TypeParamenters:
    'S -> State
    'A -> Action
    'Q -> Query

    'N -> Node for Virtual DOM (like HTML Element or Text Node descriptions)
    'I -> Implementation of the Virtual DOM (HTMLElement or TextNode instances)
*)

and MS<'S1, 'S2> = MS of ('S1 -> 'S2)

// and IMapState<'S1> =
//     abstract Map<'S2> : unit -> MS<'S1, 'S2>

and Template<'N, 'S, 'A, 'Q> =
    | Node of 'N
    | Fragment of Template<'N, 'S, 'A, 'Q> list
    | MapState of IMapState<'N, 'S, 'A, 'Q> // * Template<'N, 'S2, 'A, 'Q>
// | MapAction
// MapImpl
// BindState
// Query
// Lifecycle
// Capture capture/release state
// Sequence
// Component hold state, has update cycle
// Adapter???
// Lazy???
// When???/If/Else

and RenderNode<'N, 'I, 'S> = 'N -> 'I -> 'S -> View<'S, 'I>

and Render<'N, 'I, 'S, 'A, 'Q> = RenderNode<'N, 'I, 'S> -> Template<'N, 'S, 'A, 'Q> -> 'I -> 'S -> View<'S, 'I>

and View<'S, 'I> =
    { Impl: 'I
      Change: 'S -> unit
      Destroy: unit -> unit }

let mapState<'S1, 'S2> (f: 'S1 -> 'S2) =
    { new IMapState<'S1> with
        override this.Map() = (MS f) }

let resolve<'V, 'S> (value: Value<'V, 'S>) (state: 'S) =
    match value with
    | Literal v -> v
    | Derived f -> f state

let rec render<'N, 'I, 'S>
    (renderNode: RenderNode<'N, 'I, 'S>)
    (template: Template<'N, 'S, 'A, 'Q>)
    (impl: 'I)
    (state: 'S)
    : View<'S, 'I> =
    match template with
    | Node n -> renderNode n impl state
    | Fragment ls ->
        let ls =
            List.map (fun i -> render renderNode i impl state) ls

        { Impl = impl
          Change = fun state -> List.iter (fun i -> i.Change state) ls
          Destroy = fun () -> List.iter (fun i -> i.Destroy()) ls }
// | MapState ev ->
//     let (map, template) = ev.Eval()
//     render renderNode template impl <| map state
// | Embed ev ->
//     let (renderEmbedded, attach, template) = ev.Eval()
//     let implEmbedded = attach impl
//     render renderEmbedded template implEmbedded state


type Packed = { Name: string }

type PackedType =
    | PackedInt
    | PackedReal

type NumberOps<'t> =
    { opPack: 't -> Packed
      opUnpack: Packed -> 't
      opAdd: 't -> 't -> 't }

type ApplyNumberOps<'x> =
    abstract Apply : 't NumberOps -> 'x

// ∃ 't. 't NumberOps
type ExNumberOps =
    abstract Apply : ApplyNumberOps<'x> -> 'x

// take any 't NumberOps to an ExNumberOps
// in some sense this is the only "proper" way to create an instance of ExNumberOps
let wrap n =
    { new ExNumberOps with
        member __.Apply(f) = f.Apply(n) }

let getNumberOps (t: PackedType) =
    match t with
    | PackedInt ->
        wrap
            { opPack = packInt
              opUnpack = unpackInt
              opAdd = (+) }
    | PackedReal ->
        wrap
            { opPack = packReal
              opUnpack = unpackReal
              opAdd = addReal }

let addPacked (t: PackedType) (a: Packed) (b: Packed) =
    (getNumberOps t)
        .Apply { new ApplyNumberOps<_> with
                     member __.Apply
                         ({ opPack = pack
                            opUnpack = unpack
                            opAdd = add })
                         =
                         pack <| add (unpack a) (unpack b) }


// type GFunc<'R> =
//     abstract Invoke<'T> : 'T -> 'R

// let test (f : GFunc<int>) =
//     f.Invoke 42 + f.Invoke "42"

// let GFuncS = { new GFunc<int> with override this.Invoke s = 0 }
// let GFuncI = { new GFunc<int> with override this.Invoke (s: int) = s }

// let x = test GFuncI


// type IMapState<'N, 'S, 'A, 'Q> =
//     abstract Accept : GFunc<'N, 'S, 'A, 'Q, 'R> -> 'R

// and MapState<'N, 'S, 'S2, 'A, 'Q> = { MapF : 'S -> 'S2; Template : Template<'N, 'S2, 'A, 'Q> }
// with
//     interface IMapState<'N, 'S, 'A, 'Q> with
//       member this.Accept (f: GFunc<'N, 'S, 'A, 'Q, 'R>) = f.Invoke<'S2> this

// and GFunc<'N, 'S, 'A, 'Q, 'R> =
//     abstract Invoke<'S2> : MapState<'N, 'S, 'S2, 'A, 'Q> -> 'R

// let pack (cell : MapState<'N, 'S, 'S2, 'A, 'Q>) = cell :> IMapState<'N, 'S, 'A, 'Q>
// let unpack (cell : IMapState<'N, 'S, 'A, 'Q>) (f : GFunc<'N, 'S, 'A, 'Q, 'R>) : 'R = cell.Accept f


type IMapState<'N, 'S, 'A, 'Q> =
    abstract Accept : GFunc<'N, 'S, 'A, 'Q, 'R> -> 'R

and MapState<'N, 'S, 'S2, 'A, 'Q>(m, t) =
    member this.MapF : 'S -> 'S2 = m
    member this.Template : Template<'N, 'S2, 'A, 'Q> = t
    with
        interface IMapState<'N, 'S, 'A, 'Q> with
            member this.Accept f = f.Invoke<'S2> this

and GFunc<'N, 'S, 'A, 'Q, 'R> =
    abstract Invoke<'S2> : MapState<'N, 'S, 'S2, 'A, 'Q> -> 'R

let pack (mapState: MapState<'N, 'S, 'S2, 'A, 'Q>) = mapState :> IMapState<'N, 'S, 'A, 'Q>
let unpack (mapState: IMapState<'N, 'S, 'A, 'Q>) (f: GFunc<'N, 'S, 'A, 'Q, 'R>) : 'R = mapState.Accept f

let makeMapState<'N, 'S, 'S2, 'A, 'Q> (f: 'S -> 'S2) (t: Template<'N, 'S2, 'A, 'Q>) =
    pack <| MapState<'N, 'S, 'S2, 'A, 'Q>(f, t)

let render parent template state : View<_, _> = failwith ""

let renderMapState<'N, 'S, 'A, 'Q> mapState impl s =
    unpack
        mapState
        { new GFunc<'N, 'S, 'A, 'Q, View<'S, 'I>> with
            member __.Invoke(mapState: MapState<'N, 'S, 'S2, 'A, 'Q>) =
                render impl mapState.Template <| mapState.MapF s }

// let getLength cell =
//     unpack cell
//         { new GFunc<int> with
//             member __.Invoke (cell : IMapState<'T>) =
//               List.length cell.Items }
