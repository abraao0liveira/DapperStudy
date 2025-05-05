namespace DapperStudy.Models;

public class CareerItem
{
    public CareerItem(Course course)
    {
        Course = course;
    }

    public Guid Id { get; set; }
    public required string Title { get; set; }
    public Course Course { get; set; }
}