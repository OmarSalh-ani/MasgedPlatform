import api from '@/lib/axios'
import type { MasgedSettings, MasgedSettingsApiResponse } from '@/types/masgedSettings'

export async function getMasgedSettings(): Promise<MasgedSettings | null> {
  const { data } = await api.get<MasgedSettingsApiResponse>('/adminmasgedsettings')
  return data.data
}
