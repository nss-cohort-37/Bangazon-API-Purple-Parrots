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

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomersController(IConfiguration config)
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
                           c.Id, c.FirstName, c.LastName, c.CreatedDate, c.Active, c.Address, c.City, c.State, c.Email, c.Phone
                        FROM 
                            Customer c";
                    //    WHERE 
                    //        Id = @id";
                    //cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Customer customer = null;

                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            City = reader.GetString(reader.GetOrdinal("City")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            Product = new List<Product>()
                        };
                    }
                    reader.Close();

                    return Ok(customer);
                }
            }
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Customer (c.FirstName, c.LastName, c.CreatedDate, c.Active, c.Address, c.City, c.State, c.Email, c.Phone)
                                    OUTPUT INSERTED.Id
                                    VALUES (@firstName, @lastName, @createdDate, @active, @address, @city, @state, @email, @phone)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));
                    cmd.Parameters.Add(new SqlParameter("@createdDate", customer.CreatedDate));
                    cmd.Parameters.Add(new SqlParameter("@active", customer.Active));
                    cmd.Parameters.Add(new SqlParameter("@address", customer.Address));
                    cmd.Parameters.Add(new SqlParameter("@city", customer.City));
                    cmd.Parameters.Add(new SqlParameter("@state", customer.State));
                    cmd.Parameters.Add(new SqlParameter("@email", customer.Email));
                    cmd.Parameters.Add(new SqlParameter("@phone", customer.Phone));
                    //(object) is a cast it's like a built in interface to treat these as an object, allowing you use "??" because customer.Notes and DBNull are different types 
                    //the "??" coaslesce operator specifies to use DBNull.Value as the backup if customer.Notes is empty
                    //cmd.Parameters.Add(new SqlParameter("@notes", (object)customer.Notes ?? DBNull.Value));

                    int newId = (int)cmd.ExecuteScalar();
                    customer.Id = newId;
                    return CreatedAtRoute("GetCustomer", new { id = newId }, customer);
                }
            }
        }
        //Put
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        {
                            cmd.CommandText = @"UPDATE Customer
                                            SET FirstName = @firstName, LastName = @lastName ,  CreatedDate = @createdDate, Active = @active, Address = @address,  City = @city,  State = @state, Email = @email, Phone = @phone
                                            WHERE Id = @id";
                           
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                            cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));
                            cmd.Parameters.Add(new SqlParameter("@createdDate", customer.CreatedDate));
                            cmd.Parameters.Add(new SqlParameter("@active", customer.Active));
                            cmd.Parameters.Add(new SqlParameter("@address", customer.Address));
                            cmd.Parameters.Add(new SqlParameter("@city", customer.City));
                            cmd.Parameters.Add(new SqlParameter("@state", customer.State));
                            cmd.Parameters.Add(new SqlParameter("@email", customer.Email));
                            cmd.Parameters.Add(new SqlParameter("@phone", customer.Phone));

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return new StatusCodeResult(StatusCodes.Status204NoContent);
                            }
                            throw new Exception("No Rows Affected");
                        }
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

        //Delete
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

         private Customer GetAllCustomersProduct()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.FirstName, c.LastName, c.CreatedDate, c.Active, c.[Address], c.City, c.[State], c.Email, c.Phone, p.Id as ProductId, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.[Description]
                                        FROM Customer c
                                        LEFT JOIN Product p ON c.Id = p.ProductTypeId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    Customer customer = null;

                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            City = reader.GetString(reader.GetOrdinal("City")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            Product = new List<Product>()
                        };
                    }
                    customer.Product.Add(new Product()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                        DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                        ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Description = reader.GetString(reader.GetOrdinal("Description"))
                    });
                    reader.Close();

                    return customer;
                }
            }
        }

        private List<Customer> GetAllCustomers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.FirstName, c.LastName, c.CreatedDate, c.Active, c.Address, c.City, c.State, c.Email, c.Phone
                                        FROM Customer c";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Customer> customers = new List<Customer>(); 

                    while(reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            City = reader.GetString(reader.GetOrdinal("City")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        };
                        customers.Add(customer);
                    }
                    
                    reader.Close();

                    return customers;
                }
            }
        }

        //Method to check if Customergo Exists
        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"

                            Select c.FirstName, c.LastName, c.CreatedDate, c.Active, c.Address, c.City, c.State, c.Email, c.Phone
                            FROM Customer c
                            WHERE c.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
