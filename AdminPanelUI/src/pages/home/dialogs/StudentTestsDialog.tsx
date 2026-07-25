import { DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'
import { useHomeStudentTests } from '@/hooks/useHome'

interface StudentTestsDialogProps {
  open: boolean
  studentId: number | null
  studentName: string
  onOpenChange: (open: boolean) => void
}

export function StudentTestsDialog({ open, studentId, studentName, onOpenChange }: StudentTestsDialogProps) {
  const testsQuery = useHomeStudentTests(open ? studentId : null)

  if (!open || studentId == null) return null

  return (
    <DialogShell title={`اختبارات الطالب: ${studentName}`} onClose={() => onOpenChange(false)}>
      {testsQuery.isLoading ? <p className="text-sm text-slate-600">جاري التحميل...</p> : null}
      {testsQuery.isError ? <p className="text-sm text-red-600">تعذر تحميل الاختبارات</p> : null}
      {!testsQuery.isLoading && !testsQuery.isError && (
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead className="bg-[#7C8738] text-white">
              <tr>
                <th className="px-3 py-2">التاريخ</th>
                <th className="px-3 py-2">النوع</th>
                <th className="px-3 py-2">من</th>
                <th className="px-3 py-2">إلى</th>
                <th className="px-3 py-2">الدرجة</th>
                <th className="px-3 py-2">ملاحظات</th>
              </tr>
            </thead>
            <tbody>
              {(testsQuery.data ?? []).map((test, index) => (
                <tr key={`${test.testName}-${index}`} className="border-t">
                  <td className="px-3 py-2 text-center">{test.testName}</td>
                  <td className="px-3 py-2 text-center">{test.testType}</td>
                  <td className="px-3 py-2 text-center">{test.from}</td>
                  <td className="px-3 py-2 text-center">{test.to}</td>
                  <td className="px-3 py-2 text-center">{test.testDegree}</td>
                  <td className="px-3 py-2 text-center">{test.notes}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {(testsQuery.data ?? []).length === 0 ? <p className="mt-3 text-center text-sm text-slate-500">لا توجد اختبارات</p> : null}
        </div>
      )}
    </DialogShell>
  )
}
