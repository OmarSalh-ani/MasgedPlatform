import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createHomeCircle,
  deleteHomeStudent,
  exportHomeStudents,
  getHomeFilterOptions,
  getHomeRegistrationSettings,
  getHomeStudentNames,
  getHomeStudentReviews,
  getHomeStudentTests,
  getHomeStudents,
  removeHomeStudentsFromCircle,
  sendHomeWhatsapp,
  transferHomeStudents,
  updateHomeRegistrationSettings,
} from '@/services/homeService'
import type { HomeFilters, HomeStudentNameLookupFilters } from '@/types/home'

export const HOME_QUERY_KEY = ['home', 'list'] as const

export function useHomeFilterOptions() {
  return useQuery({
    queryKey: ['home', 'filter-options'],
    queryFn: getHomeFilterOptions,
  })
}

export function useHomeStudentNames(filters: HomeStudentNameLookupFilters | null) {
  return useQuery({
    queryKey: ['home', 'student-names', filters],
    queryFn: () => getHomeStudentNames(filters!),
    enabled: filters != null,
  })
}

export function useHomeRegistrationSettings() {
  return useQuery({
    queryKey: ['home', 'registration-settings'],
    queryFn: getHomeRegistrationSettings,
  })
}

export function useHome(appliedFilters: HomeFilters | null) {
  const queryClient = useQueryClient()

  const listQuery = useQuery({
    queryKey: [...HOME_QUERY_KEY, appliedFilters],
    queryFn: () => getHomeStudents(appliedFilters!),
    enabled: appliedFilters != null,
  })

  const exportMutation = useMutation({
    mutationFn: () => {
      if (!appliedFilters) throw new Error('Filters required')
      const { pageNumber: _pageNumber, pageSize: _pageSize, ...exportFilters } = appliedFilters
      return exportHomeStudents(exportFilters)
    },
    onSuccess: (blob) => {
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `Students_${new Date().toISOString().slice(0, 10)}.xlsx`
      link.click()
      window.URL.revokeObjectURL(url)
    },
  })

  const deleteMutation = useMutation({
    mutationFn: deleteHomeStudent,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: HOME_QUERY_KEY }),
  })

  const whatsappMutation = useMutation({ mutationFn: sendHomeWhatsapp })
  const transferMutation = useMutation({
    mutationFn: transferHomeStudents,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: HOME_QUERY_KEY }),
  })
  const removeFromCircleMutation = useMutation({
    mutationFn: removeHomeStudentsFromCircle,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: HOME_QUERY_KEY }),
  })
  const createCircleMutation = useMutation({
    mutationFn: createHomeCircle,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: HOME_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: ['home', 'filter-options'] })
    },
  })
  const updateRegistrationMutation = useMutation({
    mutationFn: updateHomeRegistrationSettings,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['home', 'registration-settings'] }),
  })

  return {
    listQuery,
    exportMutation,
    deleteMutation,
    whatsappMutation,
    transferMutation,
    removeFromCircleMutation,
    createCircleMutation,
    updateRegistrationMutation,
  }
}

export function useHomeStudentTests(studentId: number | null) {
  return useQuery({
    queryKey: ['home', 'tests', studentId],
    queryFn: () => getHomeStudentTests(studentId!),
    enabled: studentId != null,
  })
}

export function useHomeStudentReviews(studentId: number | null) {
  return useQuery({
    queryKey: ['home', 'reviews', studentId],
    queryFn: () => getHomeStudentReviews(studentId!),
    enabled: studentId != null,
  })
}
