namespace APSYS.Plant.SeedPatternIdentifier.Domain
{
    using System.Collections.Generic;
    using Infrastructure.Common;

    public class SeedTube
    {
        public int SeedTubeNumber { get; set; }
        public List<SeedTubeDataReading> SeedTubeDataReadings { get; set; }

        public override string ToString()
        {
            string text = string.Format("{0} --- ", SeedTubeNumber);

            if (SeedTubeDataReadings != null)
            {
                text += SeedTubeDataReadings.ToString(a => a.ToString());
            }

            return text;
        }
    }
}