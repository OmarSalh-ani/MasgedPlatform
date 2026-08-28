import type { TestCertificateScores } from '@/types/testCertificate'

const MAX_POSSIBLE_SCORE = 100

export function formatScore(score: number): string {
  return score > 0 ? Math.round(score).toLocaleString('en-US') : '0'
}

export function calculateTotalAndGrade(
  memorizationScore: number,
  tajweedScore: number,
  revisionScore: number,
): TestCertificateScores {
  const total = Math.round((memorizationScore + tajweedScore + revisionScore) * 100) / 100
  const roundedTotal = Math.round(total)
  const percentage = (total / MAX_POSSIBLE_SCORE) * 100

  let grade = 'ضعيف'
  if (percentage >= 90) grade = 'ممتاز'
  else if (percentage >= 80) grade = 'جيد جداً'
  else if (percentage >= 70) grade = 'جيد'
  else if (percentage >= 60) grade = 'مقبول'

  return { total: roundedTotal, grade }
}

export function buildTestCertificateNotificationDefaults(
  studentName: string,
  grade: string,
): { title: string; body: string } {
  return {
    title: `شهادة اختبار — ${studentName}`,
    body: `تم إصدار شهادة اختبار لـ ${studentName}. التقدير: ${grade}. اضغط لعرض الشهادة.`,
  }
}
