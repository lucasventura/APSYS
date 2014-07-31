namespace APSYS.Plant.SeedPatternIdentifier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SeedTubeDataService
    {
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

        public static SeedCounter GetSeedCounter(List<SeedTubeDataReading> seedTubeDataReadings)
        {
            var seedCounter = new SeedCounter();

            foreach (var seedTubeDataReading in seedTubeDataReadings)
            {
                foreach (var tubeDataReading in seedTubeDataReading.SeedTubeDataReadings)
                {
                    int maxValueSensor = GetMaxValueSensorBySensorNumber(tubeDataReading.SensorNumber);

                    if (tubeDataReading.SensorValue > maxValueSensor)
                    {
                        tubeDataReading.HasSeed = true;
                    }
                }
            }

            var count = seedTubeDataReadings.Where(a => a.SeedTubeDataReadings.Any(b => b.HasSeed)).ToList();
            seedCounter.Count = count.Count;
            return seedCounter;
        }

        private static int GetMaxValueSensorBySensorNumber(int sensorNumber)
        {
            // todo: Fazer calibracao setando variaveis e numeros de sensores
            int maxValueSensor1 = 150;
            int maxValueSensor2 = 68;
            int maxValueSensor3 = 85;

            if (sensorNumber == 1)
            {
                return maxValueSensor1;
            }

            if (sensorNumber == 2)
            {
                return maxValueSensor2;
            }

            if (sensorNumber == 3)
            {
                return maxValueSensor3;
            }

            string message = string.Format("Sensor {0} não existente.", sensorNumber);
            throw new Exception(message);
        }
    }
}