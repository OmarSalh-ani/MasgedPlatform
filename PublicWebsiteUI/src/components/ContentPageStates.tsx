export function PageLoading() {
  return (
    <div className="page-state">
      <div className="state-spinner" aria-hidden="true" />
      <p>جاري التحميل...</p>
    </div>
  )
}

export function PageError() {
  return (
    <div className="page-state page-state--error">
      <i className="fas fa-exclamation-circle page-state__icon" aria-hidden="true" />
      <p>تعذر تحميل محتوى الموقع</p>
      <button type="button" className="btn btn-primary" onClick={() => window.location.reload()}>
        إعادة المحاولة
      </button>
    </div>
  )
}
