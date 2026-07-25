import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { clearAdminAuth } from '@/lib/authStorage'
import { changePassword } from '@/services/authService'
import type { ChangePasswordRequest } from '@/types/auth'

export function useChangePassword() {
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload: ChangePasswordRequest) => changePassword(payload),
    onSuccess: (response) => {
      if (!response.success) return
      clearAdminAuth()
      navigate('/login', { replace: true })
    },
  })
}
