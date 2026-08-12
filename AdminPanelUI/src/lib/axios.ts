import axios from 'axios'
import { API_BASE_URL } from './constants'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('admin_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const requestUrl = error.config?.url ?? ''
    const isSessionCheck = requestUrl.includes('/adminauth/session')

    if (
      error.response?.status === 401 &&
      !window.location.pathname.startsWith('/login') &&
      !isSessionCheck
    ) {
      localStorage.removeItem('admin_token')
      localStorage.removeItem('admin_session')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

export default api
