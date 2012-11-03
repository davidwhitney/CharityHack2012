namespace CharityHack2012.Models
{
    public class CharityProfile
    {
        public string CharityName { get; set; }
        public string CharityRegistrationNumber { get; set; }

        public Income Income { get; set; }

        public CharityProfile()
        {
            Income = new Income();
        }
    }

    public class Income
    {
        public string Total { get; set; }
        public string Voluntary { get; set; }
        public string TradingToRaiseFunds { get; set; }
        public string Investment { get; set; }
        public string CharitableActivities { get; set; }
        public string Other { get; set; }
    }
}