export type PrivacyLocale = 'ar' | 'en'

export interface PrivacySection {
  id?: string
  title: string
  paragraphs: string[]
  bullets?: string[]
}

export interface PrivacyPolicyCopy {
  locale: PrivacyLocale
  dir: 'rtl' | 'ltr'
  badge: string
  title: string
  subtitle: string
  homeLabel: string
  lastUpdated: string
  switchLabel: string
  switchLinkText: string
  sections: PrivacySection[]
}

const LAST_UPDATED = '2026-06-01'

export const PRIVACY_POLICY_AR: PrivacyPolicyCopy = {
  locale: 'ar',
  dir: 'rtl',
  badge: 'الخصوصية',
  title: 'سياسة الخصوصية',
  subtitle: 'تطبيق طريق الهدى — مسجد الشيخ مبارك عبدالله المبارك الصباح',
  homeLabel: 'الرئيسية',
  lastUpdated: LAST_UPDATED,
  switchLabel: 'English',
  switchLinkText: 'Read in English',
  sections: [
    {
      title: 'مقدمة',
      paragraphs: [
        'نحن في مسجد الشيخ مبارك عبدالله المبارك الصباح («المسجد»، «نحن») نحترم خصوصيتك. توضّح هذه السياسة كيف نجمع ونستخدم ونحمي المعلومات عند استخدامك موقعنا الإلكتروني وتطبيق «طريق الهدى» للهواتف الذكية (Android / iOS) المخصّص لأولياء الأمور والمعلّمين.',
        'باستخدامك للموقع أو التطبيق، فإنك توافق على الممارسات الموضّحة هنا. إذا لم توافق، يرجى التوقف عن استخدام خدماتنا.',
      ],
    },
    {
      title: 'من نحن',
      paragraphs: [
        'المتحكّم في البيانات: مسجد الشيخ مبارك عبدالله المبارك الصباح، دولة الكويت.',
        'للاستفسارات المتعلقة بالخصوصية: support@mosque-mbark-j.com',
      ],
    },
    {
      title: 'البيانات التي نجمعها',
      paragraphs: ['قد نجمع الأنواع التالية من البيانات حسب كيفية استخدامك للخدمات:'],
      bullets: [
        'بيانات الحساب: الاسم، رقم الهاتف (أولياء الأمور)، البريد الإلكتروني (المعلّمين)، كلمة المرور (مخزّنة بشكل آمن على الخادم).',
        'بيانات الملف الشخصي: العنوان، الحالة الاجتماعية، ومعلومات المتابعة التي تقدّمها طوعاً.',
        'بيانات الطلاب: سجلات الحضور، خطط الحفظ والمراجعة، أرشيف التسميع، والصور المعروضة من نظام المسجد.',
        'رسائل المحادثة بين ولي الأمر والمعلّم ضمن التطبيق.',
        'بيانات الجهاز والإشعارات: رمز FCM للإشعارات، نوع المنصة (Android/iOS)، ومعرّفات تقنية لإيصال الإشعارات.',
        'الموقع الجغرافي (عند منح الإذن): لأوقات الصلاة، اتجاه القبلة، أقرب المساجد، والتحقق من الحضور في نطاق المسجد.',
        'الصوت والفيديو (عند منح الإذن): أثناء مكالمات الفيديو مع المعلّم، أو الأوامر الصوتية للمعلّم.',
        'الصور (عند منح الإذن): لحفظ صفحات القرآن في معرض الجهاز عند طلبك.',
      ],
    },
    {
      title: 'كيف نستخدم بياناتك',
      paragraphs: ['نستخدم البيانات للأغراض التالية فقط:'],
      bullets: [
        'إنشاء الحساب وتسجيل الدخول والتحقق (OTP / بيانات الاعتماد).',
        'عرض ومتابعة تقدّم الطلاب لأولياء الأمور والمعلّمين.',
        'إرسال إشعارات بالمحادثات ومكالمات الفيديو والتنبيهات الإدارية.',
        'تقديم ميزات الصلاة والقبلة والمساجد القريبة.',
        'تحسين استقرار الخدمة ودعم المستخدمين.',
      ],
    },
    {
      title: 'أساس قانوني للمعالجة',
      paragraphs: [
        'نعالج بياناتك لتنفيذ علاقتك مع المسجد (تقديم الخدمة التعليمية)، وبموافقتك عند طلب الأذونات الحساسة (الموقع، الكاميرا، الميكروفون، الإشعارات)، وللمصلحة المشروعة في تأمين أنظمتنا.',
      ],
    },
    {
      title: 'مشاركة البيانات مع أطراف ثالثة',
      paragraphs: [
        'لا نبيع بياناتك ولا نشاركها لأغراض إعلانية. قد تُعالج البيانات بواسطة مزوّدين موثوقين يساعدوننا في تشغيل التطبيق:',
      ],
      bullets: [
        'Google Firebase Cloud Messaging — إيصال الإشعارات (رمز الجهاز وبيانات تقنية).',
        'Agora — مكالمات الفيديو والصوت المباشرة بين المعلّم وولي الأمر.',
        'Google (Speech-to-Text على Android) — تحويل الأوامر الصوتية للمعلّم إلى نص (عند استخدام هذه الميزة).',
        'Google ML Kit — مسح رموز QR محلياً على الجهاز لتسجيل الحضور.',
      ],
    },
    {
      title: 'أذونات التطبيق (Android / iOS)',
      paragraphs: ['يطلب تطبيق «طريق الهدى» الأذونات التالية عند الحاجة فقط:'],
      bullets: [
        'الإنترنت — الاتصال بالخوادم وتنزيل خطوط القرآن عند الطلب.',
        'الإشعارات — تنبيهات المحادثة والمكالمات.',
        'الموقع — الصلاة، القبلة، المساجد، الحضور.',
        'الكاميرا والميكروفون — مكالمات الفيديو ومسح QR.',
        'الصور/التخزين — حفظ صور القرآن في المعرض.',
        'البلوتوث — توجيه صوت المكالمة إلى سماعات (اختياري).',
        'البصمة / Face ID — تسجيل دخول أو تأكيد حضور المعلّم (اختياري).',
      ],
    },
    {
      title: 'بيانات الأطفال',
      paragraphs: [
        'التطبيق موجّه لأولياء الأمور والمعلّمين (18+) وليس للاستخدام المباشر من قبل الأطفال. تُعرض بيانات الطلاب (القُصّر) على ولي الأمر أو المعلّم المخوّل فقط ضمن برامج المسجد التعليمية.',
        'لا نجمع عن قصد بيانات شخصية مباشرة من الأطفال دون مشاركة ولي الأمر أو المؤسسة التعليمية.',
      ],
    },
    {
      title: 'الاحتفاظ بالبيانات',
      paragraphs: [
        'نحتفظ ببيانات الحساب والسجلات التعليمية طوال مدة اشتراكك في برامج المسجد أو حسب ما يقتضيه القانون واللوائح المحلية. قد تُحذف البيانات عند طلبك أو عند إغلاق الحساب وفق سياسات المسجد.',
      ],
    },
    {
      id: 'data-deletion',
      title: 'حقوقك وطلب حذف البيانات',
      paragraphs: [
        'يمكنك في أي وقت:',
      ],
      bullets: [
        'سحب أذونات الجهاز (الموقع، الكاميرا، إلخ) من إعدادات الهاتف.',
        'طلب تصحيح أو تحديث بياناتك من داخل التطبيق (الملف الشخصي) أو بالتواصل معنا.',
        'حذف حسابك وبياناتك الشخصية نهائياً من داخل التطبيق: للأولياء من «حسابي» ← «حذف الحساب»، وللمعلّمين من «الإعدادات» ← «حذف الحساب»، مع تأكيد كلمة المرور.',
      ],
    },
    {
      title: 'الأمان',
      paragraphs: [
        'نستخدم HTTPS لتشفير البيانات أثناء النقل. نطبّق ضوابط وصول داخلية على أنظمتنا. لا يمكن ضمان أمان مطلق على الإنترنت، لكننا نعمل على حماية معلوماتك بمعقولية تجارية.',
      ],
    },
    {
      title: 'التغييرات على هذه السياسة',
      paragraphs: [
        'قد نحدّث هذه السياسة من وقت لآخر. سننشر النسخة المحدّثة على هذه الصفحة مع تاريخ «آخر تحديث». استمرارك في استخدام الخدمات بعد التحديث يعني موافقتك على التغييرات.',
      ],
    },
    {
      title: 'تواصل معنا',
      paragraphs: [
        'لأي سؤال حول الخصوصية أو لطلب حذف البيانات:',
        'البريد الإلكتروني: support@mosque-mbark-j.com',
        'الموقع: https://mosque-mbark-j.com',
      ],
    },
  ],
}

