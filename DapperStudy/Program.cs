using System.Data;
using Dapper;
using DapperStudy.Models;
using Microsoft.Data.SqlClient;

namespace DapperStudy;

class Program
{
    static void Main(string[] args)
    {
        const string connectionString =
            "Server=localhost;Database=study_db;User Id=sa;Password=C3rul3@nC@v3_150;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=true;";

        using (var connection = new SqlConnection(connectionString))
        {
            //UpdateCategory(connection);
            //CreateCategory(connection);
            //DeleteCategory(connection);
            //CreateManyCategory(connection);
            //ListCategories(connection);
            //ExecuteProcedure(connection);
            //ExecuteReadProcedure(connection);
            //ExecuteScalar(connection);
            //ReadView(connection);
        }
    }

    //methods
    static void ListCategories(SqlConnection connection)
    {
        const string sqlSelect = " select [id], [title] from [category] ";
        var categories = connection.Query<Category>(sqlSelect);

        foreach (var item in categories)
        {
            Console.WriteLine($"{item.Id} - {item.Title}");
        }
    }
    
    static void CreateCategory(SqlConnection connection)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Title = "Amazon AWS",
            Url = "https://aws.amazon.com",
            Summary = "AWS Cloud",
            Order = 8,
            Description = "Categoria de cursos de aws",
            Featured = true
        };

        const string sqlInsert = " insert into [category] " +
                                 " values(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured) ";

        var rows = connection.Execute(sqlInsert, new
        {
            Id = category.Id,
            Title = category.Title,
            Url = category.Url,
            Summary = category.Summary,
            Order = category.Order,
            Description = category.Description,
            Featured = category.Featured
        });
        Console.WriteLine($"Inserted {rows} rows");
    }
    
    static void UpdateCategory(SqlConnection connection)
    {
        const string sqlUpdate = " update [category] set [title] = @Title where [id] = @Id";
        var rows = connection.Execute(sqlUpdate, new
        {
            Id = new Guid("100708e9-6b66-4f2d-8c24-af447dbcf1b2"),
            Title = "Frontend"
        });
        Console.WriteLine($"Updated {rows} rows");
    }
    
    static void CreateManyCategory(SqlConnection connection)
    {
        var categoryOne = new Category
        {
            Id = Guid.NewGuid(),
            Title = "Amazon AWS",
            Url = "https://aws.amazon.com",
            Summary = "AWS Cloud",
            Order = 8,
            Description = "Categoria de cursos de aws",
            Featured = true
        };
        
        var categoryTwo = new Category
        {
            Id = Guid.NewGuid(),
            Title = "Nova Categoria",
            Url = "https://new.category",
            Summary = "Nova Categoria",
            Order = 9,
            Description = "Categoria de cursos",
            Featured = false
        };

        const string sqlInsert = " insert into [category] " +
                                 " values(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured) ";

        var rows = connection.Execute(sqlInsert, new[]{
            new
            {
                Id = categoryOne.Id,
                Title = categoryOne.Title,
                Url = categoryOne.Url,
                Summary = categoryOne.Summary,
                Order = categoryOne.Order,
                Description = categoryOne.Description,
                Featured = categoryOne.Featured
            },
            new
            {
                Id = categoryTwo.Id,
                Title = categoryTwo.Title,
                Url = categoryTwo.Url,
                Summary = categoryTwo.Summary,
                Order = categoryTwo.Order,
                Description = categoryTwo.Description,
                Featured = categoryTwo.Featured
            }
        });
        Console.WriteLine($"Inserted {rows} rows");
    }
    
    static void DeleteCategory(SqlConnection connection)
    {
        var sqlDelete = " delete from [category] where [id] = @Id ";

        var rows = connection.Execute(sqlDelete, new
        {
            id = new Guid("")
        });
        Console.WriteLine($"Deleted {rows} rows");
    }

    static void ExecuteProcedure(SqlConnection connection)
    {
      const string procedure = "[sp_delete_student]";
      var pars = new { student_id = "DACDCC02-4192-468A-813B-BE0F229987CE" };
      
      var affectedRows = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);
      Console.WriteLine($"Deleted {affectedRows} rows");
    }

    static void ExecuteReadProcedure(SqlConnection connection)
    {
      const string procedure = "[sp_get_courses_by_category]";
      var pars = new { category_id = "A96CD50F-F5F1-4CE5-BB8B-928C6E8FA47A" };
      var courses = connection.Query<Category>(procedure, pars, commandType: CommandType.StoredProcedure);

      foreach (var course in courses)
      {
        Console.WriteLine($"{course.Id} - {course.Title}");
      }
    }

    static void ExecuteScalar(SqlConnection connection)
    {
      var category = new Category
      {
        Title = "Amazon AWS",
        Url = "https://aws.amazon.com",
        Summary = "AWS Cloud",
        Order = 8,
        Description = "Categoria de cursos de aws",
        Featured = true
      };

      const string sqlInsert = " insert into [category] " +
                               " output inserted.[id] " + //para guids
                               " values(newid(), @Title, @Url, @Summary, @Order, @Description, @Featured) ";
                               //" select scope_identity() "; para inteiros

      var id = connection.ExecuteScalar<Guid>(sqlInsert, new
      {
        Title = category.Title,
        Url = category.Url,
        Summary = category.Summary,
        Order = category.Order,
        Description = category.Description,
        Featured = category.Featured
      });
      Console.WriteLine($"Inserted {id}");
    }

    static void ReadView(SqlConnection connection)
    {
      var sql = " select * from [vw_courses] ";
      
      var courses = connection.Query(sql);
      foreach (var course in courses)
      {
        Console.WriteLine($"{course.id} - {course.title}");
      }
    }

    static void OneToOne(SqlConnection connection)
    {
        var sql = " select * from [career_item] inner join [course]" +
                  "on [career_item].[course_id] = [course].[id] ";
        
        //um objeto(model) dentro do outro(segregar)
        //tenho dois primeiros parametros e a juncao deles esta dentro do terceiro parametro
        var items = connection.Query<CareerItem, Course, CareerItem>(
            sql,
            (careerItem, course) => //vai segregrar os itens em dois
            {
                careerItem.Course = course;
                return careerItem;
            }, splitOn: "[id]"); //divididos pelo id
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Title} - {item.Course.Title}"); //os dois objetos estao populados
        }
    }

    static void OneToMany(SqlConnection connection)
    {
        var sql = " select * from [career] inner join [career_item]" +
                  "on [career_item].[career_id] = [career].[id] order by [career].[title] ";

        var careers = new List<Career>();
        //um objeto(model) dentro do outro(segregar)
        //recebemos o primeiro parametro(pai), populado pelo segundo(filho) e a juncao deles esta dentro do terceiro(result no pai)
        var items = connection.Query<Career, CareerItem, Career>(
            sql,
            (career, careerItem) => //vai segregrar os itens em dois
            {
                var car = careers.FirstOrDefault(x => x.Id == career.Id);
                if (car == null)
                {
                    car = career;
                    car.Items.Add(careerItem);
                    careers.Add(car);
                }
                else 
                {
                    car.Items.Add(careerItem);
                }
                return career;
            }, splitOn: "[career_id]"); //divididos pelo id
        foreach (var career in items)
        {
            Console.WriteLine($"{career.Title}");
            foreach (var item in career.Items)
            {
                Console.WriteLine($" - {item.Title}");
            }
        }
    }
}
