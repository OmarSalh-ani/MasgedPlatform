import { useMutation, useQuery } from '@tanstack/react-query'
import {
  exportSpecialStudentsReport,
  getSpecialStudentsReport,
} from '@/services/specialStudentsReportService'

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

async function readBlobError(blob?: Blob): Promise<string | null> {
  if (!blob) return null
  try {
    return (await blob.text()) || null
  } catch {
    return null
  }
}

export function useSpecialStudentsReport() {
  const reportQuery = useQuery({
    queryKey: ['special-students-report'],
    queryFn: getSpecialStudentsReport,
  })

  const exportMutation = useMutation({
    mutationFn: exportSpecialStudentsReport,
    onSuccess: (blob) => {
      const stamp = new Date().toISOString().slice(0, 10)
      downloadBlob(blob, `تقرير_الطلاب_المميزين_جميع_الحلقات_${stamp}.xlsx`)
    },
    onError: async (error: { response?: { data?: Blob } }) => {
      const message = await readBlobError(error.response?.data)
      window.alert(message ?? 'حدث خطأ أثناء تصدير التقرير.')
    },
  })

  return { reportQuery, exportMutation }
}
