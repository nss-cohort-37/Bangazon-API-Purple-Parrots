//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using Microsoft.Data.SqlClient;
//using BangazonAPI.Models;
//using Microsoft.AspNetCore.Http;

//namespace BangazonAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrderController : ControllerBase
//    {
//        private readonly IConfiguration _config;
//        public OrderController(IConfiguration config)
//        {
//            _config = config;
//        }

//        public SqlConnection Connection
//        {
//            get
//            {
//                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//            }
//        }

//        //Get All
//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"Select o.Id, o.CustomerId, o.UserPaymentTypeId, p.Id, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Description, op.Id, op.OrderId, op.ProductId
//                        FROM Order o
//                        Left Join Product p
//                        On op.ProductId = p.Id
//                        LEFT Join OrderProduct op
//                        ON o.Id = op.OrderId
//                        WHERE o.CustomerId = customerId";

//                    SqlDataReader reader = cmd.ExecuteReader();

//                    List<Order> orders = new List<Order>();
//                    Order order = null;

//                    while (reader.Read())
//                    {
//                         order = new Order
//                        {
//                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
//                            UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId")),
//                             Product = new Product()
//                             {
//                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
//                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
//                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
//                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
//                                Title = reader.GetString(reader.GetOrdinal("Title")),
//                                Description = reader.GetString(reader.GetOrdinal("Description"))
//                            }
//                        };

//                        //if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
//                        //{
//                        //    order.Notes = reader.GetString(reader.GetOrdinal("Notes"));
//                        //}

//                        orders.Add(order);
//                    }
//                    reader.Close();

//                    return Ok(orders);
//                }
//            }
//        }

//        //Get by ID
//        [HttpGet("{id}", Name = "GetOrder")]
//        public async Task<IActionResult> Get([FromRoute] int id)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"Select o.Id, o.CustomerId, o.UserPaymentTypeId, o.ProductId, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description
//                        FROM Order o
//                        Left Join Product p
//                        On o.ProductId = p.Id
//                        WHERE o.Id = @id";

//                    cmd.Parameters.Add(new SqlParameter("@id", id));
//                    SqlDataReader reader = cmd.ExecuteReader();

//                    Order order = null;

//                    if (reader.Read())
//                    {
//                        order = new Order
//                        {
//                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
//                            UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId")),
//                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
//                            Product = new Product()
//                            {
//                                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
//                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
//                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
//                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
//                                Title = reader.GetString(reader.GetOrdinal("Title")),
//                                Description = reader.GetString(reader.GetOrdinal("Description")),
//                            }
//                        };
//                        //if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
//                        //{
//                        //    order.Notes = reader.GetString(reader.GetOrdinal("Notes"));
//                        //}
//                    }
//                    reader.Close();

//                    return Ok(order);
//                }
//            }
//        }
//        //Post
//        [HttpPost]
//        public async Task<IActionResult> Post([FromBody] Order order)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand()) 

//                {
//                    cmd.CommandText = @"INSERT INTO Owner (CustomerId, UserPaymentTypeId, ProductId)
//                                        OUTPUT INSERTED.Id
//                                        VALUES (@customerId, @userPaymentTypeId, @productId)";

//                    cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
//                    cmd.Parameters.Add(new SqlParameter("@userPaymentTypeId", order.UserPaymentTypeId));
//                    cmd.Parameters.Add(new SqlParameter("@productId", order.ProductId));
//                    //(object) is a cast it's like a built in interface to treat these as an object, allowing you use "??" because order.Notes and DBNull are different types 
//                    //the "??" coaslesce operator specifies to use DBNull.Value as the backup if order.Notes is empty
//                    //cmd.Parameters.Add(new SqlParameter("@notes", (object)order.Notes ?? DBNull.Value));

//                    int newId = (int)cmd.ExecuteScalar();
//                    order.Id = newId;
//                    return CreatedAtRoute("GetOrder", new { id = newId }, order);
//                }
//            }
//        }
//        //Put
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
//        {
//            try
//            {
//                using (SqlConnection conn = Connection)
//                {
//                    conn.Open();
//                    using (SqlCommand cmd = conn.CreateCommand())
//                    {
//                        {
//                            cmd.CommandText = @"UPDATE Order
//                                            SET CustomerId = @customerId, UserPaymentTypeId = @userPaymentTypeId, ProductId = @productId
//                                            WHERE Id = @id";

//                            cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
//                            cmd.Parameters.Add(new SqlParameter("@userPaymentTypeId", order.UserPaymentTypeId));
//                            cmd.Parameters.Add(new SqlParameter("@productId", order.ProductId));                            

//                            int rowsAffected = cmd.ExecuteNonQuery();
//                            if (rowsAffected > 0)
//                            {
//                                return new StatusCodeResult(StatusCodes.Status204NoContent);
//                            }
//                            throw new Exception("No Rows Affected");
//                        }
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                if (!OrderExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//        }

//        //Delete
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete([FromRoute] int id)
//        {
//            try
//            {
//                using (SqlConnection conn = Connection)
//                {
//                    conn.Open();
//                    using (SqlCommand cmd = conn.CreateCommand())
//                    {
//                        cmd.CommandText = @"DELETE FROM Order WHERE Id = @id";
//                        cmd.Parameters.Add(new SqlParameter("@id", id));

//                        int rowsAffected = cmd.ExecuteNonQuery();
//                        if (rowsAffected > 0)
//                        {
//                            return new StatusCodeResult(StatusCodes.Status204NoContent);
//                        }
//                        throw new Exception("No rows affected");
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                if (!OrderExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//        }


//        //Method to check if Ordergo Exists
//        private bool OrderExists(int id)
//        {
//            using (SqlConnection conn = Connection)
//            {
//                conn.Open();
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    cmd.CommandText = @"Select o.Id, o.CustomerId, o.UserPaymentTypeId, o.ProductId, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description
//                        FROM Order o
//                        Left Join Product p
//                        On o.ProductId = p.Id
//                        WHERE o.Id = @id";
//                    cmd.Parameters.Add(new SqlParameter("@id", id));

//                    SqlDataReader reader = cmd.ExecuteReader();
//                    return reader.Read();
//                }
//            }
//        }
//    }
//}
