import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Textarea } from '@/components/ui/textarea'
import { SendNoteFormActions } from '@/pages/send-notes/SendNoteFormActions'
import { SendNoteTeacherSelect } from '@/pages/send-notes/SendNoteTeacherSelect'
import {
  createSendNoteFormSchema,
  type CreateSendNoteFormValues,
} from '@/pages/send-notes/sendNoteFormSchema'
import type { CreateSendNotePayload, TeacherOption } from '@/types/sendNote'

interface SendNoteCreateFormProps {
  teachers: TeacherOption[]
  isPending: boolean
  onSubmit: (payload: CreateSendNotePayload) => void
}

export function SendNoteCreateForm({ teachers, isPending, onSubmit }: SendNoteCreateFormProps) {
  const [selectedTeacherIds, setSelectedTeacherIds] = useState<number[]>([])

  const form = useForm<CreateSendNoteFormValues>({
    resolver: zodResolver(createSendNoteFormSchema),
    defaultValues: { teacherIds: [], note: '' },
  })

  useEffect(() => {
    form.setValue('teacherIds', selectedTeacherIds, { shouldValidate: true })
  }, [selectedTeacherIds, form])

  const toggleTeacher = (teacherId: number, checked: boolean) => {
    setSelectedTeacherIds((prev) =>
      checked ? [...prev, teacherId] : prev.filter((id) => id !== teacherId),
    )
  }

  const selectAllTeachers = (checked: boolean) => {
    setSelectedTeacherIds(checked ? teachers.map((t) => t.id) : [])
  }

  const handleSubmit = (values: CreateSendNoteFormValues) => {
    onSubmit({
      teacherIds: values.teacherIds,
      note: values.note.trim(),
    })
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-5">
        <FormField
          control={form.control}
          name="teacherIds"
          render={() => (
            <FormItem>
              <FormLabel>اختر المعلمين</FormLabel>
              <SendNoteTeacherSelect
                teachers={teachers}
                selectedTeacherIds={selectedTeacherIds}
                onToggle={toggleTeacher}
                onSelectAll={selectAllTeachers}
              />
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="note"
          render={({ field }) => (
            <FormItem>
              <FormLabel>نص الملاحظة</FormLabel>
              <FormControl>
                <Textarea rows={6} placeholder="اكتب الملاحظة هنا..." {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <SendNoteFormActions isPending={isPending} />
      </form>
    </Form>
  )
}
