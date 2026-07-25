export function RegistrationClosed() {
  return (
    <section className="reg-page reg-page--closed">
      <div className="container">
        <div className="reg-closed-card">
          <div className="reg-closed-card__icon" aria-hidden="true">
            <i className="fas fa-door-closed" />
          </div>
          <h2>التسجيل مغلق حالياً</h2>
          <p>
            نعتذر، التسجيل في الأنشطة مغلق حالياً. يرجى المحاولة لاحقاً أو التواصل معنا للاستفسار.
          </p>
        </div>
      </div>
    </section>
  )
}
