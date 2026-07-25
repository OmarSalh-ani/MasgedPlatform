import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  FirstTimeSetupPayload,
  MasgedSettings,
  SaveMasgedSettingsPayload,
  SetupStatus,
} from '@/types/masgedSettings'

export async function getMasgedSettings(): Promise<MasgedSettings | null> {
  const { data } = await api.get<ApiResponse<MasgedSettings | null>>('/adminmasgedsettings')
  return data.data
}

export async function getSetupStatus(): Promise<SetupStatus> {
  const { data } = await api.get<ApiResponse<SetupStatus>>('/adminmasgedsettings/setup-status')
  return data.data
}

export async function completeFirstTimeSetup(
  payload: FirstTimeSetupPayload,
): Promise<MasgedSettings> {
  const formData = new FormData()
  formData.append('masgedName', payload.masgedName)
  formData.append('primaryColor', payload.primaryColor)
  formData.append('domain', payload.domain)
  if (payload.logoFile) {
    formData.append('logoFile', payload.logoFile)
  }
  appendOptionalField(formData, 'parentAppStoreUrl', payload.parentAppStoreUrl)
  appendOptionalField(formData, 'parentGooglePlayUrl', payload.parentGooglePlayUrl)
  appendOptionalField(formData, 'teacherAppStoreUrl', payload.teacherAppStoreUrl)
  appendOptionalField(formData, 'teacherGooglePlayUrl', payload.teacherGooglePlayUrl)
  formData.append('adminName', payload.adminName)
  formData.append('adminEmail', payload.adminEmail)
  formData.append('adminPassword', payload.adminPassword)
  const { data } = await api.post<ApiResponse<MasgedSettings>>(
    '/adminmasgedsettings/setup',
    formData,
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
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
  appendOptionalField(formData, 'domain', payload.domain)
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
