namespace Telligent.Tag.Application.Configs;

public class Config
{
    public Apis Apis { get; set; }
}

public struct Apis
{
    public string MemberApi { get; set; }

    public string ElectronicCommerceApi { get; set; }
}