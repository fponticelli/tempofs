namespace Tempo.Std

module List =
    let filterMap<'A, 'B> (f: 'A -> 'B option) (ls: 'A list) : 'B list =
        List.foldBack
            (fun curr acc ->
                match f curr with
                | Some v -> v :: acc
                | None -> acc)
            ls
            []

    type RankStrategy =
        | StandardCompetition
        | ModifiedCompetition
        | DenseRanking

    let rank<'A, 'B when 'B: comparison> (getScore: 'A -> 'B) (strategy: RankStrategy) (ls: 'A list) =
        let calcRank (current: int) (groupLength: int) =
            match strategy with
            | StandardCompetition -> (current + 1, groupLength - 1)
            | ModifiedCompetition ->
                let assign = current + groupLength
                (assign, 0)
            | DenseRanking -> (current + 1, 0)

        let (_, ls) =
            List.groupBy getScore ls
            |> List.sortBy (fun (s, _) -> s)
            |> List.map (fun (_, v) -> v)
            |> List.fold
                (fun (currRank: int, ls: (int * 'A) list) rest ->
                    let (assign, shift) = calcRank currRank rest.Length
                    let accLs = List.map (fun v -> (assign, v)) rest
                    (assign + shift, ls @ accLs))
                (0, List.empty)

        ls

    let moveItem<'T> (oldIndex: int) (newIndex: int) (list: List<'T>) : List<'T> =
        if oldIndex = newIndex then
            list
        else if oldIndex > newIndex then
            List.permute
                (fun index ->
                    if index = oldIndex then
                        newIndex
                    else if (index < newIndex) || (index > oldIndex) then
                        index
                    else
                        index + 1)
                list
        else
            List.permute
                (fun index ->
                    if index = oldIndex then
                        newIndex
                    else if (index < oldIndex) || (index > newIndex) then
                        index
                    else
                        index - 1)
                list
