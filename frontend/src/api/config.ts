export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7163'
export const SIGNALR_HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL ?? `${API_BASE_URL}/hubs/chat`