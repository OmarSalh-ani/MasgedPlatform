import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { Students2Response } from '@/types/students2'

export async function getStudents2(search: string): Promise<Students2Response> {
  const { data } = await api.get<ApiResponse<Students2Response>>('/adminstudents2', {
    params: search ? { search } : undefined,
  })
  return data.data
}
