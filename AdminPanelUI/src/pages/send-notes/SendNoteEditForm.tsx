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
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { SendNoteFormActions } from '@/pages/send-notes/SendNoteFormActions'
import {
  editSendNoteFormSchema,
  type EditSendNoteFormValues,
} from '@/pages/send-notes/sendNoteFormSchema'
import type { UpdateSendNotePayload } from '@/types/sendNote'

interface SendNoteEditFormProps {
  teacherName: string
  defaultNote: string
  isPending: boolean
  onSubmit: (payload: UpdateSendNotePayload) => void
}

export function SendNoteEditForm({
  teacherName,
  defaultNote,
  isPending,
  onSubmit,
}: SendNoteEditFormProps) {
  const form = useForm<EditSendNoteFormValues>({
    resolver: zodResolver(editSendNoteFormSchema),
    defaultValues: { note: defaultNote },
  })

  const handleSubmit = (values: EditSendNoteFormValues) => {
    onSubmit({ note: values.note.trim() })
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-5">
        <FormItem>
          <FormLabel>المعلم</FormLabel>
          <FormControl>
            <Input readOnly value={teacherName} className="bg-slate-50" />
          </FormControl>
        </FormItem>
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
