namespace Tempo

module Update =
    type Update<'S, 'A> = 'S -> 'A -> 'S

    type MiddlewarePayload<'S, 'A, 'Q> =
        { Current: 'S
          Previous: 'S
          Action: 'A
          Dispatch: 'A -> unit
          Request: 'Q -> unit }

    type Middleware<'S, 'A, 'Q> = MiddlewarePayload<'S, 'A, 'Q> -> unit
