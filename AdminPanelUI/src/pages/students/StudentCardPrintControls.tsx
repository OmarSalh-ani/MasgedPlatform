import {
  printBack,
  printFront,
  saveBackAsPng,
  saveFrontAsPng,
} from '@/pages/students/studentCardPrintUtils'

interface StudentCardPrintControlsProps {
  circleOptions: string[]
  selectedCircle: string
  onCircleChange: (name: string) => void
  onToggleRuler: () => void
}

export function StudentCardPrintControls({
  circleOptions,
  selectedCircle,
  onCircleChange,
  onToggleRuler,
}: StudentCardPrintControlsProps) {
  return (
    <>
      <div className="print-instructions">
        <h3>تعليمات الطباعة بالألوان</h3>
        <p>
          <strong>
            لضمان طباعة البطاقة بالألوان والتنسيقات الكاملة، يرجى اتباع الخطوات التالية:
          </strong>
        </p>
        <ol>
          <li>اضغط على زر الطباعة أدناه</li>
          <li>في نافذة الطباعة، ابحث عن &quot;خيارات إضافية&quot; أو &quot;More settings&quot;</li>
          <li>فعّل خيار &quot;رسومات الخلفية&quot; أو &quot;Background graphics&quot;</li>
          <li>تأكد من اختيار &quot;طباعة ملونة&quot; أو &quot;Color printing&quot;</li>
          <li>اضغط طباعة</li>
        </ol>
        <p>
          <strong>ملاحظة:</strong> في Chrome: اذهب إلى More settings → فعّل Background graphics
        </p>
      </div>

      <div className="controls">
        <button type="button" className="print-btn" onClick={() => void saveFrontAsPng()}>
          حفظ الوجه الأمامي كصورة PNG
        </button>
        <button type="button" className="print-btn" onClick={() => void saveBackAsPng()}>
          حفظ الوجه الخلفي كصورة PNG
        </button>
        <button type="button" className="print-btn" onClick={onToggleRuler}>
          إظهار/إخفاء المسطرة
        </button>
        <button type="button" className="print-btn" onClick={printFront}>
          طباعة الوجه الأمامي
        </button>
        <button type="button" className="print-btn" onClick={printBack}>
          طباعة الوجه الخلفي
        </button>
      </div>

      <div className="controls circle-selector">
        <div className="info-row">
          <span className="info-label">اسم الحلقة:</span>
          <select
            className="circle-select"
            value={selectedCircle}
            onChange={(e) => onCircleChange(e.target.value)}
          >
            <option value="">—</option>
            {circleOptions.map((name) => (
              <option key={name} value={name}>
                {name}
              </option>
            ))}
          </select>
        </div>
      </div>
    </>
  )
}
