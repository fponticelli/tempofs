namespace Tempo.Std

module Result =
    let iter<'V, 'E> (audit: 'V -> unit) (res: Result<'V, 'E>) : unit =
        match res with
        | Ok v -> audit v
        | Error _ -> ()

    let get<'V, 'E> (res: Result<'V, 'E>) : 'V =
        match res with
        | Ok v -> v
        | Error e -> failwith $"{e}"
