import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { setAdminAuth } from '@/lib/authStorage'
import { login } from '@/services/authService'
import type { LoginRequest } from '@/types/auth'

export function useLogin() {
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload: LoginRequest) => login(payload),
    onSuccess: (response) => {
      if (!response.success || !response.data) return

      const { token, id, username, isAdmin, isGirlTeacher, isViewOnly, redirectPath } =
        response.data

      setAdminAuth(token, {
        id,
        username,
        isAdmin,
        isGirlTeacher,
        isViewOnly,
      })

      navigate(redirectPath || '/', { replace: true })
    },
  })
}
