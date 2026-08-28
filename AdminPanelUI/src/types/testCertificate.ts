export interface TestCertificate {
  testId: number
  studentId: number
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

export interface SendTestCertificateNotificationPayload {
  title: string
  body: string
}

export interface SendTestCertificateNotificationResult {
  recipientsResolved: number
  recipientsWithoutTokens: number
  tokensAttempted: number
  successCount: number
  failureCount: number
}
