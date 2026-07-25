import type { LucideIcon } from 'lucide-react'

export interface NavVisibility {
  adminOnly?: boolean
  hideForGirlTeacher?: boolean
  hideForGirlTeacherAdmin?: boolean
}

export interface AdminNavLink {
  type: 'link'
  to: string
  label: string
  icon: LucideIcon
  visibility?: NavVisibility
  external?: boolean
}

export interface AdminNavGroup {
  type: 'group'
  id: string
  label: string
  icon: LucideIcon
  children: AdminNavLink[]
  autoExpandPaths: string[]
}

export interface AdminNavDivider {
  type: 'divider'
}

export interface AdminNavSection {
  type: 'section'
  label: string
}

export type AdminNavEntry = AdminNavLink | AdminNavGroup | AdminNavDivider | AdminNavSection
