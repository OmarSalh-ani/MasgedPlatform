export interface HeroSlideListItem {

  id: number

  imageUrl: string | null

  sortOrder: number

}



export interface HeroSlide {

  id: number

  imageUrl: string | null

  sortOrder: number

}



export interface SaveHeroSlidePayload {

  sortOrder: number

  imageFiles: File[]

}

