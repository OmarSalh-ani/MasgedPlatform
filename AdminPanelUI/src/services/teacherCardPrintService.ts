import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { TeacherCardPrint } from '@/types/teacherCardPrint'

export async function fetchTeacherCardPrint(
  id: number,
): Promise<ApiResponse<TeacherCardPrint>> {
  const { data } = await api.get<ApiResponse<TeacherCardPrint>>(
    `/adminteacher/${id}/card-print`,
  )
  return data
}
