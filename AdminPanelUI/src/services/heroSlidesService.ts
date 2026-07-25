import api from '@/lib/axios'

import type { HeroSlide, HeroSlideListItem, SaveHeroSlidePayload } from '@/types/heroSlide'

import type { ApiResponse, PagedResult } from '@/types/api'



function toFormData(payload: SaveHeroSlidePayload): FormData {

  const formData = new FormData()

  formData.append('sortOrder', String(payload.sortOrder))

  for (const file of payload.imageFiles) {

    formData.append('images', file)

  }

  return formData

}



export async function getHeroSlides(): Promise<HeroSlideListItem[]> {

  const { data } = await api.get<PagedResult<HeroSlideListItem>>('/adminheroslides', {

    params: { pageNumber: 1, pageSize: 0 },

  })

  return data.items

}



export async function getHeroSlide(id: number): Promise<HeroSlide> {

  const { data } = await api.get<ApiResponse<HeroSlide>>(`/adminheroslides/${id}`)

  return data.data

}



export async function getNextHeroSlideSortOrder(): Promise<number> {

  const { data } = await api.get<ApiResponse<number>>('/adminheroslides/next-sort-order')

  return data.data

}



export async function createHeroSlide(payload: SaveHeroSlidePayload): Promise<HeroSlide> {

  const { data } = await api.post<ApiResponse<HeroSlide>>(

    '/adminheroslides',

    toFormData(payload),

    { headers: { 'Content-Type': 'multipart/form-data' } },

  )

  return data.data

}



export async function updateHeroSlide(

  id: number,

  payload: SaveHeroSlidePayload,

): Promise<HeroSlide> {

  const { data } = await api.put<ApiResponse<HeroSlide>>(

    `/adminheroslides/${id}`,

    toFormData(payload),

    { headers: { 'Content-Type': 'multipart/form-data' } },

  )

  return data.data

}



export async function deleteHeroSlide(id: number, deleteImageFile = false): Promise<boolean> {

  const { data } = await api.delete<ApiResponse<boolean>>(`/adminheroslides/${id}`, {

    params: { deleteImageFile },

  })

  return data.data

}

