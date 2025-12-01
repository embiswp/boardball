public class AdatgyujtoAllomas
{
    public List<MeresiAdat> Adatok { get; set; }

    public AdatgyujtoAllomas()
    {
        Adatok = new List<MeresiAdat>();
    }

    public void MeresiAdatFogadas(MeresiAdat adat)
    {
        Adatok.Add(adat);
        System.Console.WriteLine($"Szenzor ID: {adat.SzenzorId}, Meres ideje: {adat.MeresIdeje}, Homerseklet: {adat.Homerseklet}");
    }

    public void ElsoKiertekeles()
    {
        System.Console.WriteLine("Elso kiertekeles kezdete...");
        var atlagHomerseklet = Adatok.Average(a => a.Homerseklet);
        System.Console.WriteLine($"Atlagos homerseklet: {atlagHomerseklet}");
    }
}