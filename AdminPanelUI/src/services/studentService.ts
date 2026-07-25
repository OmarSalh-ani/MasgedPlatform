import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { SaveStudentPayload, Student, StudentFormData } from '@/types/student'

export async function getStudentFormData(): Promise<StudentFormData> {
  const { data } = await api.get<ApiResponse<StudentFormData>>('/adminstudent/form-data')
  return data.data
}

export async function getStudent(id: number): Promise<Student> {
  const { data } = await api.get<ApiResponse<Student>>(`/adminstudent/${id}`)
  return data.data
}

export async function createStudent(payload: SaveStudentPayload): Promise<Student> {
  const { data } = await api.post<ApiResponse<Student>>('/adminstudent', payload)
  return data.data
}

export async function updateStudent(id: number, payload: SaveStudentPayload): Promise<Student> {
  const { data } = await api.put<ApiResponse<Student>>(`/adminstudent/${id}`, payload)
  return data.data
}
