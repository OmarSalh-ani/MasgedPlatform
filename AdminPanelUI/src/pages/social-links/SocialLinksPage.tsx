import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Plus, Share2 } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useSocialLinks } from '@/hooks/useSocialLinks'
import { SocialLinksTable } from '@/pages/social-links/SocialLinksTable'
import { DeleteSocialLinkDialog } from '@/pages/social-links/dialogs/DeleteSocialLinkDialog'

export function SocialLinksPage() {
  const { query, deleteMutation } = useSocialLinks()
  const [deleteId, setDeleteId] = useState<number | null>(null)

  const handleDeleteConfirm = () => {
    if (deleteId === null) return
    deleteMutation.mutate(deleteId, {
      onSettled: () => setDeleteId(null),
    })
  }

  if (query.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (query.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل قائمة روابط التواصل. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const items = query.data ?? []

  return (
    <div>
      <PageHeader
        title="روابط التواصل الاجتماعي"
        description="إدارة روابط فيسبوك، تويتر، واتساب، إنستغرام، يوتيوب"
        actions={
          <Link
            to="/social-links/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <Plus className="size-4" />
            إضافة رابط
          </Link>
        }
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف الرابط. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <div className="overflow-hidden rounded-xl bg-white shadow-md">
        <div className="bg-[#7C8738] px-5 py-4 font-semibold text-white">قائمة الروابط</div>
        {items.length > 0 ? (
          <SocialLinksTable items={items} onDelete={setDeleteId} />
        ) : (
          <div className="p-8 text-center text-slate-500">
            <Share2 className="mx-auto mb-3 size-12 opacity-50" />
            <p>
              لا توجد روابط.{' '}
              <Link to="/social-links/new" className="text-[var(--color-primary)] underline">
                إضافة رابط
              </Link>
            </p>
          </div>
        )}
      </div>

      <DeleteSocialLinkDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
