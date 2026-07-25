export const QR_COLOR_OPTIONS = [
  { value: '#7C8738', label: '🔵 أزرق' },
  { value: '#000000', label: '⚫ أسود' },
  { value: '#28a745', label: '🟢 أخضر' },
  { value: '#dc3545', label: '🔴 أحمر' },
  { value: '#ffc107', label: '🟡 أصفر' },
  { value: '#e83e8c', label: '🩷 وردي' },
] as const

export type ReadyQrType = 'parent' | 'teacher' | 'mosque' | 'maleRegister' | 'femaleRegister'

export interface ReadyQrDefinition {
  url: string
  title: string
  description: string
  defaultColor: string
  defaultColorOrder: readonly string[]
}

export const READY_QR_DEFINITIONS: Record<ReadyQrType, ReadyQrDefinition> = {
  parent: {
    url: 'https://parent.mosque-mbark-j.com/login.aspx',
    title: 'لوحة أولياء الأمور',
    description: 'رابط دخول أولياء الأمور للمتابعة',
    defaultColor: '#7C8738',
    defaultColorOrder: ['#7C8738', '#000000', '#28a745', '#dc3545', '#ffc107', '#e83e8c'],
  },
  teacher: {
    url: 'https://teacher.mosque-mbark-j.com/login.aspx',
    title: 'لوحة المعلمين',
    description: 'رابط دخول المعلمين لإدارة الحلقات',
    defaultColor: '#28a745',
    defaultColorOrder: ['#28a745', '#7C8738', '#000000', '#dc3545', '#ffc107', '#e83e8c'],
  },
  mosque: {
    url: 'https://mosque-mbark-j.com/index.aspx',
    title: 'موقع المسجد',
    description: 'الموقع الرسمي لمسجد الشيخ مبارك عبدالله المبارك الصباح',
    defaultColor: '#000000',
    defaultColorOrder: ['#000000', '#7C8738', '#28a745', '#dc3545', '#ffc107', '#e83e8c'],
  },
  maleRegister: {
    url: 'https://mosque-mbark-j.com/index.aspx?q=MRegister',
    title: 'تسجيل الذكور',
    description: 'رابط تسجيل الطلاب الذكور',
    defaultColor: '#dc3545',
    defaultColorOrder: ['#dc3545', '#7C8738', '#000000', '#28a745', '#ffc107', '#e83e8c'],
  },
  femaleRegister: {
    url: 'https://mosque-mbark-j.com/index.aspx?q=WRegister',
    title: 'تسجيل الإناث',
    description: 'رابط تسجيل الطالبات الإناث',
    defaultColor: '#e83e8c',
    defaultColorOrder: ['#e83e8c', '#7C8738', '#000000', '#28a745', '#dc3545', '#ffc107'],
  },
}

export const QR_COLOR_PREVIEW_CLASS: Record<string, string> = {
  '#7C8738': 'bg-[#7C8738]',
  '#000000': 'bg-black',
  '#28a745': 'bg-[#28a745]',
  '#dc3545': 'bg-[#dc3545]',
  '#ffc107': 'bg-[#ffc107]',
  '#e83e8c': 'bg-[#e83e8c]',
}

export const CUSTOM_QR_SIZE = 300
export const READY_QR_SIZE = 200

export const STUDENT_QR_SIZE = 200
export const STUDENT_QR_DEFAULT_COLOR = '#7C8738'
export const STUDENT_QR_FILENAME_PREFIX = 'student-qr'
