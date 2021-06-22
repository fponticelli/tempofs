namespace Tempo.Utils

module List =
    let filterMap<'A, 'B> (f: 'A -> 'B option) (ls: 'A list) : 'B list =
        List.foldBack
            (fun curr acc ->
                match f curr with
                | Some v -> v :: acc
                | None -> acc)
            ls
            []
