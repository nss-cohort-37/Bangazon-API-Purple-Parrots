using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BangazonAPI.Controllers
//{
//    public class UserPaymentTypeController : Controller
//    {
//        // GET: /<controller>/
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}


{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPaymentTypesController : ControllerBase

    {
        private readonly IConfiguration _config;

        public UserPaymentTypesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }




        //When making a GET request for a single owner by Id, the Owner JSON representation should include an array of Dogs they own, and their Neighborhood.


        [HttpGet("{id}", Name = "GetUserPaymentType")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                        Id, Name, Active
                        FROM PaymentType
                        WHERE Id = @id";


                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    PaymentType paymentype = null;

                    if (reader.Read())
                    {
                        paymentype = new PaymentType
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active"))


                        };
                    }
                    reader.Close();

                    return Ok(paymentype);
                }
            }
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++




        // Add a payment option for customer      POST
        //    api/userPaymentTypes    - WORKING !!!!

        

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentType paymentype)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO PaymentType (Name, Active)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @active)";
                    cmd.Parameters.Add(new SqlParameter("@name", paymentype.Name));
                    cmd.Parameters.Add(new SqlParameter("@active", paymentype.Active));


                    int newId = (int)cmd.ExecuteScalar();
                    paymentype.Id = newId;
                    return CreatedAtRoute("AddUserPaymentType", new { id = newId }, paymentype);
                }
            }
        }



        //Update customer payment option          PUT
        //    api/userPaymentTypes/{id }     



        //Remove customer payment option        DELETE***
        //***This should be a soft delete.Meaning you should not actually delete the customer
        //    from the database. There is an Active column on the UserPaymentType table that should be set to False.







        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }
}
  