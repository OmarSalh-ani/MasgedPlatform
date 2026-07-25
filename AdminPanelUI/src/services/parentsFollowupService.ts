import publicApi from '@/lib/publicAxios'
import type { ApiResponse } from '@/types/api'
import type { ParentsFollowup, SaveParentsFollowupPayload } from '@/types/parentsFollowup'

function toFormData(payload: SaveParentsFollowupPayload): FormData {
  const formData = new FormData()
  formData.append('studentName', payload.studentName)
  formData.append('birthdate', payload.birthdate)
  formData.append('studentGender', payload.studentGender)
  formData.append('fatherName', payload.fatherName)
  formData.append('fatherPhone', payload.fatherPhone)
  formData.append('address', payload.address)
  formData.append('maritalStatus', payload.maritalStatus)
  formData.append('healthCondition', payload.healthCondition)
  if (payload.healthDetails) {
    formData.append('healthDetails', payload.healthDetails)
  }
  formData.append('learningDifficulties', payload.learningDifficulties)
  if (payload.learningDifficultiesNotes) {
    formData.append('learningDifficultiesNotes', payload.learningDifficultiesNotes)
  }
  if (payload.photoFile) {
    formData.append('photo', payload.photoFile)
  }
  return formData
}

export async function getParentsFollowup(studentId: number): Promise<ParentsFollowup> {
  const { data } = await publicApi.get<ApiResponse<ParentsFollowup>>(
    `/adminparentsfollowup/${studentId}`,
  )
  return data.data
}

export async function submitParentsFollowup(
  studentId: number,
  payload: SaveParentsFollowupPayload,
): Promise<boolean> {
  const { data } = await publicApi.put<ApiResponse<boolean>>(
    `/adminparentsfollowup/${studentId}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}
