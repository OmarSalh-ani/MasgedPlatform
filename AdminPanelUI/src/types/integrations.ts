export interface IntegrationSettings {
  wasenderApiTokenConfigured: boolean
  wasenderApiTokenHint: string | null
  wasenderSessionApiKeyConfigured: boolean
  wasenderSessionApiKeyHint: string | null
  agoraAppIdConfigured: boolean
  agoraAppIdHint: string | null
  agoraAppCertificateConfigured: boolean
  agoraAppCertificateHint: string | null
}

export interface UpdateIntegrationSettingsPayload {
  wasenderApiToken?: string | null
  wasenderSessionApiKey?: string | null
  agoraAppId?: string | null
  agoraAppCertificate?: string | null
}
