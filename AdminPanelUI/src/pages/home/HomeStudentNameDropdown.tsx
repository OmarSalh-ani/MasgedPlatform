import { useCallback, useEffect, useRef, useState } from 'react'
import {
  SearchableDropdown,
  type SearchableDropdownOption,
} from '@/components/shared/SearchableDropdown'
import { useHomeStudentNames } from '@/hooks/useHome'
import { HOME_STUDENT_NAME_LOOKUP_PAGE_SIZE } from '@/types/home'

interface HomeStudentNameDropdownProps {
  value: string
  onChange: (value: string) => void
}

function mapStudentNames(items: { name: string }[]): SearchableDropdownOption[] {
  return items.map((item) => ({
    value: item.name,
    label: item.name,
  }))
}

export function HomeStudentNameDropdown({ value, onChange }: HomeStudentNameDropdownProps) {
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
        pageSize: HOME_STUDENT_NAME_LOOKUP_PAGE_SIZE,
      }
    : null

  const namesQuery = useHomeStudentNames(lookupFilters)

  useEffect(() => {
    if (!namesQuery.data) return

    const nextOptions = mapStudentNames(namesQuery.data.items)
    const withClearOption = [{ value: '', label: 'جميع الطلاب' }, ...nextOptions]

    if (pageNumber === 1) {
      setOptions(withClearOption)
      return
    }

    setOptions((current) => {
      const existing = new Set(current.map((item) => item.value))
      return [...current, ...nextOptions.filter((item) => !existing.has(item.value))]
    })
  }, [namesQuery.data, pageNumber])

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
    if (!namesQuery.data || namesQuery.isFetching) return
    if (namesQuery.data.pageNumber >= namesQuery.data.totalPages) return
    setPageNumber((current) => current + 1)
  }, [namesQuery.data, namesQuery.isFetching])

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
      hasMore={(namesQuery.data?.pageNumber ?? 0) < (namesQuery.data?.totalPages ?? 0)}
      isLoading={namesQuery.isFetching}
    />
  )
}
