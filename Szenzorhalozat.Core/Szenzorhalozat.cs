public class Szenzorhalozat
{
    public delegate void MeresTriggerDelegate();
    public List<Szenzor> Szenzorok { get; set; }
    public event MeresTriggerDelegate MeresTrigger;
    
    public Szenzorhalozat()
    {
        Szenzorok = new List<Szenzor>();
    }
    
    public void MeresInditas()
    {
        for (int i=0; i<5; i++)
        {
            System.Console.WriteLine("Meres inditasa...");
            MeresTrigger?.Invoke();
            Thread.Sleep(1000);
        }
    }

    public void SzenzorHozzaadas(Szenzor szenzor)
    {
        Szenzorok.Add(szenzor);
        szenzor.MeresiAdatKeszult += MeresiAdatFogadas;
        MeresTrigger += szenzor.Meres;
    }

    public void MeresiAdatFogadas(MeresiAdat adat)
    {
        System.Console.WriteLine($"Szenzor ID: {adat.SzenzorId}, Meres ideje: {adat.MeresIdeje}, Homerseklet: {adat.Homerseklet}");
    }

}