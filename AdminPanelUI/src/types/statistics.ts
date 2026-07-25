export interface CircleStatistics {
  totalStudents: number
  presentToday: number
  departedToday: number
  absentToday: number
}

export interface AdditionalStatistics {
  totalTeachers: number
  totalCircles: number
  specialStudents: number
}

export interface StatisticsResponse {
  circleStatistics: CircleStatistics
  additionalStatistics: AdditionalStatistics
}
