export interface WomanActivityListItem {
  id: number
  name: string
  isVisible: boolean
}

export interface WomanActivity extends WomanActivityListItem {
  forGirl: boolean
}

export interface SaveWomanActivityPayload {
  name: string
  isVisible: boolean
}
