namespace APSYS.Plant.SeedPatternIdentifier.Domain
{
    using System.Collections.Generic;
    using Infrastructure.Common;

    public class SeedTubeDataReading
    {
        public List<SeedTubeData> SeedTubeDataReadings { get; set; }

        // public DateTime ReadingTime { get; set; }
        public int Order { get; set; }

        public override string ToString()
        {
            string text = string.Format("{0} --- ", Order);

            if (SeedTubeDataReadings != null)
            {
                text += SeedTubeDataReadings.ToString(a => a.ToString());
            }

            return text;
        }
    }
}