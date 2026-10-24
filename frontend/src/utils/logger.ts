export function logError(
    context: string,
    error: unknown,
): void {
    console.error(`[ChatApp] ${context}`, error)
}