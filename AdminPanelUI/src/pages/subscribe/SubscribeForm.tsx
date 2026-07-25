import { Controller, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Alert } from '@/components/ui/alert'
import {
  subscribeSchema,
  type SubscribeFormValues,
} from '@/pages/subscribe/subscribeSchema'
import type { UseMutationResult } from '@tanstack/react-query'
import type { SubmitSubscribePayload } from '@/types/subscribe'

type Props = {
  submitMutation: UseMutationResult<unknown, Error, SubmitSubscribePayload>
  submitErrorMessage: string | null
  onSuccess: () => void
}

export function SubscribeForm({ submitMutation, submitErrorMessage, onSuccess }: Props) {
  const {
    register,
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<SubscribeFormValues>({
    resolver: zodResolver(subscribeSchema),
    defaultValues: { fullName: '', mobile: '' },
  })

  const onSubmit = handleSubmit(async (values) => {
    await submitMutation.mutateAsync({
      fullName: values.fullName.trim(),
      mobile: values.mobile.trim(),
    })
    onSuccess()
  })

  return (
    <form onSubmit={onSubmit}>
      {submitErrorMessage && (
        <Alert variant="destructive" className="mb-5">
          {submitErrorMessage}
        </Alert>
      )}

      <div className="form-group">
        <label className="form-label" htmlFor="fullName">
          <span className="required">*</span>
          الأسم الثلاثي
        </label>
        <input
          id="fullName"
          type="text"
          maxLength={200}
          placeholder="أدخل الاسم الثلاثي"
          className={`form-control${errors.fullName ? ' error' : ''}`}
          {...register('fullName')}
        />
        {errors.fullName && (
          <div className="error-message">{errors.fullName.message}</div>
        )}
      </div>

      <div className="form-group">
        <label className="form-label" htmlFor="mobile">
          <span className="required">*</span>
          رقم الموبايل
        </label>
        <div className="mobile-prefix">
          <span className="mobile-prefix-label">+965</span>
          <Controller
            name="mobile"
            control={control}
            render={({ field }) => (
              <input
                id="mobile"
                type="text"
                inputMode="numeric"
                maxLength={8}
                placeholder="XXXXXXXX"
                className={`form-control mobile-prefix-input${errors.mobile ? ' error' : ''}`}
                value={field.value}
                onBlur={field.onBlur}
                onChange={(e) =>
                  field.onChange(e.target.value.replace(/\D/g, '').slice(0, 8))
                }
              />
            )}
          />
        </div>
        {errors.mobile && (
          <div className="error-message">{errors.mobile.message}</div>
        )}
      </div>

      <button
        type="submit"
        className="btn-submit"
        disabled={submitMutation.isPending}
      >
        {submitMutation.isPending ? 'جاري التسجيل...' : 'تسجيل'}
      </button>
    </form>
  )
}
