﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase

    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
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



        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string sortBy, [FromQuery]  string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description 
                                      FROM Product
                                      WHERE 1 = 1";

                    if(q != null)
                    {
                        cmd.CommandText += " AND Title LIKE @title OR Description LIKE @description";
                        cmd.Parameters.Add(new SqlParameter("@Title", "%" + q + "%"));
                        cmd.Parameters.Add(new SqlParameter("@Description", "%" + q + "%"));

                    }

                    if(sortBy == "recent")
                    {
                        cmd.CommandText = @"Select Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description 
                                            FROM Product
                                            WHERE 1=1
                                            ORDER BY DateAdded DESC";
                    }

                    if (sortBy == "popularity")
                    {
                        cmd.CommandText = @"Select p.Id, p.ProductTypeId, p.CustomerId, p.Price, p.[Description], p.Title, p.DateAdded, COUNT(op.ProductId) AS PopularCount
                                            FROM Product p                                      
                                            LEFT JOIN OrderProduct op ON p.Id = op.ProductId
                                            GROUP BY p.Id, p.ProductTypeId, p.CustomerId, p.Price, p.[Description], p.Title, p.DateAdded                                       
                                            HAVING 1=1";
                    }

                        SqlDataReader reader = cmd.ExecuteReader();
                        List<Product> products = new List<Product>();
                        
                        while (reader.Read())
                        {
                            Product product = new Product
                           {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                DateAdded = reader.GetDateTime(reader.GetOrdinal("DateAdded")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description"))
                                
                            };

                        products.Add(product);
                }
                reader.Close();
                return Ok(products);

                }
            }




        }
        

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Product (DateAdded, ProductTypeId, CustomerId, Price, Title, Description)
                                        OUTPUT INSERTED.Id
                                        VALUES (@DateAdded, @ProductTypeId, @CustomerId, @Price, @Title, @Description)";
                    cmd.Parameters.Add(new SqlParameter("@DateAdded", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@ProductTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", product.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@Price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@Title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@Description", product.Description));




                    int newId = (int)cmd.ExecuteScalar();
                    product.Id = newId;
                    product.DateAdded = DateTime.Now;
                    return CreatedAtRoute("GetProduct", new { id = newId }, product);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product product)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Product
                                           SET  ProductTypeId = @ProductTypeId,
                                                CustomerId = @CustomerId,
                                                Price = @Price, 
                                                Title = @Title,
                                                Description = @Description
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@ProductTypeId", product.ProductTypeId));
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", product.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@Price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@Title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@Description", product.Description));

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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
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
                        cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

       



        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, DateAdded, ProductTypeId, CustomerId, Price, Title, Description
                        FROM Product
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}