import type { AdminSession } from '@/types/auth'
import { isJwtValid } from '@/lib/jwtUtils'

const TOKEN_KEY = 'admin_token'
const SESSION_KEY = 'admin_session'

export function getAdminToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function getAdminSession(): AdminSession | null {
  const raw = localStorage.getItem(SESSION_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as AdminSession
  } catch {
    return null
  }
}

export function setAdminAuth(token: string, session: AdminSession): void {
  localStorage.setItem(TOKEN_KEY, token)
  localStorage.setItem(SESSION_KEY, JSON.stringify(session))
}

export function updateAdminSession(session: AdminSession): void {
  localStorage.setItem(SESSION_KEY, JSON.stringify(session))
}

export function clearAdminAuth(): void {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(SESSION_KEY)
}

export function isAuthenticated(): boolean {
  const token = getAdminToken()
  if (!token) return false

  if (!isJwtValid(token)) {
    clearAdminAuth()
    return false
  }

  return Boolean(getAdminSession())
}

export function canModify(): boolean {
  const session = getAdminSession()
  return session ? !session.isViewOnly : false
}

export function isAdmin(): boolean {
  return getAdminSession()?.isAdmin === true
}

export function isGirlTeacher(): boolean {
  return getAdminSession()?.isGirlTeacher === true
}

export function isSupervisor(): boolean {
  return getAdminSession()?.isSupervisor === true
}

/** Supervisor with restricted access (admins always have full access). */
export function isSupervisorOnly(): boolean {
  const session = getAdminSession()
  return Boolean(session?.isSupervisor && !session.isAdmin)
}
