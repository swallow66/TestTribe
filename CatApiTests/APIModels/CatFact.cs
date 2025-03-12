namespace CatApiTests
{
    public class CatFact
    {
        public string Fact { get; set; }
        public int Length { get; set; }

        public CatFact(string fact, int length)
        {
            Fact = fact;
            Length = length;
        }
    }
}