import {
  printBack,
  printFront,
  saveBackAsPng,
  saveFrontAsPng,
} from '@/pages/teachers/teacherCardPrintUtils'

interface TeacherCardPrintControlsProps {
  onToggleRuler: () => void
}

export function TeacherCardPrintControls({ onToggleRuler }: TeacherCardPrintControlsProps) {
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
    </>
  )
}
