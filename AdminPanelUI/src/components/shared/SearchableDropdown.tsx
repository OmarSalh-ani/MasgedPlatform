import { ChevronDown } from 'lucide-react'

import { useEffect, useMemo, useRef, useState } from 'react'

import { Input } from '@/components/ui/input'

import { cn } from '@/lib/utils'



export interface SearchableDropdownOption {

  value: string

  label: string

}



interface SearchableDropdownProps {

  value: string

  onChange: (value: string) => void

  onBlur?: () => void

  onOpenChange?: (open: boolean) => void

  options: readonly SearchableDropdownOption[]

  disabled?: boolean

  placeholder?: string

  searchPlaceholder?: string

  emptyMessage?: string

  loadingMessage?: string

  loadMoreMessage?: string

  className?: string

  id?: string

  name?: string

  serverSide?: boolean

  onSearchChange?: (query: string) => void

  onLoadMore?: () => void

  hasMore?: boolean

  isLoading?: boolean

}



export function SearchableDropdown({

  value,

  onChange,

  onBlur,

  onOpenChange,

  options,

  disabled = false,

  placeholder = 'اختر...',

  searchPlaceholder = 'ابحث...',

  emptyMessage = 'لا توجد نتائج',

  loadingMessage = 'جاري التحميل...',

  loadMoreMessage = 'تحميل المزيد...',

  className,

  id,

  name,

  serverSide = false,

  onSearchChange,

  onLoadMore,

  hasMore = false,

  isLoading = false,

}: SearchableDropdownProps) {

  const [open, setOpen] = useState(false)

  const [search, setSearch] = useState('')

  const containerRef = useRef<HTMLDivElement>(null)

  const listRef = useRef<HTMLUListElement>(null)

  const onSearchChangeRef = useRef(onSearchChange)

  onSearchChangeRef.current = onSearchChange



  const selectedLabel = useMemo(

    () => options.find((option) => option.value === value)?.label ?? (value || ''),

    [options, value],

  )



  const filteredOptions = useMemo(() => {

    if (serverSide) return options

    const query = search.trim().toLowerCase()

    if (!query) return options

    return options.filter((option) => option.label.toLowerCase().includes(query))

  }, [options, search, serverSide])



  const setDropdownOpen = (next: boolean) => {

    setOpen(next)

    onOpenChange?.(next)

    if (!next) setSearch('')

  }



  const closeDropdown = () => {

    setDropdownOpen(false)

    onBlur?.()

  }



  useEffect(() => {

    if (!open) return



    const handleClickOutside = (event: MouseEvent) => {

      if (containerRef.current?.contains(event.target as Node)) return

      closeDropdown()

    }



    document.addEventListener('mousedown', handleClickOutside)

    return () => document.removeEventListener('mousedown', handleClickOutside)

  }, [open, onBlur])



  useEffect(() => {
    if (!serverSide || !onSearchChangeRef.current || !open) return

    const timer = window.setTimeout(() => onSearchChangeRef.current?.(search), 300)

    return () => window.clearTimeout(timer)
  }, [search, serverSide, open])



  const handleToggle = () => {

    if (disabled) return

    if (open) {

      closeDropdown()

      return

    }

    setDropdownOpen(true)

  }



  const handleSelect = (optionValue: string) => {

    onChange(optionValue)

    closeDropdown()

  }



  const handleListScroll = () => {

    const element = listRef.current

    if (!element || !serverSide || !hasMore || isLoading || !onLoadMore) return

    if (element.scrollTop + element.clientHeight >= element.scrollHeight - 24) {

      onLoadMore()

    }

  }



  return (

    <div ref={containerRef} className={cn('relative w-full', className)}>

      <button

        type="button"

        id={id}

        name={name}

        disabled={disabled}

        onClick={handleToggle}

        className={cn(

          'flex h-10 w-full items-center justify-between rounded-lg border-2 border-slate-200 bg-white px-3 py-2 text-sm',

          'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[var(--color-primary)]',

          'disabled:cursor-not-allowed disabled:opacity-50',

          !selectedLabel && 'text-slate-400',

        )}

      >

        <span className="truncate">{selectedLabel || placeholder}</span>

        <ChevronDown

          className={cn('size-4 shrink-0 opacity-50 transition-transform', open && 'rotate-180')}

        />

      </button>



      {open && (

        <div className="absolute z-50 mt-1 w-full rounded-lg border-2 border-slate-200 bg-white shadow-lg">

          <div className="border-b border-slate-200 p-2">

            <Input

              autoFocus

              value={search}

              onChange={(event) => setSearch(event.target.value)}

              placeholder={searchPlaceholder}

              className="h-9"

            />

          </div>

          <ul ref={listRef} className="max-h-60 overflow-y-auto py-1" onScroll={handleListScroll}>

            {isLoading && filteredOptions.length === 0 ? (

              <li className="px-3 py-2 text-sm text-slate-500">{loadingMessage}</li>

            ) : filteredOptions.length === 0 ? (

              <li className="px-3 py-2 text-sm text-slate-500">{emptyMessage}</li>

            ) : (

              filteredOptions.map((option) => (

                <li key={option.value}>

                  <button

                    type="button"

                    className={cn(

                      'flex w-full px-3 py-2 text-sm text-start hover:bg-slate-100',

                      option.value === value && 'bg-slate-100 font-medium',

                    )}

                    onClick={() => handleSelect(option.value)}

                  >

                    {option.label}

                  </button>

                </li>

              ))

            )}

            {serverSide && isLoading && filteredOptions.length > 0 ? (

              <li className="px-3 py-2 text-sm text-slate-500">{loadingMessage}</li>

            ) : null}

            {serverSide && hasMore && !isLoading ? (

              <li className="px-3 py-2 text-center text-xs text-slate-400">{loadMoreMessage}</li>

            ) : null}

          </ul>

        </div>

      )}

    </div>

  )

}


