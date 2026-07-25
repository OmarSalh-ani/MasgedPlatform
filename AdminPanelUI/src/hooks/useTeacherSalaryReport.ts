import { useMutation, useQuery } from '@tanstack/react-query'
import {
  exportTeacherSalaryReport,
  getTeacherSalaryReport,
} from '@/services/teacherSalaryService'

export function useTeacherSalaryReport(month: number, year: number, enabled: boolean) {
  const reportQuery = useQuery({
    queryKey: ['teacher-salaries', 'report', month, year],
    queryFn: () => getTeacherSalaryReport(month, year),
    enabled: enabled && month > 0 && year > 0,
  })

  const exportMutation = useMutation({
    mutationFn: () => exportTeacherSalaryReport(month, year),
    onSuccess: (blob) => {
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `TeacherSalaryReport_${year}_${month}.xlsx`
      link.click()
      window.URL.revokeObjectURL(url)
    },
  })

  return { reportQuery, exportMutation }
}
