export interface SpecialStudentsReportItem {
  studentName: string
  circleName: string
  fatherPhone: string
  imageUrl: string
  circleId: number | null
}

export interface SpecialStudentsReportStats {
  totalStudents: number
  totalCircles: number
  averagePerCircle: number
}

export interface SpecialStudentsReport {
  items: SpecialStudentsReportItem[]
  stats: SpecialStudentsReportStats
}
