import { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { useOthaiminCenter, useOthaiminCenterFilterOptions } from '@/hooks/useOthaiminCenter'
import { canModify, getAdminSession } from '@/lib/authStorage'
import { getOthaiminCenterCircleTitle } from '@/services/othaiminCenterService'
import {
  buildAppliedOthaiminCenterFilters,
  clearOthaiminCenterSelectedStudentsStorage,
  getDefaultOthaiminCenterFilterForm,
  loadOthaiminCenterSelectedStudents,
  saveOthaiminCenterSelectedStudents,
  type HomeFilterFormState,
} from '@/pages/OthaiminCenter/othaiminCenterUtils'
import { HOME_PAGE_SIZE } from '@/types/home'
import type { HomeFilters, SelectedHomeStudent } from '@/types/home'

function parseOptionalInt(value: string | null): number | undefined {
  if (!value) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}

export function useOthaiminCenterPage() {
  const [searchParams] = useSearchParams()
  const circleQuery = parseOptionalInt(searchParams.get('circle'))
  const session = getAdminSession()
  const userCanModify = canModify()
  const isGirlTeacher = session?.isGirlTeacher ?? false

  const [filterForm, setFilterForm] = useState<HomeFilterFormState>(() =>
    getDefaultOthaiminCenterFilterForm(circleQuery),
  )
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(HOME_PAGE_SIZE)
  const [appliedFilters, setAppliedFilters] = useState<HomeFilters | null>(null)
  const [pageTitle, setPageTitle] = useState('قائمة طلاب المركز')
  const [selectedStudents, setSelectedStudents] = useState<Map<number, SelectedHomeStudent>>(() =>
    loadOthaiminCenterSelectedStudents(),
  )
  const [deleteTarget, setDeleteTarget] = useState<number | null>(null)
  const [testsTarget, setTestsTarget] = useState<{ id: number; name: string } | null>(null)
  const [reviewsTarget, setReviewsTarget] = useState<{ id: number; name: string } | null>(null)
  const [whatsappOpen, setWhatsappOpen] = useState(false)
  const [createCircleOpen, setCreateCircleOpen] = useState(false)
  const [transferOpen, setTransferOpen] = useState(false)
  const [actionMessage, setActionMessage] = useState<string | null>(null)

  const filterOptionsQuery = useOthaiminCenterFilterOptions()
  const {
    listQuery,
    exportMutation,
    deleteMutation,
    whatsappMutation,
    transferMutation,
    createCircleMutation,
  } = useOthaiminCenter(appliedFilters)

  useEffect(() => {
    setAppliedFilters(buildAppliedOthaiminCenterFilters(filterForm, 1, pageSize, circleQuery))
    if (circleQuery) {
      getOthaiminCenterCircleTitle(circleQuery).then(setPageTitle).catch(() => setPageTitle('قائمة طلاب المركز'))
    }
  }, [])

  const selectedList = useMemo(() => [...selectedStudents.values()], [selectedStudents])
  const selectedIds = useMemo(() => new Set(selectedList.map((item) => item.id)), [selectedList])
  const list = listQuery.data

  const persistSelection = (next: Map<number, SelectedHomeStudent>) => {
    setSelectedStudents(next)
    saveOthaiminCenterSelectedStudents(next)
  }

  const applyFilters = (page = 1, nextPageSize = pageSize) => {
    setPageNumber(page)
    setAppliedFilters(buildAppliedOthaiminCenterFilters(filterForm, page, nextPageSize, circleQuery))
  }

  const handleClearFilters = () => {
    const defaults = getDefaultOthaiminCenterFilterForm(circleQuery)
    setFilterForm(defaults)
    setPageNumber(1)
    clearOthaiminCenterSelectedStudentsStorage()
    setSelectedStudents(new Map())
    setAppliedFilters(buildAppliedOthaiminCenterFilters(defaults, 1, pageSize, circleQuery))
  }

  const toggleStudent = (student: SelectedHomeStudent) => {
    const next = new Map(selectedStudents)
    if (next.has(student.id)) next.delete(student.id)
    else next.set(student.id, student)
    persistSelection(next)
  }

  const selectAllOnPage = () => {
    const next = new Map(selectedStudents)
    for (const item of list?.items ?? []) {
      next.set(item.id, {
        id: item.id,
        studentName: item.studentName,
        fatherName: item.fatherName,
        fatherPhone: item.fatherPhone,
        circleName: item.circleName,
      })
    }
    persistSelection(next)
  }

  const clearSelection = () => {
    clearOthaiminCenterSelectedStudentsStorage()
    setSelectedStudents(new Map())
  }

  return {
    pageTitle,
    userCanModify,
    isGirlTeacher,
    filterForm,
    setFilterForm,
    pageNumber,
    pageSize,
    selectedList,
    selectedIds,
    list,
    filterOptionsQuery,
    listQuery,
    exportMutation,
    deleteMutation,
    whatsappMutation,
    transferMutation,
    createCircleMutation,
    deleteTarget,
    setDeleteTarget,
    testsTarget,
    setTestsTarget,
    reviewsTarget,
    setReviewsTarget,
    whatsappOpen,
    setWhatsappOpen,
    createCircleOpen,
    setCreateCircleOpen,
    transferOpen,
    setTransferOpen,
    actionMessage,
    setActionMessage,
    applyFilters,
    handleClearFilters,
    toggleStudent,
    selectAllOnPage,
    clearSelection,
    setPageSize,
    setPageNumber,
    setAppliedFilters,
    circleQuery,
  }
}
