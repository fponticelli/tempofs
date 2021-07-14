namespace Tempo.Std

module Option =
    let ofString (value: string) : string option = if value = "" then None else Some value
    let ofTrimmedString (value: string) : string option = value.Trim() |> ofString
    let asString (value: string option) : string = Option.defaultValue "" value

    let ofOptionList<'T> (ls: Option<'T> list) : List<'T> option =
        List.fold
            (fun acc opt ->
                match (acc, opt) with
                | (Some ls, Some v) -> Some(v :: ls)
                | _ -> acc)
            (Some [])
            ls

    let toBool<'T> (value: 'T option) : bool =
        match value with
        | Some _ -> true
        | None -> false
