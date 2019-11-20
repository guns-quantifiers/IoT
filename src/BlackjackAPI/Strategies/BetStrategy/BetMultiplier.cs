namespace BlackjackAPI.Strategies.BetStrategy
{
    public class BetMultiplier
    {
        public double Value { get; set; }

        public override string ToString()
        {
            return Value.ToString("F2");
        }
    }
}