export const PRIVACY_POLICY_EN: PrivacyPolicyCopy = {
  locale: 'en',
  dir: 'ltr',
  badge: 'Privacy',
  title: 'Privacy Policy',
  subtitle: 'Tareeq Al-Huda (Path of Guidance) — Sheikh Mubarak Mosque mobile app & website',
  homeLabel: 'Home',
  lastUpdated: LAST_UPDATED,
  switchLabel: 'العربية',
  switchLinkText: 'اقرأ بالعربية',
  sections: [
    {
      title: 'Introduction',
      paragraphs: [
        'Sheikh Mubarak Abdullah Al-Mubarak Al-Sabah Mosque ("the Mosque", "we", "us") respects your privacy. This policy explains how we collect, use, and protect information when you use our public website and the Tareeq Al-Huda (Path of Guidance) mobile application for parents and teachers.',
        'By using our website or app, you agree to the practices described here. If you do not agree, please discontinue use of our services.',
      ],
    },
    {
      title: 'Who we are',
      paragraphs: [
        'Data controller: Sheikh Mubarak Abdullah Al-Mubarak Al-Sabah Mosque, Kuwait.',
        'Privacy contact: support@mosque-mbark-j.com',
      ],
    },
    {
      title: 'Information we collect',
      paragraphs: ['Depending on how you use our services, we may collect:'],
      bullets: [
        'Account data: name, phone number (parents), email (teachers), password (stored securely on our servers).',
        'Profile data: address, marital status, and follow-up information you provide.',
        'Student records: attendance, memorization plans, revision archive, and photos served from the mosque system.',
        'In-app chat messages between parents and teachers.',
        'Device & notification data: FCM token, platform type (Android/iOS), and technical identifiers to deliver push notifications.',
        'Location (with permission): prayer times, Qibla direction, nearest mosques, and mosque attendance verification.',
        'Audio & video (with permission): during video calls with teachers, or teacher voice commands.',
        'Photos/storage (with permission): saving Quran page images to your gallery when you request it.',
      ],
    },
    {
      title: 'How we use your information',
      paragraphs: ['We use data only to:'],
      bullets: [
        'Create accounts, authenticate users (OTP / credentials), and manage sessions.',
        'Display student progress to authorized parents and teachers.',
        'Send notifications for chat, video calls, and administrative messages.',
        'Provide prayer, Qibla, and mosque finder features.',
        'Maintain service reliability and user support.',
      ],
    },
    {
      title: 'Legal basis',
      paragraphs: [
        'We process data to perform our educational services for mosque participants, with your consent for sensitive device permissions (location, camera, microphone, notifications), and for legitimate interests in securing our systems.',
      ],
    },
    {
      title: 'Third-party services',
      paragraphs: [
        'We do not sell your data or share it for advertising. Trusted processors may handle data to operate the app:',
      ],
      bullets: [
        'Google Firebase Cloud Messaging — push notification delivery (device token and technical metadata).',
        'Agora — live audio/video sessions between teachers and parents.',
        'Google Speech-to-Text (Android) — converts teacher voice commands to text when that feature is used.',
        'Google ML Kit — on-device QR scanning for attendance (camera frames processed locally).',
      ],
    },
    {
      title: 'App permissions',
      paragraphs: ['The Tareeq Al-Huda app requests permissions only when needed:'],
      bullets: [
        'Internet — API access and on-demand Quran font downloads.',
        'Notifications — chat and video call alerts.',
        'Location — prayer times, Qibla, nearby mosques, attendance.',
        'Camera & microphone — video calls and QR attendance.',
        'Photos/storage — save Quran images to the gallery.',
        'Bluetooth — route call audio to headsets (optional).',
        'Biometrics — optional teacher login or attendance confirmation.',
      ],
    },
    {
      title: "Children's data",
      paragraphs: [
        'The app is intended for parents and teachers (18+), not direct use by children. Student (minor) information is shown only to authorized parents or teachers within mosque educational programs.',
        'We do not knowingly collect personal data directly from children without parental or institutional involvement.',
      ],
    },
    {
      title: 'Data retention',
      paragraphs: [
        'We retain account and educational records while you participate in mosque programs or as required by applicable law. Data may be deleted upon your request or account closure according to mosque policies.',
      ],
    },
    {
      id: 'data-deletion',
      title: 'Your rights & data deletion',
      paragraphs: ['You may at any time:'],
      bullets: [
        'Revoke device permissions (location, camera, etc.) in your phone settings.',
        'Update your profile information in the app or by contacting us.',
        'Permanently delete your account and personal data in the app: parents via Profile → Delete Account, teachers via Settings → Delete Account, with password confirmation.',
      ],
    },
    {
      title: 'Security',
      paragraphs: [
        'We use HTTPS to encrypt data in transit and apply internal access controls. No method of transmission over the Internet is 100% secure, but we protect your information using commercially reasonable measures.',
      ],
    },
    {
      title: 'Changes to this policy',
      paragraphs: [
        'We may update this policy from time to time. The revised version will be posted on this page with an updated "Last updated" date. Continued use after changes constitutes acceptance.',
      ],
    },
    {
      title: 'Contact us',
      paragraphs: [
        'For privacy questions or deletion requests:',
        'Email: support@mosque-mbark-j.com',
        'Website: https://mosque-mbark-j.com',
      ],
    },
  ],
}

export function getPrivacyPolicyContent(locale: PrivacyLocale): PrivacyPolicyCopy {
  return locale === 'en' ? PRIVACY_POLICY_EN : PRIVACY_POLICY_AR
}
