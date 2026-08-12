export interface TeacherListItem {
  id: number
  name: string
  circleCount: number
  mobile: string | null
  email: string
  password: string
  usersManage: boolean
  imageUrl: string | null
}

export interface TeacherCircleOption {
  id: number
  name: string
}

export interface TeacherMosqueOption {
  id: number
  name: string
  googleMapsUrl: string | null
}

export interface TeacherMapLocation {
  url: string
  lat?: string | null
  lng?: string | null
}

export interface Teacher {
  id: number
  name: string
  mobile: string | null
  email: string
  baseSalary: number | null
  usersManage: boolean
  isGirlTeacher: boolean
  isViewOnly: boolean
  isSupervisor: boolean
  imageUrl: string | null
  selectedMosqueIds: number[]
  manualLocations: TeacherMapLocation[]
}

export interface SaveTeacherPayload {
  name: string
  mobile?: string | null
  email: string
  password?: string
  baseSalary?: number | null
  circleId?: number | null
  isGirlTeacher: boolean
  usersManage: boolean
  isViewOnly: boolean
  isSupervisor: boolean
  removeImage?: boolean
  imageFile?: File
  selectedMosqueIds: number[]
  manualLocations: TeacherMapLocation[]
}