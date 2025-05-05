namespace DapperStudy.Models;

public class Category
{ 
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Url { get; set; }
    public required string Summary { get; set; }
    public int Order { get; set; }
    public required string Description { get; set; }
    public bool Featured { get; set; }
}