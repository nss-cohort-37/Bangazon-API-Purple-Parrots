using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using BangazonAPI.Models;
namespace CustomerWalkerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerController(IConfiguration config)
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
        ////Get All
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"Select c.Id, c.FirstName, c.LastName, c.CreatedDate, c.Active, c.Address, c.City, c.State, c.Email, c.Phone
        //                FROM Customer c";
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<Customer> customers = new List<Customer>();
        //            while (reader.Read())
        //            {
        //                Customer customer = new Customer
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
        //                    Active = reader.GetBoolean(reader.GetOrdinal("Active")),
        //                    Address = reader.GetString(reader.GetOrdinal("Address")),
        //                    City = reader.GetString(reader.GetOrdinal("City")),
        //                    State = reader.GetString(reader.GetOrdinal("State")),
        //                    Email = reader.GetString(reader.GetOrdinal("Email")),
        //                    Phone = reader.GetString(reader.GetOrdinal("Phone"))
        //                };
        //                //if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
        //                //{
        //                //    customer.Notes = reader.GetString(reader.GetOrdinal("Notes"));
        //                //}
        //                customers.Add(customer);
        //            }
        //            reader.Close();
        //            return Ok(customers);
        //        }
        //    }
        //}


        //=============================================================================

        // I want this url to be available
        // api/customers?name=Jane
        
        [HttpGet]

        public async Task<IActionResult> Get(
        [FromQuery] string firstName,
        [FromQuery] string lastName)
          

        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = @"
                        SELECT
                        Id, FirstName, LastName, CreatedDate, Active, Address, City, State, Email, Phone
                        FROM Customer 
                        WHERE 1 = 1";



                    if (firstName != null)
                    {
                        cmd.CommandText += " AND FirstName LIKE @firstName";
                        cmd.Parameters.Add(new SqlParameter("@firstName", "%" + firstName + "%"));
                    }


                    if (lastName != null)
                    {
                        cmd.CommandText += " AND LastName LIKE @lastName";
                        cmd.Parameters.Add(new SqlParameter("@lastName", "%" + lastName + "%"));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Customer> customers = new List<Customer>();
                

                    while (reader.Read())
                    {

                        int idid = reader.GetInt32(reader.GetOrdinal("Id"));
                        string fnameValue = reader.GetString(reader.GetOrdinal("FirstName"));
                        string lnameValue = reader.GetString(reader.GetOrdinal("LastName"));
                        DateTime CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                        bool Active = reader.GetBoolean(reader.GetOrdinal("Active"));
                        string Address = reader.GetString(reader.GetOrdinal("Address"));
                        string City = reader.GetString(reader.GetOrdinal("City"));
                        string State = reader.GetString(reader.GetOrdinal("State"));
                        string Email = reader.GetString(reader.GetOrdinal("Email"));
                        string Phone = reader.GetString(reader.GetOrdinal("Phone"));



                        Customer customer = new Customer
                        {
                            Id = idid,
                            FirstName = fnameValue,
                            LastName = lnameValue,
                            CreatedDate = CreatedDate,
                            Active = Active,
                            Address = Address,
                            City = City,
                            State = State,
                            Email = Email,
                            Phone = Phone


                        };

                        customers.Add(customer);
                    }

                    reader.Close();

                    return Ok(customers);
                }
            }

        }






       

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
                        cmd.CommandText = @"UPDATE Customer
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
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName
                        FROM Customer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }


       
    }
}