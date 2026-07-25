import axios from 'axios'
import api from '@/lib/axios'
import { getAdminSession } from '@/lib/authStorage'
import type { ApiResponse } from '@/types/api'
import type { ChangePasswordRequest, LoginRequest, LoginResponse, AdminSession } from '@/types/auth'

export async function login(
  payload: LoginRequest,
): Promise<ApiResponse<LoginResponse>> {
  const { data } = await api.post<ApiResponse<LoginResponse>>(
    '/adminauth/login',
    {
      username: payload.username,
      password: payload.password,
    },
  )
  return data
}

export async function changePassword(
  payload: ChangePasswordRequest,
): Promise<ApiResponse<boolean>> {
  const { data } = await api.post<ApiResponse<boolean>>('/adminauth/change-password', {
    currentPassword: payload.currentPassword,
    newPassword: payload.newPassword,
    confirmPassword: payload.confirmPassword,
  })
  return data
}

export async function getSession(): Promise<AdminSession> {
  const localSession = getAdminSession()

  try {
    const { data } = await api.get<ApiResponse<AdminSession>>('/adminauth/session')
    if (!data.success || !data.data) {
      throw new Error(data.message || 'Unauthorized')
    }
    return data.data
  } catch (error) {
    // Production API may not have this endpoint yet — fall back to local JWT session.
    if (axios.isAxiosError(error) && error.response?.status === 404 && localSession) {
      return localSession
    }
    throw error
  }
}