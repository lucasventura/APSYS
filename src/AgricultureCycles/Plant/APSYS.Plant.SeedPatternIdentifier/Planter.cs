namespace APSYS.Plant.SeedPatternIdentifier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Planter
    {
        public List<SeedTube> SeedTubes { get; set; }

        public static Planter Verify(string logData)
        {
            var seedTubeDatas = new List<SeedTubeData>();

            char[] separator = { '\n' };
            var splitFields = logData.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var seedTubeDataLine in splitFields)
            {
                var seedTubeData = SeedTubeData.ParseSensorData(seedTubeDataLine);

                if (seedTubeData == null)
                {
                    // todo: logar erro no parse
                    continue;
                }

                seedTubeDatas.Add(seedTubeData);
            }

            var planter = new Planter();
            var seedTubes = new List<SeedTube>();
            var seedTube = new SeedTube()
            {
                SeedTubeDatas = seedTubeDatas,
                SeedTubeNumber = 1
            };

            seedTubes.Add(seedTube);
            planter.SeedTubes = seedTubes;

            return planter;
        }
    }
}