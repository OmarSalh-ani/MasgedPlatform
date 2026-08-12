import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { MasgedSettings, SaveMasgedSettingsPayload } from '@/types/masgedSettings'

export async function getMasgedSettings(): Promise<MasgedSettings | null> {
  const { data } = await api.get<ApiResponse<MasgedSettings | null>>('/adminmasgedsettings')
  return data.data
}

function toFormData(payload: SaveMasgedSettingsPayload): FormData {
  const formData = new FormData()
  formData.append('masgedName', payload.masgedName)
  if (payload.logoFile) {
    formData.append('logoFile', payload.logoFile)
  }
  if (payload.removeLogo) {
    formData.append('removeLogo', 'true')
  }
  appendOptionalField(formData, 'parentAppStoreUrl', payload.parentAppStoreUrl)
  appendOptionalField(formData, 'parentGooglePlayUrl', payload.parentGooglePlayUrl)
  appendOptionalField(formData, 'teacherAppStoreUrl', payload.teacherAppStoreUrl)
  appendOptionalField(formData, 'teacherGooglePlayUrl', payload.teacherGooglePlayUrl)
  appendOptionalField(formData, 'primaryColor', payload.primaryColor)
  return formData
}

function appendOptionalField(formData: FormData, key: string, value?: string | null) {
  if (value === undefined) return
  formData.append(key, value?.trim() ?? '')
}

export async function saveMasgedSettings(payload: SaveMasgedSettingsPayload): Promise<MasgedSettings> {
  const { data } = await api.put<ApiResponse<MasgedSettings>>(
    '/adminmasgedsettings',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}
