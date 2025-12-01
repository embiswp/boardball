public class Szenzorhalozat
{
    public delegate void MeresTriggerDelegate();
    public List<Szenzor> Szenzorok { get; set; }
    public AdatgyujtoAllomas AdatgyujtoAllomas { get; set; }
    public event MeresTriggerDelegate MeresTrigger;
    
    public Szenzorhalozat()
    {
        Szenzorok = new List<Szenzor>();
        AdatgyujtoAllomas = new AdatgyujtoAllomas();
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
        szenzor.MeresiAdatKeszult += AdatgyujtoAllomas.MeresiAdatFogadas;
        MeresTrigger += szenzor.Meres;
    }
}