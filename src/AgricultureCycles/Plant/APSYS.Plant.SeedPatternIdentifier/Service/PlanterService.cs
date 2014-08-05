namespace APSYS.Plant.SeedPatternIdentifier.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using NLog;

    public class PlanterService
    {
        private readonly SeedTubeDataService _seedTubeDataService;
        private Logger _loggerResults;

        public PlanterService(SeedTubeDataService seedTubeDataService)
        {
            _seedTubeDataService = seedTubeDataService;
        }

        public Planter Verify(string logData)
        {
            _loggerResults = LogManager.GetLogger("logDataResultsRule");
            _loggerResults.Info("---------------------------------------- Nova Verificação ----------------------------------------\n");

            int errorcount = 1;

            var seedTubeDataLineText = _seedTubeDataService.SplitPureTextToSeedTube(logData);

            var seedTubeDataReadings = _seedTubeDataService.MountSeedTubeDataReadings(seedTubeDataLineText);

            var seedCounter = _seedTubeDataService.GetSeedCounter(seedTubeDataReadings);

            var todosSensores = seedTubeDataReadings.SelectMany(a => a.SeedTubeDataReadings).GroupBy(a => a.SensorNumber);

            foreach (IGrouping<int, SeedTubeData> sensor in todosSensores)
            {
                _loggerResults.Info("Sensor: " + sensor.Key + " - " + sensor.Count());

                var todosValoresSensor = sensor.GroupBy(a => a.SensorValue);

                foreach (var valorSensor in todosValoresSensor)
                {
                    _loggerResults.Info("Sensor: " + sensor.Key + " - Valor: " + valorSensor.Key + " - " + valorSensor.Count());
                }
            }

            var planter = new Planter();
            var seedTubes = new List<SeedTube>();
            var seedTube = new SeedTube()
            {
                SeedTubeDataReadings = seedTubeDataReadings,
                SeedTubeNumber = 1
            };

            seedTubes.Add(seedTube);
            planter.SeedTubes = seedTubes;

            _loggerResults.Info("Total de Erros: {0}\n", errorcount);

            return planter;
        }
    }
}