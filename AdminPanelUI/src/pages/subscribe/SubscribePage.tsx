import { useEffect, useState } from 'react'
import { useSubscribe } from '@/hooks/useSubscribe'
import { SubscribeForm } from '@/pages/subscribe/SubscribeForm'
import '@/pages/subscribe/subscribe.css'

export function SubscribePage() {
  const [showSuccess, setShowSuccess] = useState(false)
  const { submitMutation, getSubmitErrorMessage } = useSubscribe()
  const submitErrorMessage = submitMutation.isError
    ? getSubmitErrorMessage(submitMutation.error)
    : null

  useEffect(() => {
    document.title = 'التسجيل في الدورس والدورات التعليمية'
  }, [])

  const handleSuccess = () => {
    setShowSuccess(true)
    window.scrollTo(0, 0)
  }

  return (
    <div className="subscribe-page" dir="rtl">
      <div className="container">
        <div className="header">
          <h1>التسجيل في الدورس والدورات التعليمية</h1>
          <p>يرجى تسجيل البيانات الآتية</p>
        </div>

        {showSuccess && (
          <div className="success-message">
            <strong>تم التسجيل بنجاح!</strong>
            <p>شكراً لك على التسجيل. سيتم التواصل معك قريباً.</p>
          </div>
        )}

        {!showSuccess && (
          <SubscribeForm
            submitMutation={submitMutation}
            submitErrorMessage={submitErrorMessage}
            onSuccess={handleSuccess}
          />
        )}
      </div>
    </div>
  )
}
