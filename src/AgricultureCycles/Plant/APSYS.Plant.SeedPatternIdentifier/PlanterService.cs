namespace APSYS.Plant.SeedPatternIdentifier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NLog;

    public class PlanterService
    {
        private static Logger _loggerResults;

        public static Planter Verify(string logData)
        {
            _loggerResults = LogManager.GetLogger("logDataResultsRule");
            _loggerResults.Info("---------------------------------------- Nova Verificação ----------------------------------------\n");

            const int numberOfSensorsPerSeedTube = 3;
            int errorcount = 1;
            int order = 1;
            var seedTubeDataReadings = new List<SeedTubeDataReading>();

            var splitFields = ParseData(logData);

            foreach (var seedTubeDataLine in splitFields)
            {
                char[] separator = { '_' };
                var seedTubeDataReadingText = seedTubeDataLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (seedTubeDataReadingText.Count() == numberOfSensorsPerSeedTube)
                {
                    var seedTubeDatas = new List<SeedTubeData>();
                    foreach (string seedTubeDataText in seedTubeDataReadingText)
                    {
                        var seedTubeData = SeedTubeDataService.ParseSensorData(seedTubeDataText);

                        if (seedTubeData == null)
                        {
                            _loggerResults.Error("Erro {1} no parse de um sensor: {0}", seedTubeDataText, errorcount++);
                            continue;
                        }

                        seedTubeDatas.Add(seedTubeData);
                    }

                    if (seedTubeDatas.Count == numberOfSensorsPerSeedTube)
                    {
                        if (seedTubeDatas.Select(a => a.SensorNumber).Distinct().Count() == numberOfSensorsPerSeedTube)
                        {
                            var seedTubeDataReading = new SeedTubeDataReading()
                            {
                                Order = order++,
                                SeedTubeDataReadings = seedTubeDatas
                            };

                            seedTubeDataReadings.Add(seedTubeDataReading);
                        }
                        else
                        {
                            _loggerResults.Error("Leituras com sensores duplicados", seedTubeDatas.Count);
                        }
                    }
                    else
                    {
                        _loggerResults.Error("Leituras com apenas {0} sensores", seedTubeDatas.Count);
                    }
                }
                else
                {
                    _loggerResults.Error("Erro {1} no parse da linha devido ao número de sensores do tubo de semente {2} ser diferente: {0}", seedTubeDataLine, errorcount++, numberOfSensorsPerSeedTube);
                }
            }

            var seedCounter = SeedTubeDataService.GetSeedCounter(seedTubeDataReadings);

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

        private static List<string> ParseData(string logData)
        {
            char[] separatora = { ';', '\r', '\n', '$', '_' };
            var splitFieldsa = logData.Split(separatora, StringSplitOptions.RemoveEmptyEntries);

            var filterLogData = logData.Replace("Iniciando", string.Empty);
            filterLogData = filterLogData.Replace("\r", string.Empty);
            filterLogData = filterLogData.Replace("\n", string.Empty);
            var count = filterLogData.Replace("?", string.Empty);
            var diff = filterLogData.Length - count.Length;

            var exists = count.Any(a => !char.IsNumber(a) && a != ',' && a != ';' && a != '$' && a != '_');

            if (exists)
            {
                var diffs = count.Where(a => !char.IsNumber(a) && a != ',' && a != ';' && a != '$' && a != '_').ToList();
            }

            char[] separator = { '$', ';' };
            var splitFields = count.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            var difff = splitFieldsa.Count() - splitFields.Count();
            return splitFields.ToList();
        }
    }
}