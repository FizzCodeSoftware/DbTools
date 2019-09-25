namespace FizzCode.DbTools.DataGenerator
{
    public class GeneratorName : GeneratorTypizedText
    {
        public override object Get()
        {
            var personNames = new PersonNames();
            var familyNameindex = Context.Random.Next(1, personNames.FamilyNames.Length);
            var surNameIndex = Context.Random.Next(1, personNames.SurNames.Length);

            var middleName = " ";
            if (Context.Random.Next(0, 1) > 0)
            {
                var middleNameIndex = Context.Random.Next(1, personNames.SurNames.Length);
                middleName = $" {personNames.SurNames[middleNameIndex]} ";
            }

            var name = $"{personNames.SurNames[surNameIndex]}{middleName}{personNames.FamilyNames[familyNameindex]}";

            return name;
        }
    }
}
