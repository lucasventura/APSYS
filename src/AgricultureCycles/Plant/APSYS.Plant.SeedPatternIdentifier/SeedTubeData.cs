namespace APSYS.Plant.SeedPatternIdentifier
{
    public class SeedTubeData
    {
        public int SensorNumber { get; set; }
        public int SensorValue { get; set; }
        public bool HasSeed { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", SensorNumber, SensorValue);
        }
    }
}