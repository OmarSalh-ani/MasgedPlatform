export interface WhatsappPreConfiguredMessage {
  id: number
  event: string
  eventDisplayName: string
  eventDescription: string
  whatsappMessage: string
  isEnabled: boolean
  previewMessage: string
}

export const WHATSAPP_PARAMETER_TAGS = [
  '{رقم الطالب}',
  '{اسم الطالب}',
  '{اسم الأب}',
  '{التاريخ}',
  '{الوقت}',
  '{اسم الحلقة}',
  '{اسم المعلم}',
] as const

export const WHATSAPP_REVISE_TAGS = [
  '{نوع المراجعة}',
  '{اسم السورة}',
  '{من}',
  '{إلى}',
  '{ملاحظات}',
] as const

export const WHATSAPP_TEST_TAGS = [
  '{تاريخ الاختبار}',
  '{اسم السورة}',
  '{حزب رقم}',
  '{من}',
  '{إلى}',
  '{درجة الحفظ}',
  '{درجة التجويد}',
  '{درجة الأداء}',
  '{المجموع}',
  '{التقدير}',
  '{النتيجة النهائية}',
  '{ملاحظات}',
] as const

export const WHATSAPP_MEET_TAGS = ['{رابط الاجتماع}', '{اسم الاجتماع}'] as const
