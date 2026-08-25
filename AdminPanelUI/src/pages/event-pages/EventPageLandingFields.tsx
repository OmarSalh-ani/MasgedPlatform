import type { Control } from 'react-hook-form'
import { EventPageTextField } from '@/pages/event-pages/EventPageTextField'
import type { EventPageFormValues } from '@/pages/event-pages/eventPageFormSchema'

interface EventPageLandingFieldsProps {
  control: Control<EventPageFormValues>
}

export function EventPageLandingFields({ control }: EventPageLandingFieldsProps) {
  return (
    <div className="grid gap-4 md:grid-cols-2">
      <div className="md:col-span-2">
        <EventPageTextField
          control={control}
          name="courseTitle"
          label="عنوان الدورة *"
          maxLength={300}
        />
      </div>
      <div className="md:col-span-2">
        <EventPageTextField
          control={control}
          name="invitationText"
          label="نص الدعوة"
          maxLength={500}
          rows={2}
        />
      </div>
      <EventPageTextField control={control} name="mosqueName" label="اسم المسجد" maxLength={300} />
      <EventPageTextField control={control} name="dateText" label="تاريخ الدورة" maxLength={300} />
      <div className="md:col-span-2">
        <EventPageTextField
          control={control}
          name="subjectText"
          label="موضوع الدورة"
          maxLength={1000}
          rows={2}
        />
      </div>
      <EventPageTextField control={control} name="timeText" label="الوقت" maxLength={300} />
      <EventPageTextField control={control} name="contactPhone" label="رقم التواصل" maxLength={50} />
      <div className="md:col-span-2">
        <EventPageTextField
          control={control}
          name="extraNotes"
          label="ملاحظات إضافية"
          rows={3}
        />
      </div>
      <EventPageTextField
        control={control}
        name="supervisorsText"
        label="المشرفون"
        maxLength={1000}
        rows={2}
      />
      <EventPageTextField
        control={control}
        name="socialAccounts"
        label="حسابات المسجد"
        maxLength={200}
      />
      <div className="md:col-span-2">
        <EventPageTextField
          control={control}
          name="locationNote"
          label="موقع المسجد"
          maxLength={500}
        />
      </div>
    </div>
  )
}
