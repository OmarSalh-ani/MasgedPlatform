export function applyWhatsappPreviewTokens(message: string): string {
  return message
    .replaceAll('{رقم الطالب}', '12345')
    .replaceAll('{اسم الطالب}', 'أحمد محمد')
    .replaceAll('{اسم الأب}', 'محمد أحمد')
    .replaceAll('{التاريخ}', '15-12-2024')
    .replaceAll('{الوقت}', '10:30 ص')
    .replaceAll('{اسم الحلقة}', 'حلقة الفجر')
    .replaceAll('{اسم المعلم}', 'الشيخ عبدالله')
    .replaceAll('{اسم الاجتماع}', 'اجتماع دار القرآن التفاعلي')
    .replaceAll('{رابط الاجتماع}', 'https://meet.google.com/abc-defg-hij')
}
