import { Link } from 'react-router-dom'
import { Pencil, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { SendNotesTablePagination } from '@/pages/send-notes/SendNotesTablePagination'
import type { SendNoteListItem } from '@/types/sendNote'
import { formatSendNoteDate } from '@/types/sendNote'

interface SendNotesTableProps {
  items: SendNoteListItem[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  onPageChange: (page: number) => void
  onDelete: (id: number) => void
}

export function SendNotesTable({
  items,
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  onPageChange,
  onDelete,
}: SendNotesTableProps) {
  return (
    <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
      {items.length === 0 ? (
        <p className="px-4 py-12 text-center text-sm text-slate-500">لا توجد ملاحظات مسجلة</p>
      ) : (
        <>
          <div className="overflow-x-auto">
              <table className="min-w-full text-sm">
                <thead>
                  <tr className="border-b border-slate-200 bg-slate-50">
                    <th className="px-4 py-3 text-right font-semibold text-slate-700">المعلم</th>
                    <th className="px-4 py-3 text-right font-semibold text-slate-700">الملاحظة</th>
                    <th className="px-4 py-3 text-right font-semibold text-slate-700">
                      تاريخ الإرسال
                    </th>
                    <th className="px-4 py-3 text-right font-semibold text-slate-700">الحالة</th>
                    <th className="px-4 py-3 text-right font-semibold text-slate-700">إجراءات</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((item) => (
                    <tr
                      key={item.id}
                      className="border-b border-slate-100 transition-colors last:border-0 hover:bg-slate-50/80"
                    >
                      <td className="px-4 py-3 text-right text-slate-800">{item.teacherName}</td>
                      <td className="max-w-xs px-4 py-3 text-right">
                        <p className="line-clamp-2 whitespace-normal text-slate-800">{item.note}</p>
                      </td>
                      <td className="px-4 py-3 text-right tabular-nums text-slate-600">
                        {formatSendNoteDate(item.createdAt)}
                      </td>
                      <td className="px-4 py-3 text-right">
                        <span
                          className={
                            item.isRead
                              ? 'inline-flex rounded-full bg-emerald-50 px-3 py-1 text-xs font-semibold text-emerald-700 ring-1 ring-emerald-100'
                              : 'inline-flex rounded-full bg-amber-50 px-3 py-1 text-xs font-semibold text-amber-700 ring-1 ring-amber-100'
                          }
                        >
                          {item.isRead ? 'تمت القراءة' : 'غير مقروءة'}
                        </span>
                        {item.isRead && item.readTime && (
                          <p className="mt-1 text-xs text-slate-500">
                            {formatSendNoteDate(item.readTime)}
                          </p>
                        )}
                      </td>
                      <td className="px-4 py-3 text-right">
                        <div className="flex justify-end gap-2">
                          <Link
                            to={`/send-notes/${item.id}/edit`}
                            className="inline-flex items-center rounded-lg bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-3 py-1.5 text-xs text-white hover:opacity-90"
                            title="تعديل"
                          >
                            <Pencil className="size-3.5" strokeWidth={1.5} absoluteStrokeWidth />
                          </Link>
                          <Button
                            type="button"
                            variant="outline"
                            className="border-red-200 px-3 py-1.5 text-xs text-red-600 hover:bg-red-50"
                            title="حذف"
                            onClick={() => onDelete(item.id)}
                          >
                            <Trash2 className="size-3.5" strokeWidth={1.5} absoluteStrokeWidth />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {totalCount > 0 && (
              <SendNotesTablePagination
                pageNumber={pageNumber}
                pageSize={pageSize}
                totalCount={totalCount}
                totalPages={totalPages}
                onPageChange={onPageChange}
              />
            )}
        </>
      )}
    </div>
  )
}
