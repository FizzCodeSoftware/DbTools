using System;

namespace FizzCode.DbTools.DataGenerator;
public class GeneratorDateMinMax : GeneratorMinMax<DateTime>
{
    public GeneratorDateMinMax(int pastNYears, int futureNYears = 0)
        : base(DateTime.Now.AddYears(-pastNYears), DateTime.Now.AddYears(futureNYears))
    {
    }

    public GeneratorDateMinMax(DateTime min, DateTime max)
        : base(min, max)
    {
    }

    public override object Get()
    {
        var timeSpan = Max - Min;
        var newSpanDays = new TimeSpan(Context.Random.Next(0, (int)timeSpan.TotalDays), 0, 0, 0);
        var dateTime = Min + newSpanDays;

        return dateTime;
    }
}
