namespace DapperStudy.Models;

public class Career
{
    public Career()
    {
        Items = new List<CareerItem>(); //iniciei a lista, para nao ter obj nulo
    }

    public Guid Id { get; set; }
    public required string Title { get; set; }
    public IList<CareerItem> Items { get; set; }
}