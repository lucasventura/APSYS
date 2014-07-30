namespace APSYS.Plant.SeedPatternIdentifier
{
    using System;

    public class SeedTubeData
    {
        public int SensorNumber { get; set; }
        public int SensorValue { get; set; }

        public static SeedTubeData ParseSensorData(string sensorData)
        {
            try
            {
                char[] separator = { ',' };
                var splitFields = sensorData.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                var seedTubeData = new SeedTubeData();
                seedTubeData.SensorNumber = int.Parse(splitFields[0]);
                seedTubeData.SensorValue = int.Parse(splitFields[1]);

                return seedTubeData;
            }
            catch (Exception e)
            {
                // todo: logar
            }

            return null;
        }
    }
}