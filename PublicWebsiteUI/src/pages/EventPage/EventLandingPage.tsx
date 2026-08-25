import axios from 'axios'
import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { PageLoading } from '@/components/ContentPageStates'
import { usePublicEventPage, useSubmitPublicEventPage } from '@/hooks/usePublicEventPage'
import { EventLandingForm } from '@/pages/EventPage/EventLandingForm'
import { EventLandingHero } from '@/pages/EventPage/EventLandingHero'
import { EventLandingInfo } from '@/pages/EventPage/EventLandingInfo'
import { EventLandingTracks } from '@/pages/EventPage/EventLandingTracks'
import type { SubmitEventPageAnswer } from '@/types/eventPage'
import './event-page.css'

function getErrorMessage(error: unknown) {
  if (!axios.isAxiosError(error) || !error.response?.data || typeof error.response.data !== 'object') {
    return 'تعذر إرسال التسجيل'
  }
  const data = error.response.data as Record<string, unknown>
  const errors = data.errors ?? data.Errors
  if (Array.isArray(errors) && typeof errors[0] === 'string') return errors[0]
  const message = data.message ?? data.Message
  return typeof message === 'string' && message ? message : 'تعذر إرسال التسجيل'
}

function toAnswers(values: Record<number, string | string[]>): SubmitEventPageAnswer[] {
  return Object.entries(values).map(([fieldId, value]) =>
    Array.isArray(value)
      ? { fieldId: Number(fieldId), values: value }
      : { fieldId: Number(fieldId), value },
  )
}

export function EventLandingPage() {
  const { slug } = useParams()
  const pageQuery = usePublicEventPage(slug)
  const submitMutation = useSubmitPublicEventPage(slug)
  const [submitted, setSubmitted] = useState(false)

  if (pageQuery.isLoading) return <PageLoading />

  if (pageQuery.isError || !pageQuery.data) {
    return (
      <main className="event-page">
        <div className="container">
          <div className="event-missing">
            <i className="fas fa-file-circle-xmark" aria-hidden="true" />
            <h1>الصفحة غير موجودة</h1>
            <p>قد تكون الصفحة غير منشورة أو أن الرابط غير صحيح.</p>
          </div>
        </div>
      </main>
    )
  }

  const page = pageQuery.data

  return (
    <main className="event-page">
      <div className="container event-page__layout">
        <EventLandingHero page={page} />
        <EventLandingInfo page={page} />
        <EventLandingTracks tracks={page.tracks} />
        {submitted ? (
          <section className="event-success">
            <i className="fas fa-circle-check" aria-hidden="true" />
            <h2>تم التسجيل بنجاح</h2>
            <p>شكراً لتسجيلكم. سيتم التواصل معكم عند الحاجة.</p>
          </section>
        ) : page.isRegistrationOpen ? (
          <EventLandingForm
            fields={page.formFields}
            isSubmitting={submitMutation.isPending}
            errorMessage={submitMutation.isError ? getErrorMessage(submitMutation.error) : null}
            onSubmit={(answers) => {
              submitMutation.mutate(
                { answers: toAnswers(answers) },
                { onSuccess: () => setSubmitted(true) },
              )
            }}
          />
        ) : (
          <section className="event-closed">
            <i className="fas fa-door-closed" aria-hidden="true" />
            <h2>التسجيل مغلق حالياً</h2>
            <p>نعتذر، التسجيل في هذه الدورة مغلق حالياً.</p>
          </section>
        )}
      </div>
    </main>
  )
}
