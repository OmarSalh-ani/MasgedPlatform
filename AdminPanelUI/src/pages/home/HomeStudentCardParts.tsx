import {
  BookOpen,
  ClipboardList,
  Eye,
  Pencil,
  Printer,
  QrCode,
  Trash2,
} from 'lucide-react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { useStudentQrDownload } from '@/hooks/useStudentQrDownload'
import type { HomeStudentListItem, SelectedHomeStudent } from '@/types/home'

export interface HomeStudentItemProps {
  item: HomeStudentListItem
  selected: boolean
  canModify: boolean
  onToggle: (student: SelectedHomeStudent) => void
  onDelete: (id: number) => void
  onShowTests: (id: number, name: string) => void
  onShowReviews: (id: number, name: string) => void
}

export function buildHomeStudentUrls(id: number) {
  return {
    followupUrl: `/parents-followup?id=${id}&Print=1`,
    planUrl: `/student-plans?studentId=${id}`,
    editUrl: `/students/${id}/edit`,
    cardPrintUrl: `/students/${id}/card-print`,
  }
}

export function toSelectedStudent(item: HomeStudentListItem): SelectedHomeStudent {
  return {
    id: item.id,
    studentName: item.studentName,
    fatherName: item.fatherName,
    fatherPhone: item.fatherPhone,
    circleName: item.circleName,
  }
}

export function InfoRow({ label, value, highlight = false }: { label: string; value: string; highlight?: boolean }) {
  return (
    <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3">
      <span className="min-w-24 font-semibold text-slate-700">{label}:</span>
      {highlight ? (
        <span className="rounded-full bg-[#CBAC2D] px-2 py-0.5 text-xs font-bold text-white">{value}</span>
      ) : (
        <span className="text-slate-600">{value}</span>
      )}
    </div>
  )
}

export function HomeStudentSelectCheckbox({
  item,
  selected,
  onToggle,
  className = '',
}: {
  item: HomeStudentListItem
  selected: boolean
  onToggle: (student: SelectedHomeStudent) => void
  className?: string
}) {
  return (
    <label className={`flex items-center gap-2 rounded-lg bg-slate-50 p-3 ${className}`}>
      <input type="checkbox" checked={selected} onChange={() => onToggle(toSelectedStudent(item))} />
      <span className="font-semibold text-[var(--color-primary)]">تحديد</span>
    </label>
  )
}

export function HomeStudentActionButtons({
  item,
  canModify,
  onDelete,
  onShowTests,
  onShowReviews,
  compact = false,
}: Pick<HomeStudentItemProps, 'item' | 'canModify' | 'onDelete' | 'onShowTests' | 'onShowReviews'> & {
  compact?: boolean
}) {
  const { followupUrl, planUrl, editUrl, cardPrintUrl } = buildHomeStudentUrls(item.id)
  const buttonClass = compact ? 'min-w-0 flex-1 px-2 text-xs' : 'min-w-[120px] flex-1'
  const { isDownloading, downloadStudentQr } = useStudentQrDownload()

  const handleDownloadQr = async () => {
    const success = await downloadStudentQr(item.id, item.studentName)
    if (!success) window.alert('تعذر إنشاء رمز QR')
  }

  return (
    <div className="flex flex-wrap gap-2 pt-2">
      <ActionLink href={followupUrl} icon={<Eye className="size-4" />} label="عرض الاستمارة" internal compact={compact} />
      <ActionLink href={cardPrintUrl} icon={<Printer className="size-4" />} label="طباعة الكرت" internal compact={compact} />
      <Button
        type="button"
        variant="outline"
        className={buttonClass}
        disabled={isDownloading}
        onClick={() => void handleDownloadQr()}
      >
        <QrCode className="size-4" />
        {isDownloading ? 'جاري التحميل...' : 'تحميل QR'}
      </Button>
      <Button type="button" variant="outline" className={buttonClass} onClick={() => onShowTests(item.id, item.studentName)}>
        <ClipboardList className="size-4" />
        عرض الاختبارات
      </Button>
      <Button type="button" variant="outline" className={buttonClass} onClick={() => onShowReviews(item.id, item.studentName)}>
        <BookOpen className="size-4" />
        عرض المراجعة
      </Button>
      <ActionLink href={planUrl} icon={<ClipboardList className="size-4" />} label="عرض الخطة" internal compact={compact} />
      {canModify ? (
        <>
          <ActionLink href={editUrl} icon={<Pencil className="size-4" />} label="تعديل" internal compact={compact} />
          <Button type="button" className={`${buttonClass} bg-red-600 hover:bg-red-700`} onClick={() => onDelete(item.id)}>
            <Trash2 className="size-4" />
            حذف
          </Button>
        </>
      ) : null}
    </div>
  )
}

function ActionLink({
  href,
  icon,
  label,
  internal = false,
  compact = false,
}: {
  href: string
  icon: React.ReactNode
  label: string
  internal?: boolean
  compact?: boolean
}) {
  const className = `inline-flex ${compact ? 'min-w-0 flex-1 px-2 text-xs' : 'min-w-[120px] flex-1'} items-center justify-center gap-1 rounded-lg bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-3 py-2 text-sm font-semibold text-white hover:opacity-90`

  if (internal) {
    return (
      <Link to={href} target="_blank" className={className}>
        {icon}
        {label}
      </Link>
    )
  }

  return (
    <a href={href} target="_blank" rel="noreferrer" className={className}>
      {icon}
      {label}
    </a>
  )
}
