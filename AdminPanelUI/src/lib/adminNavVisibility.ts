import type { AdminSession } from '@/types/auth'
import type { AdminNavEntry, AdminNavGroup, AdminNavLink, NavVisibility } from '@/types/adminNav'

function isVisible(visibility: NavVisibility | undefined, session: AdminSession): boolean {
  if (!visibility) return true
  if (visibility.adminOnly && !session.isAdmin) return false
  if (visibility.hideForGirlTeacher && session.isGirlTeacher) return false
  if (visibility.hideForGirlTeacherAdmin && session.isGirlTeacher && session.isAdmin) return false
  return true
}

function filterLink(link: AdminNavLink, session: AdminSession): AdminNavLink | null {
  return isVisible(link.visibility, session) ? link : null
}

function filterGroup(group: AdminNavGroup, session: AdminSession): AdminNavGroup | null {
  const children = group.children
    .map((child) => filterLink(child, session))
    .filter((child): child is AdminNavLink => child !== null)
  if (children.length === 0) return null
  return { ...group, children }
}

function isNavItem(entry: AdminNavEntry): boolean {
  return entry.type === 'link' || entry.type === 'group'
}

function sectionHasVisibleItems(entries: AdminNavEntry[], sectionIndex: number): boolean {
  for (let index = sectionIndex + 1; index < entries.length; index += 1) {
    const entry = entries[index]
    if (entry.type === 'section') return false
    if (isNavItem(entry)) return true
  }
  return false
}

function filterSupervisorNav(entries: AdminNavEntry[]): AdminNavEntry[] {
  const ratingsGroup = entries.find(
    (entry): entry is AdminNavGroup =>
      entry.type === 'group' && entry.id === 'circle-ratings',
  )
  return ratingsGroup ? [ratingsGroup] : []
}

export function filterAdminNav(entries: AdminNavEntry[], session: AdminSession): AdminNavEntry[] {
  if (session.isSupervisor && !session.isAdmin) {
    return filterSupervisorNav(entries)
  }

  const filtered: AdminNavEntry[] = []

  for (const entry of entries) {
    if (entry.type === 'section' || entry.type === 'divider') {
      filtered.push(entry)
      continue
    }
    if (entry.type === 'link') {
      const link = filterLink(entry, session)
      if (link) filtered.push(link)
      continue
    }
    const group = filterGroup(entry, session)
    if (group) filtered.push(group)
  }

  return filtered.filter((entry, index) => {
    if (entry.type !== 'section') return true
    return sectionHasVisibleItems(filtered, index)
  })
}
