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
        for (int i=0; i<10; i++)
        {
            System.Console.WriteLine("Meres inditasa...");
            MeresTrigger?.Invoke();
            Thread.Sleep(1000);
        }
    }

}