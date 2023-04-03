namespace Data.Models;

[Serializable]
public class Hash
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Sha1 { get; set; } = string.Empty;
}
