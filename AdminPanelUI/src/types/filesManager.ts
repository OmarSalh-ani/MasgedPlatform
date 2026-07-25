export interface FilesManager {
  id: number
  name: string
  filePath: string
  fileUrl: string
}

export interface FilesManagerListItem {
  id: number
  name: string
  filePath: string
  fileUrl: string
}

export interface SaveFilesManagerPayload {
  name: string
  file: File
}
