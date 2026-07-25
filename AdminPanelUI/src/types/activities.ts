export interface ActivityListItem {
  id: number
  title: string
  imageUrl: string | null
  iconClass: string | null
}

export function getActivitySubtitle(item: ActivityListItem): string {
  if (item.imageUrl?.trim()) return 'صورة'
  return item.iconClass?.trim() || '—'
}
