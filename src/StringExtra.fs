namespace Tempo.Utils

module String =
    let ToOption (s: string) = if s = "" then None else Some s
