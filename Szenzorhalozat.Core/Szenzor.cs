public class Szenzor
{
    public delegate void MeresiAdatDelegate(MeresiAdat adat);
    public event MeresiAdatDelegate MeresiAdatKeszult;
    public int Id { get; set; }
    
    public void Meres()
    {
        System.Console.WriteLine($"Szenzor {Id} meres vegrehajtva.");
        var meresiAdat = new MeresiAdat
        {
            SzenzorId = Id,
            MeresIdeje = DateTime.Now,
            Homerseklet = new Random().Next(-20, 40)
        };
        MeresiAdatKeszult?.Invoke(meresiAdat);
    }
}