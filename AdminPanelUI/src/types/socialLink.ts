export interface SocialLink {
  id: number
  platformName: string
  url: string
  iconClass: string | null
  sortOrder: number
}

export interface SocialLinkListItem {
  id: number
  platformName: string
  url: string
}

export interface SaveSocialLinkPayload {
  platformName: string
  url: string
  iconClass: string | null
  sortOrder: number
}
