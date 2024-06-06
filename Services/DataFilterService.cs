using HttpServer.Data.Models;

namespace HttpServer.Services
{
    public class DataFilterService : IDataFilterService
    {
        private readonly IEncryptionService _encryptionService;

        public DataFilterService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public string GenerateMessage(List<Measurement> measurements, string Key, string IV)
        {
            measurements = measurements.Where(m => m.MeasureTime > DateTime.Now.AddMinutes(-1)).ToList();
            var spo2Values = measurements
                .Select(m => DecryptAndConvertToDouble(m.Spo2, Key, IV))
                .Where(v => v is > 0)
                .Select(v => v.Value)
                .ToList();

            var pulseRateValues = measurements
                .Select(m => DecryptAndConvertToDouble(m.PulseRate, Key, IV))
                .Where(v => v is > 0)
                .Select(v => v.Value)
                .ToList();

            var temperatureValues = measurements
                .Select(m => DecryptAndConvertToDouble(m.Temperature, Key, IV))
                .Where(v => v is > 0)
                .Select(v => v.Value)
                .ToList();
            
            var meanTemperature = temperatureValues.Count > 0 ? temperatureValues.Average() : 0;
            var meanSpo2 = spo2Values.Count > 0 ? spo2Values.Average() : 0;
            var meanPulseRate = pulseRateValues.Count > 0 ? pulseRateValues.Average() : 0;

            var message = "";
            Console.WriteLine($"Mean temperature: {meanTemperature}, Mean spo2: {meanSpo2}, Mean pulse rate: {meanPulseRate}");
            if (meanTemperature > 37)
            {
                message += "Temperature is too high. ";
            }
            if (meanTemperature < 30)
            {
                message += "Temperature is too low. ";
            }
            if (meanSpo2 < 90)
            {
                message += "Spo2 is too low. ";
            }
            if (meanPulseRate < 60 || meanPulseRate > 130)
            {
                message += "Pulse rate is out of normal range. ";
            }
            Console.WriteLine(message);

            return message;
        }

        public bool IsMeasurementCorrect(List<Measurement> measurements, Measurement newMeasurement, string Key, string IV)
        {
            var spo2Values = measurements
                .Select(m => DecryptAndConvertToDouble(m.Spo2, Key, IV))
                .Where(v => v.HasValue)
                .Select(v => v.Value)
                .ToList();

            var pulseRateValues = measurements
                .Select(m => DecryptAndConvertToDouble(m.PulseRate, Key, IV))
                .Where(v => v.HasValue)
                .Select(v => v.Value)
                .ToList();

            var temperatureValues = measurements
                .Select(m => DecryptAndConvertToDouble(m.Temperature, Key, IV))
                .Where(v => v.HasValue)
                .Select(v => v.Value)
                .ToList();

            var spo2Range = ComputeIQR(spo2Values);
            var pulseRateRange = ComputeIQR(pulseRateValues);
            var temperatureRange = ComputeIQR(temperatureValues);

            var newSpo2 = DecryptAndConvertToDouble(newMeasurement.Spo2, Key, IV);
            var newPulseRate = DecryptAndConvertToDouble(newMeasurement.PulseRate, Key, IV);
            var newTemperature = DecryptAndConvertToDouble(newMeasurement.Temperature, Key, IV);

            bool isSpo2Valid = newSpo2.HasValue && spo2Range.Item1 <= newSpo2 && newSpo2 <= spo2Range.Item2;
            bool isPulseRateValid = newPulseRate.HasValue && pulseRateRange.Item1 <= newPulseRate && newPulseRate <= pulseRateRange.Item2;
            bool isTemperatureValid = newTemperature.HasValue && temperatureRange.Item1 <= newTemperature && newTemperature <= temperatureRange.Item2;

            return isSpo2Valid && isPulseRateValid && isTemperatureValid;
        }

        private double? DecryptAndConvertToDouble(string encryptedValue, string Key, string IV)
        {
            try
            {
                var decryptedValue = _encryptionService.Decrypt(encryptedValue, Key, IV);
                decryptedValue = decryptedValue.Replace('.', ',');
                if (double.TryParse(decryptedValue, out var numericValue))
                {
                    return numericValue;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private (double, double) ComputeIQR(List<double> values)
        {
            if (values.Count == 0)
            {
                return (double.MinValue, double.MaxValue);
            }

            values.Sort();
            double q1 = values[(int)(values.Count * 0.25)];
            double q3 = values[(int)(values.Count * 0.75)];
            double iqr = q3 - q1;
            double lowerBound = q1 - 1.5 * iqr;
            double upperBound = q3 + 1.5 * iqr;
            return (lowerBound, upperBound);
        }
    }
}
