export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  id: number
  username: string
  isAdmin: boolean
  isGirlTeacher: boolean
  isViewOnly: boolean
  redirectPath: string
}

export interface AdminSession {
  id: number
  username: string
  isAdmin: boolean
  isGirlTeacher: boolean
  isViewOnly: boolean
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
  confirmPassword: string
}
