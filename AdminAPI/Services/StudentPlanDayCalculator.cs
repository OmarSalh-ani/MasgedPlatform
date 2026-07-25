namespace AdminAPI.Services;

public static class StudentPlanDayCalculator
{
    public static int CalcCircleDaysInList(DateTime start, DateTime end, IReadOnlyList<int> circleDayNumbers)
    {
        if (circleDayNumbers.Count == 0)
            return 0;

        var count = 0;
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
        {
            if (circleDayNumbers.Contains((int)d.DayOfWeek))
                count++;
        }

        return count;
    }

    public static (int TotalDays, int ElapsedDays, int RemainingDays) Calculate(
        DateTime planFromDate,
        DateTime planToDate,
        IReadOnlyList<int> circleDayNumbers,
        DateTime today)
    {
        var total = CalcCircleDaysInList(planFromDate.Date, planToDate.Date, circleDayNumbers);
        if (total <= 0 && circleDayNumbers.Count == 0)
            total = (planToDate.Date - planFromDate.Date).Days + 1;

        var remainingStart = today > planFromDate.Date ? today : planFromDate.Date;
        var remaining = CalcCircleDaysInList(remainingStart, planToDate.Date, circleDayNumbers);
        if (today > planToDate.Date)
            remaining = 0;
        if (remaining <= 0 && circleDayNumbers.Count == 0)
            remaining = Math.Max(0, (planToDate.Date - today).Days + 1);

        var elapsed = total - remaining;
        if (elapsed < 0)
            elapsed = 0;

        return (total, elapsed, remaining);
    }
}
