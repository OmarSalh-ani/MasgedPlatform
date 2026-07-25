import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createOthaiminCenterCircle,
  deleteOthaiminCenterStudent,
  exportOthaiminCenterStudents,
  getOthaiminCenterFilterOptions,
  getOthaiminCenterStudentNames,
  getOthaiminCenterStudentReviews,
  getOthaiminCenterStudentTests,
  getOthaiminCenterStudents,
  sendOthaiminCenterWhatsapp,
  transferOthaiminCenterStudents,
} from '@/services/othaiminCenterService'
import type { HomeFilters, HomeStudentNameLookupFilters } from '@/types/home'

export const OTHAIMIN_CENTER_QUERY_KEY = ['othaimin-center', 'list'] as const

export function useOthaiminCenterFilterOptions() {
  return useQuery({
    queryKey: ['othaimin-center', 'filter-options'],
    queryFn: getOthaiminCenterFilterOptions,
  })
}

export function useOthaiminCenterStudentNames(filters: HomeStudentNameLookupFilters | null) {
  return useQuery({
    queryKey: ['othaimin-center', 'student-names', filters],
    queryFn: () => getOthaiminCenterStudentNames(filters!),
    enabled: filters != null,
  })
}

export function useOthaiminCenter(appliedFilters: HomeFilters | null) {
  const queryClient = useQueryClient()

  const listQuery = useQuery({
    queryKey: [...OTHAIMIN_CENTER_QUERY_KEY, appliedFilters],
    queryFn: () => getOthaiminCenterStudents(appliedFilters!),
    enabled: appliedFilters != null,
  })

  const exportMutation = useMutation({
    mutationFn: () => {
      if (!appliedFilters) throw new Error('Filters required')
      const { pageNumber: _pageNumber, pageSize: _pageSize, ...exportFilters } = appliedFilters
      return exportOthaiminCenterStudents(exportFilters)
    },
    onSuccess: (blob) => {
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `MrkzStudents_${new Date().toISOString().slice(0, 10)}.xlsx`
      link.click()
      window.URL.revokeObjectURL(url)
    },
  })

  const deleteMutation = useMutation({
    mutationFn: deleteOthaiminCenterStudent,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: OTHAIMIN_CENTER_QUERY_KEY }),
  })

  const whatsappMutation = useMutation({ mutationFn: sendOthaiminCenterWhatsapp })
  const transferMutation = useMutation({
    mutationFn: transferOthaiminCenterStudents,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: OTHAIMIN_CENTER_QUERY_KEY }),
  })
  const createCircleMutation = useMutation({
    mutationFn: createOthaiminCenterCircle,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: OTHAIMIN_CENTER_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: ['othaimin-center', 'filter-options'] })
    },
  })

  return {
    listQuery,
    exportMutation,
    deleteMutation,
    whatsappMutation,
    transferMutation,
    createCircleMutation,
  }
}

export function useOthaiminCenterStudentTests(studentId: number | null) {
  return useQuery({
    queryKey: ['othaimin-center', 'tests', studentId],
    queryFn: () => getOthaiminCenterStudentTests(studentId!),
    enabled: studentId != null,
  })
}

export function useOthaiminCenterStudentReviews(studentId: number | null) {
  return useQuery({
    queryKey: ['othaimin-center', 'reviews', studentId],
    queryFn: () => getOthaiminCenterStudentReviews(studentId!),
    enabled: studentId != null,
  })
}
