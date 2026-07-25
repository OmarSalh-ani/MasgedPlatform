import api from '@/lib/axios'
import type {
  ApiResponse,
  CountryDialEntry,
  PublicRegisterSuccess,
  PublicRegistrationConfig,
  PublicWebsiteContent,
  RegistrationMode,
  SubmitRegistrationPayload,
} from '@/types/publicIndex'

function unwrapApiResponse<T>(response: ApiResponse<T>, fallbackMessage: string): T {
  if (!response.success || response.data == null) {
    throw new Error(response.message || fallbackMessage)
  }
  return response.data
}

export async function getWebsiteContent() {
  const { data } = await api.get<ApiResponse<PublicWebsiteContent>>('/publicindex/content')
  return unwrapApiResponse(data, 'تعذر تحميل محتوى الموقع')
}

export async function getRegistrationConfig(mode: RegistrationMode) {
  const query = mode === 'default' ? '' : `?mode=${mode}`
  const { data } = await api.get<ApiResponse<PublicRegistrationConfig>>(
    `/publicindex/registration-config${query}`,
  )
  return unwrapApiResponse(data, 'تعذر تحميل إعدادات التسجيل')
}

export async function getCountryDialCodes() {
  const { data } = await api.get<ApiResponse<CountryDialEntry[]>>('/publiccountrydialcodes')
  return data.data
}

export async function submitRegistration(payload: SubmitRegistrationPayload) {
  const { data } = await api.post<ApiResponse<{ id: number }>>('/publicindex/registration', payload)
  return data.data
}

export async function getRegisterSuccess() {
  const { data } = await api.get<ApiResponse<PublicRegisterSuccess>>('/publicindex/register-success')
  return data.data
}
