export interface ParentsFollowup {
  studentId: number
  studentName: string
  birthdate: string | null
  studentGender: string
  fatherName: string
  fatherPhone: string
  address: string | null
  maritalStatus: string | null
  healthCondition: string | null
  healthDetails: string | null
  learningDifficulties: string | null
  learningDifficultiesNotes: string | null
  photoUrl: string | null
}

export interface SaveParentsFollowupPayload {
  studentName: string
  birthdate: string
  studentGender: string
  fatherName: string
  fatherPhone: string
  address: string
  maritalStatus: string
  healthCondition: string
  healthDetails?: string
  learningDifficulties: string
  learningDifficultiesNotes?: string
  photoFile?: File
}
