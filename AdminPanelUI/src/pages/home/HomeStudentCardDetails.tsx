import { InfoRow } from '@/pages/home/HomeStudentCardParts'
import type { HomeStudentListItem } from '@/types/home'

function displayValue(value: string | number | null | undefined, fallback = '—') {
  if (value === null || value === undefined || value === '') {
    return fallback
  }

  return String(value)
}

export function HomeStudentCardDetails({ item }: { item: HomeStudentListItem }) {
  return (
    <div className="space-y-2">
      <InfoRow label="اسم الأب" value={displayValue(item.fatherName)} />
      <InfoRow label="الحلقة" value={displayValue(item.circleName)} />
      <InfoRow label="المستوى" value={item.planLevelName} highlight />
      <InfoRow label="الجنس" value={displayValue(item.studentGender)} />
      <InfoRow label="تاريخ الميلاد" value={displayValue(item.birthdate)} />
      <InfoRow label="العمر" value={`${item.age} سنة`} />
      <InfoRow label="هاتف ولي الأمر" value={displayValue(item.fatherPhone)} />
      <InfoRow label="هاتف ولي الأمر 2" value={displayValue(item.fatherPhone2)} />
      <InfoRow label="هاتف الطالب" value={displayValue(item.studentPhone)} />
      <InfoRow label="تاريخ التسجيل" value={displayValue(item.createdAt)} />
      <InfoRow label="طالب مميز" value={item.isSpecial} highlight={item.isSpecial === 'نعم'} />
      <InfoRow label="طالب نخبة" value={item.isElite} highlight={item.isElite === 'نعم'} />
      <InfoRow label="الاستمارة مكتملة" value={item.completeFollowup} />
      <InfoRow label="مرات الغياب" value={String(item.leaveCount)} />
      {item.womanActivityType ? <InfoRow label="نوع التسجيل" value={item.womanActivityType} /> : null}
      {item.learnCertificate ? <InfoRow label="المؤهل العلمي" value={item.learnCertificate} /> : null}
    </div>
  )
}
