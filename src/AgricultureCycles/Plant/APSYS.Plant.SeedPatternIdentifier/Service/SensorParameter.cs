namespace APSYS.Plant.SeedPatternIdentifier.Service
{
    public class SensorParameter
    {
        public int SensorNumber { get; set; }
        public int SensorMaxValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", SensorNumber, SensorMaxValue);
        }
    }
}