namespace VacacionesApi.Services;

public sealed class BusinessDaysCalculator : IBusinessDaysCalculator
{
    public int Calculate(DateOnly start, DateOnly end, IEnumerable<DateOnly>? holidays = null)
    {
        if (end < start)
        {
            throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");
        }

        var holidaySet = holidays is null ? new HashSet<DateOnly>() : new HashSet<DateOnly>(holidays);
        var businessDays = 0;

        for (var day = start; day <= end; day = day.AddDays(1))
        {
            var isSunday = day.DayOfWeek == DayOfWeek.Sunday;
            if (!isSunday && !holidaySet.Contains(day))
            {
                businessDays++;
            }
        }

        return businessDays;
    }
}
