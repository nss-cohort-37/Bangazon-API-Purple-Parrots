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


        // this code gets the user payment types with the customerId  
        //  localhost:5000/api/userpaymenttypes/?customerId=4
        // TESTED and it works

        [HttpGet]

        public async Task<IActionResult> Get(
              [FromQuery] int? customerId)

        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {


                        cmd.CommandText = @"
                        SELECT upt.Id, upt.CustomerId, upt.PaymentTypeId, upt.AcctNumber,upt.Active,  c.FirstName, c.LastName
                        FROM UserPaymentType upt
                        LEFT JOIN Customer c ON upt.CustomerId = c.Id   
                        WHERE 1 = 1";
                        

                    if (customerId != null)
                    {
                        cmd.CommandText += " AND CustomerId = @customerId";
                        cmd.Parameters.Add(new SqlParameter("@customerId", customerId));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<UserPaymentType> userpaymenttypes = new List<UserPaymentType>();

                    while (reader.Read())
                    {

                        int id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string acctnumber = reader.GetString(reader.GetOrdinal("AcctNumber"));
                        bool active = reader.GetBoolean(reader.GetOrdinal("Active"));
                        int customeridid = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                        int paymenttypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"));

                        UserPaymentType userpaymenttype = new UserPaymentType
                        {
                            Id = id,
                            Active = active,
                            AcctNumber = acctnumber,
                            CustomerId = customeridid,
                            PaymentTypeId = paymenttypeId
                        };

                        userpaymenttypes.Add(userpaymenttype);
                    }

                    reader.Close();

                    return Ok(userpaymenttypes);
                }
            }

        }




        // Add a payment option for customer      POST
  
            // WORKS if you do this
        //NOTE - it is acting weird and wont post until you run it wrong then run it correctly?
        //localhost:5000/api/userpaymenttype  wont post
        //localhost:5000/api/userpaymenttypes gives error
        //localhost:5000/api/userpaymenttype  now it works??? 

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserPaymentType userpaymentype)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO UserPaymentType (AcctNumber, Active, CustomerId, PaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@acctnumber, @active, @customerid, @paymenttypeid)";

                    cmd.Parameters.Add(new SqlParameter("@acctnumber", userpaymentype.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@active", userpaymentype.Active));
                    cmd.Parameters.Add(new SqlParameter("@customerid", userpaymentype.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@paymenttypeid", userpaymentype.PaymentTypeId));


                    int newId = (int)cmd.ExecuteScalar();
                    userpaymentype.Id = newId;
                    return CreatedAtRoute("AddUserPaymentType", new { id = newId }, userpaymentype);
                }
            }
        }



        //Update customer payment option          PUT      THIS CODE WORKS FINE!!   NO WEIRD TYPO NEEDED
        //    api/userPaymentTypes/{id }     


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] UserPaymentType userpaymentType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE UserPaymentType
                                            SET AcctNumber = @acctnumber,
                                            Active = @active,
                                            CustomerId = @customerid,
                                            PaymentTypeId = @paymenttypeid
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@acctnumber", userpaymentType.AcctNumber));
                        cmd.Parameters.Add(new SqlParameter("@active", userpaymentType.Active));
                        cmd.Parameters.Add(new SqlParameter("@customerid", userpaymentType.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymenttypeid", userpaymentType.PaymentTypeId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!UserPaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //Remove customer payment option        DELETE***  this WORKS 
        //***This should be a soft delete.Meaning you should not actually delete the customer
        //    from the database. There is an Active column on the UserPaymentType table that should be set to False.



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE UserPaymentType
                                            SET Active = 0
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }

            catch (Exception)
            {
                if (!UserPaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        private bool UserPaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name
                        FROM PaymentType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }



        }
    }
}

  