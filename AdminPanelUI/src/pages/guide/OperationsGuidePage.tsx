import type { ReactNode } from 'react'
import { BookOpen } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Card } from '@/components/ui/card'

interface SectionProps {
  id: string
  title: string
  children: ReactNode
}

const sections = [
  { id: 'first-setup', title: 'الإعداد الأول ومدير النظام' },
  { id: 'integrations', title: 'التكاملات (واتساب / أجورا)' },
  { id: 'database', title: 'قاعدة البيانات وبيانات القرآن' },
  { id: 'firebase', title: 'الإشعارات (Firebase)' },
  { id: 'mobile', title: 'تطبيق الجوال (Flutter / Codemagic)' },
  { id: 'domain', title: 'تغيير النطاق' },
  { id: 'backup', title: 'النسخ الاحتياطي والاستعادة' },
  { id: 'troubleshooting', title: 'حل المشكلات' },
  { id: 'checklist', title: 'قائمة تحقق لعميل جديد' },
]

function Section({ id, title, children }: SectionProps) {
  return (
    <Card id={id} className="scroll-mt-24 space-y-4">
      <h2 className="border-b pb-2 text-lg font-bold text-slate-900">{title}</h2>
      <div className="space-y-3 text-sm leading-relaxed text-slate-700">{children}</div>
    </Card>
  )
}

function Code({ children }: { children: ReactNode }) {
  return (
    <pre
      dir="ltr"
      className="overflow-x-auto rounded-lg bg-slate-900 p-4 text-left font-mono text-xs text-slate-100"
    >
      {children}
    </pre>
  )
}

function Inline({ children }: { children: ReactNode }) {
  return (
    <code dir="ltr" className="rounded bg-slate-100 px-1.5 py-0.5 font-mono text-xs text-slate-800">
      {children}
    </code>
  )
}

function Steps({ items }: { items: ReactNode[] }) {
  return (
    <ol className="list-inside list-decimal space-y-2">
      {items.map((item, index) => (
        <li key={index}>{item}</li>
      ))}
    </ol>
  )
}

function Bullets({ items }: { items: ReactNode[] }) {
  return (
    <ul className="list-inside list-disc space-y-2">
      {items.map((item, index) => (
        <li key={index}>{item}</li>
      ))}
    </ul>
  )
}

function Rows({ rows }: { rows: [ReactNode, ReactNode][] }) {
  return (
    <div className="divide-y rounded-lg border">
      {rows.map(([label, value], index) => (
        <div key={index} className="grid gap-1 p-3 sm:grid-cols-[220px_1fr] sm:gap-4">
          <div className="font-semibold text-slate-900">{label}</div>
          <div>{value}</div>
        </div>
      ))}
    </div>
  )
}

