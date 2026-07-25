import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { StudentCardPrint } from '@/types/studentCardPrint'

export async function fetchStudentCardPrint(
  id: number,
): Promise<ApiResponse<StudentCardPrint>> {
  const { data } = await api.get<ApiResponse<StudentCardPrint>>(
    `/adminregisterform/${id}/card-print`,
  )
  return data
}
