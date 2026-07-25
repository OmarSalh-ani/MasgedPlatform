export interface TestCertificate {
  testId: number
  studentName: string
  circleName: string
  teacherName: string
  testDate: string
  testFrom: string
  testTo: string
  memorizationScore: number
  tajweedScore: number
  revisionScore: number
}

export interface TestCertificateScores {
  total: number
  grade: string
}
