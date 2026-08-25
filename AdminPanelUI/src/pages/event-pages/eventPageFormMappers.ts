import { parseOptionsText, type EventPageFormValues } from '@/pages/event-pages/eventPageFormSchema'
import type { EventPage, SaveEventPagePayload } from '@/types/eventPage'

function optional(value?: string) {
  return value?.trim() ? value.trim() : undefined
}

export function toEventPageFormValues(page: EventPage): EventPageFormValues {
  return {
    activityName: page.activityName,
    slug: page.slug,
    courseTitle: page.courseTitle,
    invitationText: page.invitationText ?? '',
    mosqueName: page.mosqueName ?? '',
    subjectText: page.subjectText ?? '',
    dateText: page.dateText ?? '',
    timeText: page.timeText ?? '',
    extraNotes: page.extraNotes ?? '',
    supervisorsText: page.supervisorsText ?? '',
    contactPhone: page.contactPhone ?? '',
    socialAccounts: page.socialAccounts ?? '',
    locationNote: page.locationNote ?? '',
    isPublished: page.isPublished,
    isRegistrationOpen: page.isRegistrationOpen,
    tracks: page.tracks.map((track) => ({
      title: track.title,
      description: track.description ?? '',
      sortOrder: track.sortOrder,
    })),
    formFields: page.formFields.map((field) => ({
      fieldId: field.id,
      label: field.label,
      fieldType: field.fieldType,
      isRequired: field.isRequired,
      sortOrder: field.sortOrder,
      optionsText: field.options.join('\n'),
    })),
  }
}

export function toSaveEventPagePayload(values: EventPageFormValues): SaveEventPagePayload {
  return {
    activityName: values.activityName.trim(),
    slug: values.slug.trim().toLowerCase(),
    courseTitle: values.courseTitle.trim(),
    invitationText: optional(values.invitationText),
    mosqueName: optional(values.mosqueName),
    subjectText: optional(values.subjectText),
    dateText: optional(values.dateText),
    timeText: optional(values.timeText),
    extraNotes: optional(values.extraNotes),
    supervisorsText: optional(values.supervisorsText),
    contactPhone: optional(values.contactPhone),
    socialAccounts: optional(values.socialAccounts),
    locationNote: optional(values.locationNote),
    isPublished: values.isPublished,
    isRegistrationOpen: values.isRegistrationOpen,
    imageFile: values.imageFile,
    tracks: values.tracks.map((track) => ({
      title: track.title.trim(),
      description: optional(track.description),
      sortOrder: track.sortOrder,
    })),
    formFields: values.formFields.map((field) => ({
      id: field.fieldId,
      label: field.label.trim(),
      fieldType: field.fieldType,
      isRequired: field.isRequired,
      sortOrder: field.sortOrder,
      options: parseOptionsText(field.optionsText),
    })),
  }
}
