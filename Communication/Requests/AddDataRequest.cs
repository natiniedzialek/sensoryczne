namespace HttpServer.Communication.Requests;

public class AddDataRequest
{
    public string Spo2 { get; set; }
    public string PulseRate { get; set; }
    public string Temperature { get; set; }

    public override string ToString()
    {
        return $"AddDataRequest: " +
               $"Spo2 = {Spo2}, " +
               $"PulseRate = {PulseRate}, " +
               $"Temperature = {Temperature}";
    }
}