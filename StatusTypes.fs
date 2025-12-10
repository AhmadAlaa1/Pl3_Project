namespace StudentManagement

module StatusTypes =

    /// Status codes مع الأرقام الحقيقية
    type StatusCode =
        | OK = 200
        | Created = 201
        | NoContent = 204
        | BadRequest = 400
        | Unauthorized = 401
        | Forbidden = 403
        | NotFound = 404
        | Conflict = 409
        | ServerError = 500

    /// أخطاء ممكن نرجعها من العمليات المختلفة
    type StatusError =
        | ValidationError of string
        | NotFoundError of string
        | ConflictError of string
        | ServerError of string

    /// تحويل StatusError لرسالة string
    let toMsg (err: StatusError) : string =
        match err with
        | ValidationError msg -> msg
        | NotFoundError msg -> msg
        | ConflictError msg -> msg
        | ServerError msg -> msg

    /// تحويل StatusError لكود HTTP
    let toCode (err: StatusError) : StatusCode =
        match err with
        | ValidationError _ -> StatusCode.BadRequest
        | NotFoundError _ -> StatusCode.NotFound
        | ConflictError _ -> StatusCode.Conflict
        | ServerError _ -> StatusCode.ServerError
