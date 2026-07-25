import { useMutation, useQuery } from '@tanstack/react-query'
import {
  exportCompletedSurahsReport,
  exportMemorizationRevisionReport,
  getMemorizationRevisionReport,
  getMemorizationRevisionStudents,
} from '@/services/memorizationRevisionReportService'

function downloadBlob(blob: Blob, fileName: string) {
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  window.URL.revokeObjectURL(url)
}

export function useMemorizationRevisionStudents() {
  return useQuery({
    queryKey: ['memorization-revision-report', 'students'],
    queryFn: getMemorizationRevisionStudents,
  })
}

export function useMemorizationRevisionReport(studentId: number | null) {
  const reportQuery = useQuery({
    queryKey: ['memorization-revision-report', studentId],
    queryFn: () => getMemorizationRevisionReport(studentId!),
    enabled: studentId !== null && studentId > 0,
  })

  const exportFullMutation = useMutation({
    mutationFn: () => exportMemorizationRevisionReport(studentId!),
    onSuccess: (blob) => {
      const stamp = new Date().toISOString().slice(0, 19).replace(/[-:T]/g, '')
      downloadBlob(blob, `تقرير_الحفظ_والمراجعة_${studentId}_${stamp}.xlsx`)
    },
    onError: async (error: { response?: { data?: Blob } }) => {
      const message = await readBlobError(error.response?.data)
      window.alert(message ?? 'حدث خطأ أثناء التصدير.')
    },
  })

  const exportCompletedMutation = useMutation({
    mutationFn: () => exportCompletedSurahsReport(studentId!),
    onSuccess: (blob) => {
      const stamp = new Date().toISOString().slice(0, 19).replace(/[-:T]/g, '')
      downloadBlob(blob, `السور_التي_تمت_${studentId}_${stamp}.xlsx`)
    },
    onError: async (error: { response?: { data?: Blob } }) => {
      const message = await readBlobError(error.response?.data)
      window.alert(message ?? 'حدث خطأ أثناء التصدير.')
    },
  })

  return { reportQuery, exportFullMutation, exportCompletedMutation }
}

async function readBlobError(blob?: Blob): Promise<string | null> {
  if (!blob) return null
  try {
    const text = await blob.text()
    return text || null
  } catch {
    return null
  }
}
