import { useCallback, useEffect, useRef, useState } from 'react'
import {
  SearchableDropdown,
  type SearchableDropdownOption,
} from '@/components/shared/SearchableDropdown'
import { useCurrentStudentsPlanStudents } from '@/hooks/useCurrentStudentsPlans'
import { CURRENT_STUDENT_PLAN_STUDENT_LOOKUP_PAGE_SIZE } from '@/types/currentStudentPlan'

interface CurrentStudentsPlansStudentDropdownProps {
  value: string
  onChange: (value: string) => void
}

function mapStudents(
  items: { id: number; label: string }[],
): SearchableDropdownOption[] {
  return items.map((item) => ({
    value: String(item.id),
    label: item.label,
  }))
}

export function CurrentStudentsPlansStudentDropdown({
  value,
  onChange,
}: CurrentStudentsPlansStudentDropdownProps) {
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState('')
  const [pageNumber, setPageNumber] = useState(1)
  const [options, setOptions] = useState<SearchableDropdownOption[]>([])
  const searchRef = useRef(search)
  searchRef.current = search

  const lookupFilters = open
    ? {
        search: search.trim() || undefined,
        pageNumber,
        pageSize: CURRENT_STUDENT_PLAN_STUDENT_LOOKUP_PAGE_SIZE,
      }
    : null

  const studentsQuery = useCurrentStudentsPlanStudents(lookupFilters)

  useEffect(() => {
    if (!studentsQuery.data) return

    const nextOptions = mapStudents(studentsQuery.data.items)
    const withClearOption = [{ value: '', label: 'جميع الطلاب' }, ...nextOptions]

    if (pageNumber === 1) {
      setOptions(withClearOption)
      return
    }

    setOptions((current) => {
      const existing = new Set(current.map((item) => item.value))
      return [
        ...current,
        ...nextOptions.filter((item) => !existing.has(item.value)),
      ]
    })
  }, [studentsQuery.data, pageNumber])

  useEffect(() => {
    if (!value || options.some((option) => option.value === value)) return

    const selected = studentsQuery.data?.items.find((item) => String(item.id) === value)
    if (!selected) return

    setOptions((current) => {
      if (current.some((option) => option.value === value)) return current
      return [{ value, label: selected.label }, ...current]
    })
  }, [value, options, studentsQuery.data?.items])

  const handleOpenChange = useCallback((nextOpen: boolean) => {
    setOpen(nextOpen)
    if (!nextOpen) return
    setSearch('')
    setPageNumber(1)
    setOptions([])
  }, [])

  const handleSearchChange = useCallback((query: string) => {
    if (query === searchRef.current) return
    setSearch(query)
    setPageNumber(1)
    setOptions([])
  }, [])

  const handleLoadMore = useCallback(() => {
    if (!studentsQuery.data || studentsQuery.isFetching) return
    if (studentsQuery.data.pageNumber >= studentsQuery.data.totalPages) return
    setPageNumber((current) => current + 1)
  }, [studentsQuery.data, studentsQuery.isFetching])

  return (
    <SearchableDropdown
      value={value}
      onChange={onChange}
      options={options}
      placeholder="جميع الطلاب"
      searchPlaceholder="ابحث باسم الطالب..."
      serverSide
      onOpenChange={handleOpenChange}
      onSearchChange={handleSearchChange}
      onLoadMore={handleLoadMore}
      hasMore={(studentsQuery.data?.pageNumber ?? 0) < (studentsQuery.data?.totalPages ?? 0)}
      isLoading={studentsQuery.isFetching}
    />
  )
}
