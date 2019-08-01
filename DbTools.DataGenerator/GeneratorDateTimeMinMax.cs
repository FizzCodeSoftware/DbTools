namespace FizzCode.DbTools.DataGenerator
{
    using System;

    public class GeneratorDateTimeMinMax : GeneratorDateMinMax
    {
        public GeneratorDateTimeMinMax(int pastNYears, int futureNYears = 0) : base(DateTime.Now.AddYears(-pastNYears), DateTime.Now.AddYears(futureNYears))
        { }

        public GeneratorDateTimeMinMax(DateTime min, DateTime max) : base(min, max)
        { }

        public override object Get()
        {
            var dateTime = (DateTime)base.Get();

            var hours = Context.Random.Next(0, 24);
            var minutes = Context.Random.Next(0, 60);
            var seconds = Context.Random.Next(0, 60);
            var milliSeconds = Context.Random.Next(0, 1000);

            dateTime = dateTime.AddHours(hours);
            dateTime = dateTime.AddMinutes(minutes);
            dateTime = dateTime.AddSeconds(seconds);
            dateTime = dateTime.AddMilliseconds(milliSeconds);

            return dateTime;
        }
    }
}
