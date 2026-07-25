import { useEffect, useState } from 'react'
import { GraduationCap, Search, UserRound } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useStudents2 } from '@/hooks/useStudents2'
import { PUBLIC_SITE_URL } from '@/lib/constants'
import { Students2Cards } from '@/pages/students2/Students2Cards'
import { Students2Filters } from '@/pages/students2/Students2Filters'

export function Students2Page() {
  const [searchInput, setSearchInput] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')

  useEffect(() => {
    const timer = window.setTimeout(() => {
      setAppliedSearch(searchInput.trim())
    }, 300)

    return () => window.clearTimeout(timer)
  }, [searchInput])

  const { listQuery } = useStudents2(appliedSearch)

  const handleClearSearch = () => {
    setSearchInput('')
    setAppliedSearch('')
  }

  if (listQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-32 w-full" />
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (listQuery.isError) {
    return (
      <Alert variant="destructive">حدث خطأ أثناء تحميل بيانات الطلاب. يرجى المحاولة مرة أخرى.</Alert>
    )
  }

  const items = listQuery.data?.items ?? []
  const stats = listQuery.data?.stats
  const hasResults = items.length > 0
  const hasSearchTerm = appliedSearch.length > 0

  return (
    <div>
      <PageHeader
        title="إدارة الطلاب"
        description="عرض وإدارة بيانات جميع الطلاب المسجلين في المسجد"
        gradientClassName="bg-gradient-to-br from-[#143b64] to-[#1e528e]"
        titleClassName="text-4xl"
      />

      {stats && (
        <div className="mb-8 grid gap-4 md:grid-cols-3">
          <StatsCard label="إجمالي الطلاب" value={String(stats.totalStudents)} />
          <StatsCard label="الطلاب الذكور" value={String(stats.maleStudents)} />
          <StatsCard label="الطالبات الإناث" value={String(stats.femaleStudents)} />
        </div>
      )}

      <Students2Filters
        search={searchInput}
        onSearchChange={setSearchInput}
        onClear={handleClearSearch}
      />

      {hasSearchTerm && !hasResults ? (
        <div className="py-16 text-center text-slate-500">
          <Search className="mx-auto mb-4 size-16 text-slate-300" />
          <h4 className="text-xl font-semibold">لا توجد نتائج</h4>
          <p className="mt-2">
            لم يتم العثور على أي طلاب يطابقون البحث &quot;{appliedSearch}&quot;
          </p>
        </div>
      ) : null}

      {hasResults ? <Students2Cards items={items} /> : null}

      {!hasSearchTerm && !hasResults ? (
        <div className="py-16 text-center text-slate-500">
          <UserRound className="mx-auto mb-4 size-16 text-slate-300" />
          <h4 className="text-xl font-semibold">لا يوجد طلاب مسجلين</h4>
          <p className="mt-2">لم يتم تسجيل أي طلاب بعد في النظام</p>
          <a
            href={`${PUBLIC_SITE_URL}/NewRegister.aspx`}
            className="mt-6 inline-flex items-center gap-2 rounded-full bg-[var(--color-primary)] px-6 py-2.5 text-sm font-semibold text-white hover:opacity-90"
          >
            <GraduationCap className="size-4" />
            تسجيل طالب جديد
          </a>
        </div>
      ) : null}
    </div>
  )
}

function StatsCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-2xl bg-white p-5 text-center shadow-md transition hover:-translate-y-0.5 hover:shadow-lg">
      <p className="text-3xl font-bold text-[#143b64]">{value}</p>
      <p className="mt-1 font-medium text-slate-500">{label}</p>
    </div>
  )
}
