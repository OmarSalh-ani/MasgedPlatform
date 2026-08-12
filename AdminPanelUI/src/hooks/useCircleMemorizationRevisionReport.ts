import { useMutation, useQuery } from '@tanstack/react-query'
import { downloadBlob } from '@/lib/download'
import {
  exportCircleMemorizationRevisionReport,
  getCircleMemorizationTeachers,
} from '@/services/circleMemorizationRevisionReportService'
import type { CircleReportExportFormat } from '@/types/circleMemorizationRevisionReport'

export function useCircleMemorizationTeachers() {
  return useQuery({
    queryKey: ['circle-memorization-revision-report', 'teachers'],
    queryFn: getCircleMemorizationTeachers,
  })
}

export function useCircleMemorizationRevisionExport() {
  return useMutation({
    mutationFn: (params: {
      teacherId: number
      fromDate: string
      toDate: string
      format: CircleReportExportFormat
    }) => exportCircleMemorizationRevisionReport(params),
    onSuccess: (blob, vars) => {
      const stamp = new Date().toISOString().slice(0, 19).replace(/[-:T]/g, '')
      const ext = vars.format === 'excel' ? 'xlsx' : 'pdf'
      downloadBlob(blob, `تقرير_الحفظ_والمراجعة_${vars.teacherId}_${stamp}.${ext}`)
    },
    onError: async (error: { response?: { data?: Blob } }) => {
      const message = await readBlobError(error.response?.data)
      window.alert(message ?? 'حدث خطأ أثناء توليد التقرير.')
    },
  })
}

async function readBlobError(blob?: Blob): Promise<string | null> {
  if (!blob) return null
  try {
    const text = await blob.text()
    if (!text) return null
    try {
      const json = JSON.parse(text) as { message?: string }
      return json.message ?? text
    } catch {
      return text
    }
  } catch {
    return null
  }
}
