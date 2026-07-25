import { useEffect, useState } from 'react'
import { getAyahsBySurah } from '@/services/studentPlanService'
import type { EditPlanRowInput, StudentPlanItem, StudentPlanSurahOption } from '@/types/studentPlan'

interface StudentPlanItemsTableProps {
  items: StudentPlanItem[]
  editMode: boolean
  surahs: StudentPlanSurahOption[]
  planTypes: string[]
  editRows: EditPlanRowInput[]
  onEditRowsChange: (rows: EditPlanRowInput[]) => void
  onEdit: (key: string) => void
  onDelete: (key: string) => void
  canModify: boolean
}

function EditRow({
  surahs,
  planTypes,
  row,
  onChange,
  onDelete,
}: {
  surahs: StudentPlanSurahOption[]
  planTypes: string[]
  row: EditPlanRowInput
  onChange: (row: EditPlanRowInput) => void
  onDelete: () => void
}) {
  const [ayahs, setAyahs] = useState<number[]>([])

  useEffect(() => {
    if (row.surahId <= 0 || row.surahId > 114) {
      setAyahs([])
      return
    }
    getAyahsBySurah(row.surahId).then((list) => setAyahs(list.map((a) => a.ayahNumber)))
  }, [row.surahId])

  return (
    <tr className="border-b">
      <td className="px-2 py-2">
        <select
          className="w-full rounded border px-2 py-1 text-sm"
          value={row.surahId}
          onChange={(e) => onChange({ ...row, surahId: Number(e.target.value) })}
        >
          {surahs.filter((s) => s.id <= 114).map((s) => (
            <option key={s.id} value={s.id}>
              {s.nameAr}
            </option>
          ))}
        </select>
      </td>
      <td className="px-2 py-2">
        <select
          className="w-full rounded border px-2 py-1 text-sm"
          value={row.fromAyahNumber}
          onChange={(e) => onChange({ ...row, fromAyahNumber: Number(e.target.value) })}
        >
          {ayahs.map((a) => (
            <option key={a} value={a}>
              {a}
            </option>
          ))}
        </select>
      </td>
      <td className="px-2 py-2">
        <select
          className="w-full rounded border px-2 py-1 text-sm"
          value={row.toAyahNumber}
          onChange={(e) => onChange({ ...row, toAyahNumber: Number(e.target.value) })}
        >
          {ayahs.map((a) => (
            <option key={a} value={a}>
              {a}
            </option>
          ))}
        </select>
      </td>
      <td className="px-2 py-2">
        <select
          className="w-full rounded border px-2 py-1 text-sm"
          value={row.planType}
          onChange={(e) => onChange({ ...row, planType: e.target.value })}
        >
          {planTypes.map((t) => (
            <option key={t} value={t}>
              {t}
            </option>
          ))}
        </select>
      </td>
      <td className="px-2 py-2">
        <button type="button" className="rounded bg-red-600 px-2 py-1 text-xs text-white" onClick={onDelete}>
          حذف
        </button>
      </td>
    </tr>
  )
}

export function StudentPlanItemsTable({
  items,
  editMode,
  surahs,
  planTypes,
  editRows,
  onEditRowsChange,
  onEdit,
  onDelete,
  canModify,
}: StudentPlanItemsTableProps) {
  if (items.length === 0) {
    return <p className="py-6 text-center text-slate-500">لا توجد بنود في الخطة. أضف بنوداً أدناه.</p>
  }

  const updateEditRow = (key: string, patch: EditPlanRowInput) => {
    onEditRowsChange(editRows.map((r) => (r.key === key ? patch : r)))
  }

  return (
    <div className="overflow-x-auto">
      <table className="w-full min-w-[600px] text-right text-sm">
        <thead>
          <tr className="border-b bg-slate-50">
            <th className="px-2 py-2">السورة</th>
            <th className="px-2 py-2">من آية</th>
            <th className="px-2 py-2">إلى آية</th>
            <th className="px-2 py-2">نوع الخطة</th>
            <th className="px-2 py-2">إجراءات</th>
          </tr>
        </thead>
        <tbody>
          {editMode
            ? editRows.map((row) => (
                  <EditRow
                    key={row.key}
                    surahs={surahs}
                    planTypes={planTypes}
                    row={row}
                    onChange={(patch) => updateEditRow(row.key, patch)}
                    onDelete={() => onDelete(row.key)}
                  />
              ))
            : items.map((item) => (
                <tr key={item.key} className="border-b">
                  <td className="px-2 py-2">{item.surahName}</td>
                  <td className="px-2 py-2">{item.fromAyahNumber}</td>
                  <td className="px-2 py-2">{item.toAyahNumber}</td>
                  <td className="px-2 py-2">{item.planType}</td>
                  <td className="px-2 py-2">
                    {canModify && (
                      <div className="flex gap-2">
                        <button
                          type="button"
                          className="rounded bg-cyan-600 px-2 py-1 text-xs text-white"
                          onClick={() => onEdit(item.key)}
                        >
                          تعديل
                        </button>
                        <button
                          type="button"
                          className="rounded bg-red-600 px-2 py-1 text-xs text-white"
                          onClick={() => onDelete(item.key)}
                        >
                          حذف
                        </button>
                      </div>
                    )}
                  </td>
                </tr>
              ))}
        </tbody>
      </table>
    </div>
  )
}

export function itemsToEditRows(items: StudentPlanItem[]): EditPlanRowInput[] {
  return items.map((item) => ({
    key: item.key,
    surahId: item.surahId,
    fromAyahNumber: item.fromAyahNumber,
    toAyahNumber: item.toAyahNumber,
    planType: item.planType,
  }))
}
