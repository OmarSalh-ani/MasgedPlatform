import { z } from 'zod'

export const filesManagerFormSchema = z.object({
  name: z.string().trim().min(1, 'يرجى إدخال اسم الملف'),
  file: z.instanceof(File).optional(),
})

export type FilesManagerFormValues = z.infer<typeof filesManagerFormSchema>
