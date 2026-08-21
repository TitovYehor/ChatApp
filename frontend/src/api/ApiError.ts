export class ApiError extends Error {
    public readonly status: number

    public readonly responseBody: unknown

    constructor(
        status: number,
        message: string,
        responseBody?: unknown,
    ) {
        super(message)

        this.name = 'ApiError'
        this.status = status
        this.responseBody = responseBody
    }
}