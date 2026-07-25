import { useEffect, useMemo, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import {
  GENDER_OPTIONS,
  MARITAL_STATUS_OPTIONS,
  YES_NO_OPTIONS,
} from '@/pages/parents-followup/parentsFollowup.constants'
import { ParentsFollowupPhotoBox } from '@/pages/parents-followup/ParentsFollowupPhotoBox'
import { ParentsFollowupRadioGroup } from '@/pages/parents-followup/ParentsFollowupRadioGroup'
import {
  createParentsFollowupSchema,
  toBirthdateInputValue,
  type ParentsFollowupFormValues,
} from '@/pages/parents-followup/parentsFollowupSchema'
import type { SaveParentsFollowupPayload } from '@/types/parentsFollowup'
import type { UseMutationResult, UseQueryResult } from '@tanstack/react-query'
import type { ParentsFollowup } from '@/types/parentsFollowup'

type Props = {
  query: UseQueryResult<ParentsFollowup>
  submitMutation: UseMutationResult<boolean, Error, SaveParentsFollowupPayload>
  submitErrorMessage: string | null
  onSuccess: () => void
}

export function ParentsFollowupForm({
  query,
  submitMutation,
  submitErrorMessage,
  onSuccess,
}: Props) {
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const [photoError, setPhotoError] = useState<string | null>(null)
  const hasExistingPhoto = Boolean(query.data?.photoUrl)
  const schema = useMemo(
    () => createParentsFollowupSchema(hasExistingPhoto),
    [hasExistingPhoto],
  )

  const form = useForm<ParentsFollowupFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      studentName: '',
      birthdate: '',
      studentGender: '',
      address: '',
      fatherName: '',
      fatherPhone: '',
      maritalStatus: '',
      healthCondition: '',
      healthDetails: '',
      learningDifficulties: '',
      learningDifficultiesNotes: '',
    },
  })

  useEffect(() => {
    if (!query.data) return
    const data = query.data
    form.reset({
      studentName: data.studentName,
      birthdate: toBirthdateInputValue(data.birthdate),
      studentGender: data.studentGender,
      address: data.address ?? '',
      fatherName: data.fatherName,
      fatherPhone: data.fatherPhone,
      maritalStatus: data.maritalStatus ?? '',
      healthCondition: data.healthCondition ?? '',
      healthDetails: data.healthDetails ?? '',
      learningDifficulties: data.learningDifficulties ?? '',
      learningDifficultiesNotes: data.learningDifficultiesNotes ?? '',
    })
    if (data.photoUrl) setPreviewUrl(data.photoUrl)
  }, [query.data, form])

  const handlePhotoSelected = (file: File) => {
    setPhotoError(null)
    form.setValue('photoFile', file, { shouldValidate: true })
    setPreviewUrl(URL.createObjectURL(file))
  }

  const onSubmit = form.handleSubmit((values) => {
    submitMutation.mutate(
      {
        studentName: values.studentName.trim(),
        birthdate: values.birthdate,
        studentGender: values.studentGender,
        fatherName: values.fatherName.trim(),
        fatherPhone: values.fatherPhone.trim(),
        address: values.address.trim(),
        maritalStatus: values.maritalStatus,
        healthCondition: values.healthCondition,
        healthDetails: values.healthDetails?.trim() || undefined,
        learningDifficulties: values.learningDifficulties,
        learningDifficultiesNotes: values.learningDifficultiesNotes?.trim() || undefined,
        photoFile: values.photoFile,
      },
      { onSuccess: () => onSuccess() },
    )
  })

  const values = form.watch()
  const { logoUrl } = useMasgedBranding()

  return (
    <form onSubmit={onSubmit}>
      {submitErrorMessage && (
        <div className="parents-followup-alert parents-followup-alert--error">
          {submitErrorMessage}
        </div>
      )}

      <div className="container" id="customForm">
        <div className="header">
          <ParentsFollowupPhotoBox
            previewUrl={previewUrl}
            onFileSelected={handlePhotoSelected}
            onValidationError={setPhotoError}
          />
          <div className="logo-dual">
            <img src={logoUrl} alt="شعار المسجد" />
          </div>
        </div>

        <div className="form-title">استمارة تسجيل الطالب</div>

        <div className="form-content">
          <div className="form-row">
            <label htmlFor="studentName">الاسم الرباعي للطالب :</label>
            <input id="studentName" className="form-input" {...form.register('studentName')} />
          </div>
          {form.formState.errors.studentName && (
            <p className="field-error">{form.formState.errors.studentName.message}</p>
          )}

          <div className="form-row">
            <label htmlFor="birthdate">تاريخ ميلاد الطالب :</label>
            <input id="birthdate" type="date" className="form-input" {...form.register('birthdate')} />
          </div>
          {form.formState.errors.birthdate && (
            <p className="field-error">{form.formState.errors.birthdate.message}</p>
          )}

          <div className="form-row">
            <label>الجنس :</label>
            <ParentsFollowupRadioGroup
              name="studentGender"
              value={values.studentGender}
              options={GENDER_OPTIONS}
              onChange={(v) => form.setValue('studentGender', v, { shouldValidate: true })}
            />
          </div>
          {form.formState.errors.studentGender && (
            <p className="field-error">{form.formState.errors.studentGender.message}</p>
          )}

          <div className="form-row">
            <label htmlFor="address">عنوان السكن :</label>
            <input id="address" className="form-input" {...form.register('address')} />
          </div>
          {form.formState.errors.address && (
            <p className="field-error">{form.formState.errors.address.message}</p>
          )}

          <div className="section-header">بيانات أولياء الأمور</div>

          <div className="form-row">
            <label htmlFor="fatherName">اسم الأب/الأم :</label>
            <input id="fatherName" className="form-input" {...form.register('fatherName')} />
          </div>
          {form.formState.errors.fatherName && (
            <p className="field-error">{form.formState.errors.fatherName.message}</p>
          )}

          <div className="form-row">
            <label htmlFor="fatherPhone">رقم الهاتف :</label>
            <input id="fatherPhone" className="form-input" {...form.register('fatherPhone')} />
          </div>
          {form.formState.errors.fatherPhone && (
            <p className="field-error">{form.formState.errors.fatherPhone.message}</p>
          )}

          <div className="form-row">
            <label>الحالة الاجتماعية :</label>
            <ParentsFollowupRadioGroup
              name="maritalStatus"
              value={values.maritalStatus}
              options={MARITAL_STATUS_OPTIONS}
              onChange={(v) => form.setValue('maritalStatus', v, { shouldValidate: true })}
            />
          </div>
          {form.formState.errors.maritalStatus && (
            <p className="field-error">{form.formState.errors.maritalStatus.message}</p>
          )}

          <div className="section-header">الحالة الصحية والتعليمية</div>

          <div className="form-row-medical">
            <div className="medical-question">
              هل يعاني الطالب من أي حالة صحية أو إعاقة :
              <ParentsFollowupRadioGroup
                name="healthCondition"
                value={values.healthCondition}
                options={YES_NO_OPTIONS}
                onChange={(v) => form.setValue('healthCondition', v, { shouldValidate: true })}
              />
            </div>
            {form.formState.errors.healthCondition && (
              <p className="field-error">{form.formState.errors.healthCondition.message}</p>
            )}
            <div className="medical-details">
              <label htmlFor="healthDetails">إذا كانت الإجابة نعم يرجى التوضيح :</label>
              <input id="healthDetails" className="form-input" {...form.register('healthDetails')} />
            </div>
          </div>

          <div className="form-row-medical">
            <div className="medical-question">
              هل يعاني الطالب من صعوبات تعليمية أو سلوكية :
              <ParentsFollowupRadioGroup
                name="learningDifficulties"
                value={values.learningDifficulties}
                options={YES_NO_OPTIONS}
                onChange={(v) => form.setValue('learningDifficulties', v, { shouldValidate: true })}
              />
            </div>
            {form.formState.errors.learningDifficulties && (
              <p className="field-error">{form.formState.errors.learningDifficulties.message}</p>
            )}
            <div className="medical-details">
              <label htmlFor="learningDifficultiesNotes">إذا كانت الإجابة نعم يرجى التوضيح :</label>
              <input
                id="learningDifficultiesNotes"
                className="form-input"
                {...form.register('learningDifficultiesNotes')}
              />
            </div>
          </div>

          {(photoError || form.formState.errors.photoFile) && (
            <p className="field-error">
              {photoError ?? form.formState.errors.photoFile?.message}
            </p>
          )}

          <div className="section-header">تنويه</div>
          <h4 className="notice-text">
            نهدف من هذا النموذج إلى فهم حالة الطالب الأجتماعية والتعليمية والصحية لتوفير بيئة
            مناسبة وتعامل خاص يدعم احتياجاته ويضمن له أفضل بيئة تربوية وتعليمية .
          </h4>

          <button type="submit" className="submit-btn" disabled={submitMutation.isPending}>
            {submitMutation.isPending ? 'جاري الحفظ...' : 'حفظ البيانات'}
          </button>
        </div>
      </div>
    </form>
  )
}