export function OperationsGuidePage() {
  return (
    <div>
      <PageHeader
        title="دليل التشغيل"
        description="كل ما يلزم لتجهيز نسخة جديدة لعميل وتشغيلها وصيانتها"
        icon={BookOpen}
      />

      <Card className="mb-6">
        <h2 className="mb-3 text-sm font-semibold text-slate-900">المحتويات</h2>
        <div className="flex flex-wrap gap-2">
          {sections.map((section) => (
            <a
              key={section.id}
              href={`#${section.id}`}
              className="rounded-full border px-3 py-1 text-xs text-slate-700 transition hover:border-[var(--color-primary)] hover:text-[var(--color-primary)]"
            >
              {section.title}
            </a>
          ))}
        </div>
        <p className="mt-3 text-xs text-slate-500">
          تفاصيل تجهيز الخادم (IIS، DNS، SSL، النشر) موجودة في ملف <Inline>DEPLOYMENT.md</Inline>{' '}
          داخل المشروع.
        </p>
      </Card>

      <div className="space-y-6">
        <Section id="first-setup" title="الإعداد الأول ومدير النظام">
          <p>
            بعد تنصيب نسخة جديدة، أنشئ حساب مدير من صفحة «المعلمين» (أو عبر استعادة قاعدة بيانات
            فيها حساب موجود)، ثم سجّل الدخول من <Inline>/login</Inline> بالبريد وكلمة المرور.
          </p>
          <Rows
            rows={[
              ['اسم المسجد والشعار واللون', 'من صفحة «الإعدادات» بعد تسجيل الدخول'],
              [
                'مدير النظام',
                'معلم بصلاحية إدارة المستخدمين — البريد هو اسم المستخدم عند الدخول',
              ],
              ['روابط المتاجر', 'اختيارية من «الإعدادات» — تظهر في الموقع العام'],
            ]}
          />
          <p>
            أنشئ بقية المستخدمين من صفحة «المعلمين» مع تفعيل صلاحية إدارة المستخدمين لمن يحتاجها.
          </p>
        </Section>

        <Section id="integrations" title="التكاملات (واتساب / أجورا)">
          <p>
            من صفحة «التكاملات» يمكن إدخال مفاتيح العميل دون تعديل ملفات الخادم. القيم تُحفظ في قاعدة
            البيانات وتتجاوز ما في <Inline>appsettings.json</Inline>، وتُعرض مُقنّعة بعد الحفظ (اترك
            الحقل فارغاً للإبقاء على القيمة الحالية).
          </p>
          <Rows
            rows={[
              ['Wasender Api Token', 'إرسال رسائل واتساب'],
              ['Wasender Session API Key', 'إدارة الجلسة والربط'],
              ['Agora App Id', 'المكالمات المرئية'],
              ['Agora App Certificate', 'سر الخادم لتوليد رموز المكالمات'],
            ]}
          />
          <p>
            مفتاح الواتساب وحده لا يكفي: يجب أيضاً ربط جلسة من صفحة «ربط الواتساب» ومتابعة حالتها.
            تغييرات أجورا يلتقطها تطبيق الجوال خلال دقائق دون إعادة تشغيل.
          </p>
        </Section>

        <Section id="database" title="قاعدة البيانات وبيانات القرآن">
          <Bullets
            items={[
              <>
                عند أول تشغيل ينشئ AdminAPI الجداول تلقائياً إذا كان{' '}
                <Inline>Deployment:EnsureDatabase</Inline> يساوي <Inline>true</Inline>.
              </>,
              <>
                <strong>بيانات القرآن لا تُنشأ تلقائياً.</strong> الحفظ والمراجعة والخطط تحتاج جداول
                القرآن المرجعية (السور، الآيات، الصفحات، مستويات الخطة).
              </>,
              <>
                الحل الموصى به: استعادة نسخة احتياطية من قاعدة بيانات جاهزة، ثم تفريغ بيانات العميل
                السابق (الطلاب، المعلمين، الحلقات) والإبقاء على الجداول المرجعية.
              </>,
            ]}
          />
          <p>استعادة نسخة احتياطية:</p>
          <Code>{`RESTORE DATABASE NewMasgedTeacherAPIDB
FROM DISK = 'C:\\Backups\\seed.bak'
WITH MOVE 'NewMasgedTeacherAPIDB' TO 'C:\\SQLData\\NewMasgedTeacherAPIDB.mdf',
     MOVE 'NewMasgedTeacherAPIDB_log' TO 'C:\\SQLData\\NewMasgedTeacherAPIDB_log.ldf',
     REPLACE;`}</Code>
          <p>
            بعد الاستعادة راجع صفحة «الإعدادات» وعدّل اسم المسجد والشعار واللون وروابط المتاجر
            لتناسب العميل الجديد.
          </p>
        </Section>

        <Section id="firebase" title="الإشعارات (Firebase)">
          <p>
            كل عميل يحتاج مشروع Firebase خاصاً به. ملف حساب الخدمة غير مرفوع مع الكود ويجب وضعه
            يدوياً بجانب ملفات الـ API.
          </p>
          <Steps
            items={[
              <>
                من Firebase Console: أنشئ مشروعاً، ثم{' '}
                <Inline>Project settings → Service accounts → Generate new private key</Inline>.
              </>,
              <>
                انسخ الملف باسم <Inline>firebase-service-account.json</Inline> إلى مجلد نشر AdminAPI
                (وكذلك MasgedParentMobileAPI إن كان يرسل إشعارات).
              </>,
              <>
                عدّل <Inline>appsettings.json</Inline> ثم أعد تشغيل الموقع (Recycle):
              </>,
            ]}
          />
          <Code>{`"Firebase": {
  "Enabled": true,
  "ProjectId": "customer-firebase-project-id",
  "ServiceAccountJsonPath": "firebase-service-account.json"
}`}</Code>
          <Bullets
            items={[
              <>
                تطبيق الجوال يحتاج ملفاته الخاصة:{' '}
                <Inline>google-services.json</Inline> لأندرويد و{' '}
                <Inline>GoogleService-Info.plist</Inline> لـ iOS، من نفس مشروع Firebase.
              </>,
              <>لـ iOS يجب رفع مفتاح APNs في إعدادات مشروع Firebase وإلا لن تصل الإشعارات.</>,
              <>إذا كان الملف مفقوداً تعمل الأنظمة بشكل طبيعي وتتخطى إرسال الإشعارات فقط.</>,
            ]}
          />
        </Section>

        <Section id="mobile" title="تطبيق الجوال (Flutter / Codemagic)">
          <p>
            التطبيق يُبنى لكل عميل بعناوين API الخاصة به عبر <Inline>--dart-define</Inline>، فلا يوجد
            بناء واحد يخدم جميع العملاء.
          </p>
          <Code>{`flutter build appbundle --release \\
  --dart-define=API_BASE_URL=https://api.customer.com \\
  --dart-define=MEDIA_BASE_URL=https://admin.customer.com/ \\
  --dart-define=PRIVACY_POLICY_URL=https://customer.com/privacy-policy`}</Code>
          <p>توليد أيقونات التطبيق من شعار العميل:</p>
          <Code>{`cd ParentApp
.\\tool\\generate_store_icons.ps1 -LogoPath C:\\logos\\customer.png -BackgroundColor "#071B3A"`}</Code>
          <Bullets
            items={[
              <>
                مقاسات صور المتجر (أيقونة ٥١٢، صورة الغلاف ١٠٢٤×٥٠٠، لقطات الشاشة) موجودة في{' '}
                <Inline>google-play/templates/icon-assets.md</Inline>.
              </>,
              <>
                انسخ <Inline>ParentApp/codemagic.yaml.example</Inline> إلى{' '}
                <Inline>codemagic.yaml</Inline> واستبدل قيم <Inline>CUSTOMER_*</Inline> ومعرّف الحزمة{' '}
                <Inline>com.customer.app</Inline> وعناوين الـ API.
              </>,
              <>
                أسرار التوقيع (شهادات Apple، مفتاح Google Play، keystore) تُضبط من واجهة Codemagic
                وليس داخل الملف.
              </>,
              <>
                بعد نشر التطبيقين، ضع روابط المتجرين في «الإعدادات» لتظهر في الموقع العام.
              </>,
            ]}
          />
        </Section>

        <Section id="domain" title="تغيير النطاق">
          <p>تغيير النطاق الفعلي يحتاج الخطوات التالية بالترتيب:</p>
          <Steps
            items={[
              <>
                DNS: سجلات A للنطاق الجديد <Inline>@</Inline> و <Inline>www</Inline> و{' '}
                <Inline>admin</Inline> و <Inline>api</Inline> نحو عنوان الخادم.
              </>,
              <>إصدار شهادة SSL للنطاقات الأربعة وربطها في IIS.</>,
              <>
                تحديث <Inline>appsettings.json</Inline> في الـ API‏ين:{' '}
                <Inline>Cors:Origins</Inline> و <Inline>PublicSite:BaseUrl</Inline> و{' '}
                <Inline>ApiSettings:MediaBaseUrl</Inline>.
              </>,
              <>
                تحديث ملفات <Inline>.env</Inline> للواجهتين ثم <strong>إعادة بنائهما ونشرهما</strong>{' '}
                — عناوين API تُدمج داخل الحزمة وقت البناء.
              </>,
              <>
                إعادة بناء تطبيق الجوال بعناوين جديدة ونشر تحديث في المتجرين — النسخ القديمة على
                أجهزة المستخدمين ستتوقف عن العمل إذا أُغلق النطاق القديم.
              </>,
            ]}
          />
          <Alert variant="destructive">
            أبقِ النطاق القديم يعمل (إعادة توجيه) لفترة انتقالية حتى يحدّث معظم المستخدمين التطبيق.
          </Alert>
        </Section>

        <Section id="backup" title="النسخ الاحتياطي والاستعادة">
          <p>ثلاثة أشياء يجب نسخها احتياطياً:</p>
          <Rows
            rows={[
              ['قاعدة البيانات', 'نسخة كاملة يومية + نسخ سجل المعاملات إن كان وضع الاسترداد Full'],
              [
                'مجلدات الملفات',
                <>
                  <Inline>Uploads</Inline> و <Inline>FilesManager</Inline> داخل مجلد نشر AdminAPI
                </>,
              ],
              [
                'ملفات الإعداد',
                <>
                  <Inline>appsettings.json</Inline> و{' '}
                  <Inline>firebase-service-account.json</Inline> — تحتوي أسراراً لا يمكن توليدها
                  مجدداً
                </>,
              ],
            ]}
          />
          <Code>{`BACKUP DATABASE NewMasgedTeacherAPIDB
TO DISK = 'D:\\Backups\\masged_$(date).bak'
WITH INIT, COMPRESSION, CHECKSUM;`}</Code>
          <Bullets
            items={[
              'اجدول النسخ عبر SQL Server Agent أو Task Scheduler، واحتفظ بنسخة خارج الخادم.',
              'اختبر الاستعادة على خادم تجريبي مرة كل فترة — نسخة لم تُختبر ليست نسخة.',
              'قبل أي تحديث للنظام: خذ نسخة كاملة من قاعدة البيانات ومجلدات الملفات.',
            ]}
          />
        </Section>

        <Section id="troubleshooting" title="حل المشكلات">
          <Rows
            rows={[
              [
                'جداول ناقصة بعد التنصيب',
                <>
                  <Inline>Deployment:EnsureDatabase</Inline> يجب أن يكون <Inline>true</Inline> عند
                  أول تشغيل
                </>,
              ],
              [
                'ميزات القرآن فارغة',
                'الجداول المرجعية لم تُستعد — راجع قسم قاعدة البيانات',
              ],
              [
                'الصفحات ترجع 404 عند التحديث',
                <>
                  ينقص URL Rewrite أو قاعدة الرجوع إلى <Inline>index.html</Inline> في{' '}
                  <Inline>web.config</Inline>
                </>,
              ],
              [
                'الواجهة تنادي نطاقاً خاطئاً',
                <>
                  قيم <Inline>.env</Inline> كانت خاطئة وقت البناء — أعد بناء الواجهة
                </>,
              ],
              [
                'خطأ 500.19 أو 500.30 في IIS',
                'حزمة ASP.NET Core Hosting Bundle غير منصّبة، أو مجمّع التطبيقات ليس No Managed Code',
              ],
              [
                'رفع الملفات يفشل',
                <>
                  هوية مجمّع التطبيقات تحتاج صلاحية تعديل على <Inline>Uploads</Inline> و{' '}
                  <Inline>FilesManager</Inline>
                </>,
              ],
              [
                'الشعار لا يظهر',
                <>
                  تحقق من <Inline>VITE_UPLOADS_BASE_URL</Inline> ومن وصول المسار{' '}
                  <Inline>/uploads</Inline> إلى الـ API
                </>,
              ],
              [
                'الإشعارات لا تصل',
                <>
                  <Inline>Firebase:Enabled</Inline> و <Inline>ProjectId</Inline> وملف حساب الخدمة —
                  ولـ iOS مفتاح APNs
                </>,
              ],
              [
                'رسائل الواتساب لا تُرسل',
                'المفتاح موجود لكن الجلسة غير مربوطة — افتح «ربط الواتساب»',
              ],
              [
                'المكالمات المرئية لا تعمل',
                <>
                  تحقق من <Inline>Agora App Id</Inline> و <Inline>App Certificate</Inline> في
                  «التكاملات»
                </>,
              ],
              [
                'تسجيل الدخول يفشل',
                'اسم المستخدم هو البريد الإلكتروني — راجع سجلات الـ API',
              ],
            ]}
          />
        </Section>

        <Section id="checklist" title="قائمة تحقق لعميل جديد">
          <Bullets
            items={[
              'سجلات DNS للنطاقات الأربعة تشير إلى الخادم',
              'ASP.NET Core Hosting Bundle 8 و URL Rewrite منصّبان على IIS',
              'قاعدة البيانات منشأة وبيانات القرآن المرجعية مستعادة',
              'ملفات ‎.env‎ للواجهتين مضبوطة على نطاق العميل قبل البناء',
              'تنفيذ publish-all.ps1 ونشر المشاريع الأربعة',
              'أسرار فريدة لكل عميل (JWT، StudentQr، Chat) في الـ API‏ين',
              'شهادات SSL مُصدَرة ومربوطة',
              'إنشاء مدير النظام من «المعلمين» وضبط «الإعدادات»',
              'إدخال مفاتيح واتساب وأجورا في «التكاملات» وربط جلسة الواتساب',
              'ملف Firebase موضوع والإشعارات مُختبرة',
              'بناء تطبيق الجوال بعناوين العميل ونشره، ثم إضافة روابط المتجرين في «الإعدادات»',
              'جدولة النسخ الاحتياطي واختبار استعادة واحدة',
            ]}
          />
        </Section>
      </div>
    </div>
  )
}
