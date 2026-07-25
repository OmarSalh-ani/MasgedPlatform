export interface Students2ListItem {
  id: number
  name: string
  fatherName: string
  age: number
  gender: string
  fatherPhone: string
  circleName: string
  registrationType: string
  registrationDate: string
  imageUrl: string
}

export interface Students2Stats {
  totalStudents: number
  maleStudents: number
  femaleStudents: number
}

export interface Students2Response {
  items: Students2ListItem[]
  stats: Students2Stats
}

export interface Students2Filters {
  search: string
}
