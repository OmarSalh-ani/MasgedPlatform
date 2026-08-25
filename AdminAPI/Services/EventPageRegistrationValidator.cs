using AdminAPI.DTOs.PublicEventPages;
using AdminAPI.Models;

namespace AdminAPI.Services;

public static class EventPageRegistrationValidator
{
    public static List<EventPageResponseValue> BuildValues(
        EventPage page,
        IReadOnlyList<SubmitEventPageAnswerDto> answers)
    {
        var answersByField = answers.ToDictionary(a => a.FieldId);
        var values = new List<EventPageResponseValue>();

        foreach (var field in page.FormFields.OrderBy(f => f.SortOrder).ThenBy(f => f.Id))
        {
            answersByField.TryGetValue(field.Id, out var answer);
            var raw = NormalizeAnswer(field, answer);

            if (field.IsRequired && string.IsNullOrWhiteSpace(raw))
                throw new ArgumentException($"الحقل مطلوب: {field.Label}");

            if (string.IsNullOrWhiteSpace(raw))
                continue;

            ValidateTypedValue(field, raw);
            values.Add(new EventPageResponseValue
            {
                FieldId = field.Id,
                FieldLabel = field.Label,
                Value = raw,
            });
        }

        return values;
    }

    private static string NormalizeAnswer(EventPageFormField field, SubmitEventPageAnswerDto? answer)
    {
        if (answer is null)
            return string.Empty;

        if (field.FieldType == EventPageFieldTypes.MultiSelect)
        {
            var selected = (answer.Values ?? [])
                .Select(v => v.Trim())
                .Where(v => v.Length > 0)
                .Distinct()
                .ToList();

            return string.Join("، ", selected);
        }

        return (answer.Value ?? string.Empty).Trim();
    }

    private static void ValidateTypedValue(EventPageFormField field, string raw)
    {
        if (field.FieldType == EventPageFieldTypes.Number
            && !double.TryParse(raw, out _))
        {
            throw new ArgumentException($"يجب أن يكون الحقل رقماً: {field.Label}");
        }

        if (!EventPageFieldTypes.IsSelect(field.FieldType))
            return;

        var options = EventPageJsonParser.ParseOptions(field.OptionsJson);
        var selected = field.FieldType == EventPageFieldTypes.MultiSelect
            ? raw.Split("، ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : [raw];

        foreach (var value in selected)
        {
            if (!options.Contains(value))
                throw new ArgumentException($"قيمة غير صحيحة للحقل: {field.Label}");
        }
    }
}
