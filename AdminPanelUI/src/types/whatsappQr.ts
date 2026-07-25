export interface WhatsappQrStatus {
  statusText: string
  qrImageDataUrl?: string | null
  bodyHtml?: string | null
  showCreateSession: boolean
  showDisconnect: boolean
  showReconnect: boolean
  isConnected: boolean
}
