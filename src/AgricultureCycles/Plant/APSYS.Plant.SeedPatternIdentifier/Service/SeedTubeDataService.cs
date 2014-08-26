namespace APSYS.Plant.SeedPatternIdentifier.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using NLog;

    public class SeedTubeDataService
    {
        // todo: corrigir logs com IoC
        private Logger _loggerResults;

        public List<SensorParameter> SensorParameters { get; set; }

        public SeedTubeData ParseSensorData(string sensorData)
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

        public SeedCounter GetSeedCounter(List<SeedTubeDataReading> seedTubeDataReadings)
        {
            var seedCounter = new SeedCounter();

            foreach (var seedTubeDataReading in seedTubeDataReadings)
            {
                foreach (var tubeDataReading in seedTubeDataReading.SeedTubeDataReadings)
                {
                    var maxValueSensor = GetMaxValueSensorBySensorNumber(tubeDataReading.SensorNumber);

                    if (maxValueSensor == null)
                    {
                        throw new Exception(string.Format("Erro ao obter contador de semente. Sensor {0} não existe.", tubeDataReading.SensorNumber));
                    }

                    if (tubeDataReading.SensorValue > maxValueSensor.SensorMaxValue)
                    {
                        tubeDataReading.HasSeed = true;
                    }
                }
            }

            var count = seedTubeDataReadings.Where(a => a.SeedTubeDataReadings.Any(b => b.HasSeed)).ToList();
            seedCounter.Count = count.Count;
            return seedCounter;
        }

        public List<SeedTubeDataReading> MountSeedTubeDataReadings(IEnumerable<string> seedTubeDataLineText)
        {
            _loggerResults = LogManager.GetLogger("logDataResultsRule");
            _loggerResults.Info("---------------------------------------- Nova Verificação ----------------------------------------\n");

            var seedTubeDataReadings = new List<SeedTubeDataReading>();

            const int numberOfSensorsPerSeedTube = 3;
            int errorcount = 1;
            int order = 1;

            foreach (var seedTubeDataLine in seedTubeDataLineText)
            {
                char[] separator = { '_' };
                var seedTubeDataReadingText = seedTubeDataLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (seedTubeDataReadingText.Count() == numberOfSensorsPerSeedTube)
                {
                    var seedTubeDatas = new List<SeedTubeData>();
                    foreach (string seedTubeDataText in seedTubeDataReadingText)
                    {
                        var seedTubeData = ParseSensorData(seedTubeDataText);

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

            return seedTubeDataReadings;
        }

        public IEnumerable<string> SplitPureTextToSeedTube(string logData)
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
            return splitFields;
        }

        public void AddSensorParameter(SensorParameter sp)
        {
            if (SensorParameters == null)
            {
                SensorParameters = new List<SensorParameter>();
            }

            var sensorParameter = SensorParameters.FirstOrDefault(a => a.SensorNumber == sp.SensorNumber);

            if (sensorParameter != null)
            {
                sensorParameter.SensorMaxValue = sp.SensorMaxValue;
            }
            else
            {
                SensorParameters.Add(sp);
            }
        }

        private SensorParameter GetMaxValueSensorBySensorNumber(int sensorNumber)
        {
            if (SensorParameters == null)
            {
                SensorParameters = new List<SensorParameter>();

                var sensorParameter1 = new SensorParameter()
                {
                    SensorNumber = 1,
                    SensorMaxValue = 600
                };

                var sensorParameter2 = new SensorParameter()
                {
                    SensorNumber = 2,
                    SensorMaxValue = 600
                };

                var sensorParameter3 = new SensorParameter()
                {
                    SensorNumber = 3,
                    SensorMaxValue = 600
                };

                SensorParameters.Add(sensorParameter1);
                SensorParameters.Add(sensorParameter2);
                SensorParameters.Add(sensorParameter3);
            }

            if (!SensorParameters.Any(a => a.SensorNumber == sensorNumber))
            {
                return null;
            }

            return SensorParameters.First(a => a.SensorNumber == sensorNumber);
        }
    }
}