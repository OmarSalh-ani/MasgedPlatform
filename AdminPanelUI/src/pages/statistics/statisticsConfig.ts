import {
  BookOpen,
  CheckCircle,
  CircleX,
  GraduationCap,
  LogOut,
  Star,
  Users,
} from 'lucide-react'
import type { LucideIcon } from 'lucide-react'
import type { AdditionalStatistics, CircleStatistics } from '@/types/statistics'

export type StatTone = 'emerald' | 'orange' | 'rose' | 'slate' | 'sky' | 'primary' | 'amber'

export interface StatCardConfig {
  key: string
  label: string
  to: string
  icon: LucideIcon
  tone: StatTone
}

export const toneStyles: Record<
  StatTone,
  { iconWrap: string; icon: string; accent: string; hover: string; value: string }
> = {
  emerald: {
    iconWrap: 'bg-emerald-50 ring-emerald-100',
    icon: 'text-emerald-600',
    accent: 'border-emerald-500',
    hover: 'hover:border-emerald-200 hover:bg-emerald-50/40',
    value: 'text-emerald-700',
  },
  orange: {
    iconWrap: 'bg-orange-50 ring-orange-100',
    icon: 'text-orange-600',
    accent: 'border-orange-500',
    hover: 'hover:border-orange-200 hover:bg-orange-50/40',
    value: 'text-orange-700',
  },
  rose: {
    iconWrap: 'bg-rose-50 ring-rose-100',
    icon: 'text-rose-600',
    accent: 'border-rose-500',
    hover: 'hover:border-rose-200 hover:bg-rose-50/40',
    value: 'text-rose-700',
  },
  slate: {
    iconWrap: 'bg-slate-100 ring-slate-200',
    icon: 'text-slate-600',
    accent: 'border-slate-500',
    hover: 'hover:border-slate-200 hover:bg-slate-50',
    value: 'text-slate-700',
  },
  sky: {
    iconWrap: 'bg-sky-50 ring-sky-100',
    icon: 'text-sky-600',
    accent: 'border-sky-500',
    hover: 'hover:border-sky-200 hover:bg-sky-50/40',
    value: 'text-sky-700',
  },
  primary: {
    iconWrap: 'bg-blue-50 ring-blue-100',
    icon: 'text-[var(--color-primary)]',
    accent: 'border-[var(--color-primary)]',
    hover: 'hover:border-blue-200 hover:bg-blue-50/40',
    value: 'text-[var(--color-primary-dark)]',
  },
  amber: {
    iconWrap: 'bg-amber-50 ring-amber-100',
    icon: 'text-amber-600',
    accent: 'border-amber-500',
    hover: 'hover:border-amber-200 hover:bg-amber-50/40',
    value: 'text-amber-700',
  },
}

export const circleStatCards: StatCardConfig[] = [
  {
    key: 'presentToday',
    label: 'الحضور اليوم',
    to: '/attendance-report?type=present',
    icon: CheckCircle,
    tone: 'emerald',
  },
  {
    key: 'departedToday',
    label: 'المنصرفين اليوم',
    to: '/attendance-report?type=departed',
    icon: LogOut,
    tone: 'orange',
  },
  {
    key: 'absentToday',
    label: 'الغياب اليوم',
    to: '/attendance-report?type=absent',
    icon: CircleX,
    tone: 'rose',
  },
  {
    key: 'totalStudents',
    label: 'إجمالي الطلاب',
    to: '/attendance-report?type=all',
    icon: Users,
    tone: 'slate',
  },
]

export const additionalStatCards: StatCardConfig[] = [
  {
    key: 'totalTeachers',
    label: 'عدد المعلمين',
    to: '/teachers',
    icon: GraduationCap,
    tone: 'sky',
  },
  {
    key: 'totalCircles',
    label: 'عدد الحلقات',
    to: '/circles',
    icon: BookOpen,
    tone: 'primary',
  },
  {
    key: 'specialStudents',
    label: 'الطلاب المميزين',
    to: '/special-students-report',
    icon: Star,
    tone: 'amber',
  },
]

export function getCircleStatValue(stats: CircleStatistics, key: string): number {
  return stats[key as keyof CircleStatistics] ?? 0
}

export function getAdditionalStatValue(stats: AdditionalStatistics, key: string): number {
  return stats[key as keyof AdditionalStatistics] ?? 0
}
