import { DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'
import { useOthaiminCenterStudentReviews } from '@/hooks/useOthaiminCenter'

interface OthaiminCenterStudentReviewsDialogProps {
  open: boolean
  studentId: number | null
  studentName: string
  onOpenChange: (open: boolean) => void
}

export function OthaiminCenterStudentReviewsDialog({
  open,
  studentId,
  studentName,
  onOpenChange,
}: OthaiminCenterStudentReviewsDialogProps) {
  const reviewsQuery = useOthaiminCenterStudentReviews(open ? studentId : null)

  if (!open || studentId == null) return null

  return (
    <DialogShell title={`مراجعة الطالب: ${studentName}`} onClose={() => onOpenChange(false)}>
      {reviewsQuery.isLoading ? <p className="text-sm text-slate-600">جاري التحميل...</p> : null}
      {reviewsQuery.isError ? <p className="text-sm text-red-600">تعذر تحميل المراجعة</p> : null}
      {!reviewsQuery.isLoading && !reviewsQuery.isError ? (
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead className="bg-[#7C8738] text-white">
              <tr>
                <th className="px-3 py-2">النوع</th>
                <th className="px-3 py-2">التاريخ</th>
                <th className="px-3 py-2">من</th>
                <th className="px-3 py-2">إلى</th>
                <th className="px-3 py-2">السورة</th>
                <th className="px-3 py-2">تم</th>
                <th className="px-3 py-2">ملاحظات</th>
              </tr>
            </thead>
            <tbody>
              {(reviewsQuery.data ?? []).map((review, index) => (
                <tr key={`${review.createdAt}-${index}`} className="border-t">
                  <td className="px-3 py-2 text-center">{review.reviewType}</td>
                  <td className="px-3 py-2 text-center">{review.createdAt}</td>
                  <td className="px-3 py-2 text-center">{review.testFrom}</td>
                  <td className="px-3 py-2 text-center">{review.testTo}</td>
                  <td className="px-3 py-2 text-center">{review.surahName}</td>
                  <td className="px-3 py-2 text-center">{review.isDone}</td>
                  <td className="px-3 py-2 text-center">{review.displayNotes}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {(reviewsQuery.data ?? []).length === 0 ? <p className="mt-3 text-center text-sm text-slate-500">لا توجد مراجعات</p> : null}
        </div>
      ) : null}
    </DialogShell>
  )
}
