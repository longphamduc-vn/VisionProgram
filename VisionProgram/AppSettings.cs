public class PlcConnection
{
    public string Ip { get; set; }
    public int Port { get; set; }
}

public class AppSettings
{
    public string ImagePath { get; set; }
    public double Threshold { get; set; }
    public PlcConnection PlcConnection { get; set; }
}
