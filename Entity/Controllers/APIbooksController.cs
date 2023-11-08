using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;

namespace Entity.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class APIbooksController : ControllerBase
    {
        // getting all book search catagory
        [HttpGet("{cat}")]
        public IEnumerable<mybook> Get(int cat)
        {
            List<mybook> li = new List<mybook>();
            //  SqlConnection conn1 = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mynewdb;Integrated Security=True;Pooling=False");
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("EntityContext");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT * FROM book where category ='" + cat + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                li.Add(new mybook
                {
                    title = (string)reader["title"],
                });

            }

            reader.Close();
            conn1.Close();
            return li;
        }
    
    
    
    
    }
}