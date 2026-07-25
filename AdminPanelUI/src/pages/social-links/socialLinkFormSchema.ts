import { z } from 'zod'

export const socialLinkFormSchema = z.object({
  platformName: z
    .string()
    .trim()
    .min(1, 'اسم المنصة مطلوب')
    .max(100, 'اسم المنصة يجب ألا يتجاوز 100 حرفاً'),
  url: z
    .string()
    .trim()
    .min(1, 'الرابط مطلوب')
    .max(500, 'الرابط يجب ألا يتجاوز 500 حرفاً'),
  iconClass: z.string().max(100, 'اسم الأيقونة يجب ألا يتجاوز 100 حرفاً').optional(),
  sortOrder: z.number(),
})

export type SocialLinkFormValues = z.infer<typeof socialLinkFormSchema>
