import { Download, FileText, Pencil, Trash2 } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import type { FilesManagerListItem } from '@/types/filesManager'

interface FilesManagerCardProps {
  item: FilesManagerListItem
  canModify: boolean
  onDelete: (id: number) => void
}

export function FilesManagerCard({ item, canModify, onDelete }: FilesManagerCardProps) {
  return (
    <article className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-md transition hover:-translate-y-1 hover:shadow-lg">
      <div className="bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-5 py-6 text-center text-white">
        <div className="mx-auto mb-3 flex size-20 items-center justify-center rounded-full border-4 border-white/30 bg-white/20">
          <FileText className="size-9" />
        </div>
        <h2 className="text-lg font-bold break-words">{item.name}</h2>
        <p className="mt-1 text-sm opacity-80">رقم الملف: {item.id}</p>
      </div>

      <div className="space-y-4 p-5">
        <div className="flex items-start gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <span className="shrink-0 font-semibold text-slate-700">الرابط:</span>
          <span className="break-all text-slate-600">{item.fileUrl}</span>
        </div>

        <div className="flex flex-wrap gap-2">
          <a
            href={item.fileUrl}
            target="_blank"
            rel="noreferrer"
            className="inline-flex flex-1 min-w-[120px] items-center justify-center gap-1 rounded-lg bg-emerald-600 px-3 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
          >
            <Download className="size-4" />
            تحميل
          </a>
          {canModify && (
            <>
              <Link
                to={`/files-manager/${item.id}/edit`}
                className="inline-flex flex-1 min-w-[120px] items-center justify-center gap-1 rounded-lg bg-cyan-600 px-3 py-2 text-sm font-semibold text-white hover:bg-cyan-700"
              >
                <Pencil className="size-4" />
                تعديل
              </Link>
              <Button
                type="button"
                className="min-w-[120px] flex-1 bg-red-600 hover:bg-red-700"
                onClick={() => onDelete(item.id)}
              >
                <Trash2 className="size-4" />
                حذف
              </Button>
            </>
          )}
        </div>
      </div>
    </article>
  )
}
