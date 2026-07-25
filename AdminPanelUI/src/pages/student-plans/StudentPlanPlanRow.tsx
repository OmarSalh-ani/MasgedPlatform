import { useEffect, useState } from 'react'
import { getAyahsBySurah } from '@/services/studentPlanService'
import type { PlanRowInput, StudentPlanSurahOption } from '@/types/studentPlan'

interface StudentPlanPlanRowProps {
  surahs: StudentPlanSurahOption[]
  planTypes: string[]
  row: PlanRowInput
  onChange: (row: PlanRowInput) => void
}

export function StudentPlanPlanRow({ surahs, planTypes, row, onChange }: StudentPlanPlanRowProps) {
  const [ayahs, setAyahs] = useState<number[]>([])
  const [loadingAyahs, setLoadingAyahs] = useState(false)

  useEffect(() => {
    if (row.surahId <= 0 || row.surahId > 114) {
      setAyahs([])
      return
    }

    let cancelled = false
    setLoadingAyahs(true)
    getAyahsBySurah(row.surahId)
      .then((list) => {
        if (!cancelled) setAyahs(list.map((a) => a.ayahNumber))
      })
      .finally(() => {
        if (!cancelled) setLoadingAyahs(false)
      })

    return () => {
      cancelled = true
    }
  }, [row.surahId])

  const isHezbOrQuarter = row.surahId > 1000

  return (
    <tr className="border-b">
      <td className="px-2 py-2">
        <select
          className="w-full min-w-[140px] rounded border px-2 py-1 text-sm"
          value={row.surahId || ''}
          onChange={(e) =>
            onChange({
              ...row,
              surahId: Number(e.target.value),
              fromAyahNumber: Number(e.target.value) > 1000 ? 1 : 0,
              toAyahNumber: Number(e.target.value) > 1000 ? 1 : 0,
            })
          }
        >
          <option value="">-- اختر السورة --</option>
          {surahs.map((s) => (
            <option key={s.id} value={s.id}>
              {s.nameAr}
            </option>
          ))}
        </select>
      </td>
      <td className="px-2 py-2">
        <select
          className="w-full min-w-[80px] rounded border px-2 py-1 text-sm"
          disabled={isHezbOrQuarter || loadingAyahs || ayahs.length === 0}
          value={row.fromAyahNumber || ''}
          onChange={(e) => {
            const from = Number(e.target.value)
            onChange({
              ...row,
              fromAyahNumber: from,
              toAyahNumber: row.toAyahNumber < from ? from : row.toAyahNumber,
            })
          }}
        >
          <option value="">--</option>
          {ayahs.map((a) => (
            <option key={a} value={a}>
              {a}
            </option>
          ))}
        </select>
      </td>
      <td className="px-2 py-2">
        <select
          className="w-full min-w-[80px] rounded border px-2 py-1 text-sm"
          disabled={isHezbOrQuarter || loadingAyahs || ayahs.length === 0}
          value={row.toAyahNumber || ''}
          onChange={(e) => onChange({ ...row, toAyahNumber: Number(e.target.value) })}
        >
          <option value="">--</option>
          {ayahs.filter((a) => !row.fromAyahNumber || a >= row.fromAyahNumber).map((a) => (
            <option key={a} value={a}>
              {a}
            </option>
          ))}
        </select>
      </td>
      <td className="px-2 py-2">
        <select
          className="w-full min-w-[120px] rounded border px-2 py-1 text-sm"
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
      <td className="px-2 py-2" />
    </tr>
  )
}

export function emptyPlanRow(): PlanRowInput {
  return { surahId: 0, fromAyahNumber: 0, toAyahNumber: 0, planType: 'حفظ' }
}
