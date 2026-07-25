/** Matches WhatsappSender.aspx addReminderText() */
export const WHATSAPP_REMINDER_TEMPLATE = `السلام عليكم ورحمة الله وبركاته
تذكير الحلقة بعد صلاة المغرب للطالب
{أسم الطالب}
{أسم الحلقة}`

/** Matches WhatsappSender.aspx addRegisterLinkText() */
export function buildWhatsappRegisterLinkTemplate(masgedName: string) {
  return `السلام عليكم ورحمة الله وبركاته
حياكم الله في حلقات ${masgedName}،

نتمنى منكم التكرم بالدخول على الرابط التالي لإكمال بيانات تسجيل {أسم الطالب}، وذلك حتى نتمكن من المتابعة الدقيقة، والحرص على تقديم أفضل رعاية وتعامل مع أبنائنا الطلاب.

📌 رابط التسجيل: {الرابط}

بارك الله فيكم وجزاكم خيرًا على تعاونكم`
}
