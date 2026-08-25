namespace AdminAPI.Models;

public static class EventPageFieldTypes
{
    public const string Text = "Text";
    public const string Number = "Number";
    public const string SingleSelect = "SingleSelect";
    public const string MultiSelect = "MultiSelect";

    public static readonly string[] All = [Text, Number, SingleSelect, MultiSelect];

    public static bool IsSelect(string fieldType) =>
        fieldType is SingleSelect or MultiSelect;
}
