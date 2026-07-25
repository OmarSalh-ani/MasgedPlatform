import type { Control } from 'react-hook-form'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import type { StudentFormValues } from '@/pages/students/studentFormSchema'
import { STUDENT_GENDER_OPTIONS } from '@/types/student'

interface Props {
  control: Control<StudentFormValues>
  readOnly: boolean
}

export function StudentPersonalFields({ control, readOnly }: Props) {
  return (
    <>
      <FormField
        control={control}
        name="fullName"
        render={({ field }) => (
          <FormItem>
            <FormLabel>اسم الطالب *</FormLabel>
            <FormControl>
              <Input placeholder="أدخل اسم الطالب" disabled={readOnly} {...field} />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <div className="grid gap-4 md:grid-cols-2">
        <FormField
          control={control}
          name="age"
          render={({ field }) => (
            <FormItem>
              <FormLabel>العمر *</FormLabel>
              <FormControl>
                <Input type="number" disabled={readOnly} {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={control}
          name="studentGender"
          render={({ field }) => (
            <FormItem>
              <FormLabel>الجنس *</FormLabel>
              <FormControl>
                <SearchableDropdown
                  disabled={readOnly}
                  options={STUDENT_GENDER_OPTIONS}
                  placeholder="اختر الجنس"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <FormField
          control={control}
          name="fatherPhone"
          render={({ field }) => (
            <FormItem>
              <FormLabel>رقم ولي الأمر *</FormLabel>
              <FormControl>
                <Input placeholder="أدخل رقم الهاتف" disabled={readOnly} {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={control}
          name="alternativePhone"
          render={({ field }) => (
            <FormItem>
              <FormLabel>رقم ولي الأمر 2</FormLabel>
              <FormControl>
                <Input placeholder="أدخل رقم الهاتف البديل (اختياري)" disabled={readOnly} {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </>
  )
}
