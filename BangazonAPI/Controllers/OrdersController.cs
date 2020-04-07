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

<<<<<<< HEAD
namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;
        public OrdersController(IConfiguration config)
        {
            _config = config;
        }
=======
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
>>>>>>> dfc50b8c9a80e013b8d18b004c376e7f54cf7453

//        public SqlConnection Connection
//        {
//            get
//            {
//                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
//            }
//        }

<<<<<<< HEAD
        //get by customerId
      

        //Get All
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int? customerId,
            [FromQuery] bool cart)
            {
                //if cart is true and ID is not null
                if (cart == true && customerId != null)
                {
                    var order = GetOrderWithCart(customerId);
                    return Ok(order);
                }
                //
                else
                {
                    var order = GetOrders(customerId);
                    return Ok(order);
                }


            }

        //Get by ID
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id as OrderID, o.CustomerId as OrderCustomerID, o.UserPaymentTypeId as UserPaymentTypeId,
                    OrderProduct.OrderId as OPOrderID, OrderProduct.ProductID as OPProductID,
                    P.Id as ProductId, P.DateAdded as ProductDateAdded, P.ProductTypeId as ProductTypeId, P.CustomerId as ProductCustomerID, P.Price, P.Title, P.Description
                    FROM [Order] o
                    LEFT JOIN OrderProduct ON o.Id = OrderProduct.OrderId
                    LEFT JOIN Product as P ON OrderProduct.ProductId = P.Id
                    WHERE o.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
=======
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
>>>>>>> dfc50b8c9a80e013b8d18b004c376e7f54cf7453

//                    Order order = null;

<<<<<<< HEAD
                    while (reader.Read())
                    {
                        if (order == null)
                        {
                            //creates order
                            order = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("OrderCustomerId")),
                                Products = new List<Product>()
                            };

                            //If there is a payment attachmented it assigns the ID from db to the order
                            if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                            {
                                order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                            }
                        }

                        //If there is a productId 
                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                                //creates product
                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("ProductDateAdded")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                };
                                // Added product to order
                                order.Products.Add(product);
                        }
                                
                       
                    }
                        reader.Close();
                        return Ok(order);
                } 
            } 
        }


        //Post
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerProduct customerProduct)
        {
            Order order = OrderCartExists(customerProduct.CustomerId);
            if (order.Id == 0)
            {
                //Post METHOD GO CREATE AN EMPTY ORDER
                PostOrder(order, customerProduct.CustomerId);
                //reset order to be the order that gets created
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) 

                {
                    cmd.CommandText = @"INSERT INTO OrderProduct (OrderId, ProductId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@OrderId, @productId)";

                    cmd.Parameters.Add(new SqlParameter("@OrderId", order.Id));
                    cmd.Parameters.Add(new SqlParameter("@ProductId", customerProduct.ProductId));
                    //(object) is a cast it's like a built in interface to treat these as an object, allowing you use "??" because order.Notes and DBNull are different types 
                    //the "??" coaslesce operator specifies to use DBNull.Value as the backup if order.Notes is empty
                    //cmd.Parameters.Add(new SqlParameter("@notes", (object)order.Notes ?? DBNull.Value));

                    cmd.ExecuteScalar();
                    return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
                }
            }
        }

        //Put updates userpaymentType only meaning, this is the endpoint to purchase an order that is in cart
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        {
                            cmd.CommandText = @"UPDATE [Order]
                                            SET UserPaymentTypeId = @userPayment 
                                            WHERE Id = @id";
                                            
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.Parameters.Add(new SqlParameter("@userPayment", order.UserPaymentTypeId));                           
=======
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
>>>>>>> dfc50b8c9a80e013b8d18b004c376e7f54cf7453

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

<<<<<<< HEAD

        //Delete product from an order
        [HttpDelete("{id}/products{productid}")]
        public async Task<IActionResult> Delete([FromRoute] int id, int productId)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE OrderProduct 
                                            FROM OrderProduct
                                            LEFT JOIN [Order] on [Order].Id = OrderProduct.OrderId
                                            WHERE OrderProduct.ProductId = @productId AND OrderId = @OrderId AND [Order].UserPaymentTypeId IS NULL";
                        cmd.Parameters.Add(new SqlParameter("@productId", productId));
                        cmd.Parameters.Add(new SqlParameter("@OrderId", id));
=======
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
>>>>>>> dfc50b8c9a80e013b8d18b004c376e7f54cf7453

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


<<<<<<< HEAD

        private List<Order> GetOrderWithCart(int? customerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id as OrderID, o.CustomerId as OrderCustomerID, o.UserPaymentTypeId as UserPaymentTypeId,
                    OrderProduct.OrderId as OPOrderID, OrderProduct.ProductID as OPProductID,
                    P.Id as ProductId, P.DateAdded as ProductDateAdded, P.ProductTypeId as ProductTypeId, P.CustomerId as ProductCustomerID, P.Price, P.Title, P.Description
                    FROM [Order] o
                    LEFT JOIN OrderProduct ON o.Id = OrderProduct.OrderId
                    LEFT JOIN Product as P ON OrderProduct.ProductId = P.Id
                    WHERE UserPaymentTypeId IS NULL
                    AND o.CustomerId = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", customerId));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Order> orders = new List<Order>();

                    while (reader.Read())
                    {
                        var orderId = reader.GetInt32(reader.GetOrdinal("OrderID"));
                        var orderAlreadyCreated = orders.FirstOrDefault(o => o.Id == orderId);
                        if (orderAlreadyCreated == null)
                        {
                            Order order = new Order

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("OrderCustomerId")),
                                Products = new List<Product>()
                            };
                            //If there is a payment attachmented it assigns the ID from db to the order
                            if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                            {
                                order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                            }
                            orders.Add(order);
                            var hasProduct = !reader.IsDBNull(reader.GetOrdinal("ProductId"));
                            if (hasProduct)
                            {

                                //creates product
                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("ProductDateAdded")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                };
                                // Added product to order
                                order.Products.Add(product);
                            }
                        }
                        else
                        {
                            var hasProduct = !reader.IsDBNull(reader.GetOrdinal("ProductId"));
                            if (hasProduct)
                            {

                                //creates product
                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("ProductDateAdded")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                };
                                // Added product to order
                                orderAlreadyCreated.Products.Add(product);
                            }
                        }

                    }
                    reader.Close();
                    return orders;
                }
            }
        }

        private List<Order> GetOrders(int? customerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id as OrderID, o.CustomerId as OrderCustomerID, o.UserPaymentTypeId as UserPaymentTypeId,
                    OrderProduct.OrderId as OPOrderID, OrderProduct.ProductID as OPProductID,
                    P.Id as ProductId, P.DateAdded as ProductDateAdded, P.ProductTypeId as ProductTypeId, P.CustomerId as ProductCustomerID, P.Price, P.Title, P.Description
                    FROM [Order] o
                    LEFT JOIN OrderProduct ON o.Id = OrderProduct.OrderId
                    LEFT JOIN Product as P ON OrderProduct.ProductId = P.Id
                    WHERE UserPaymentTypeId IS NOT NULL
                    AND o.CustomerId = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", customerId));
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Order> orders = new List<Order>();

                    while (reader.Read())
                    {
                        var orderId = reader.GetInt32(reader.GetOrdinal("OrderID"));
                        var orderAlreadyCreated = orders.FirstOrDefault(o => o.Id == orderId);
                        if (orderAlreadyCreated == null)
                        {
                            Order order = new Order

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("OrderCustomerId")),
                                Products = new List<Product>()
                            };
                            //If there is a payment attachmented it assigns the ID from db to the order
                            if (!reader.IsDBNull(reader.GetOrdinal("UserPaymentTypeId")))
                            {
                                order.UserPaymentTypeId = reader.GetInt32(reader.GetOrdinal("UserPaymentTypeId"));
                            }
                            orders.Add(order);
                            var hasProduct = !reader.IsDBNull(reader.GetOrdinal("ProductId"));
                            if (hasProduct)
                            {

                                //creates product
                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("ProductDateAdded")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                };
                                // Added product to order
                                order.Products.Add(product);
                            }
                        }
                        else
                        {
                            var hasProduct = !reader.IsDBNull(reader.GetOrdinal("ProductId"));
                            if (hasProduct)
                            {

                                //creates product
                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    DateAdded = reader.GetDateTime(reader.GetOrdinal("ProductDateAdded")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
                                };
                                // Added product to order
                                orderAlreadyCreated.Products.Add(product);
                            }
                        }
                        
                    }
                    reader.Close();
                    return orders;
                }
            }
        }

        //method to create an order typically used if order does not already exist
        private Order PostOrder(Order order, int customerId)
        {
            order.CustomerId = customerId;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO [Order] (CustomerId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@CustomerId)";
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                    int newId = (int)cmd.ExecuteScalar();
                    order.Id = newId;
                    return order;
                }
            }
        }




        //Method to check if Order Exists
        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select o.Id, o.CustomerId, o.UserPaymentTypeId, o.ProductId, p.DateAdded, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description
                        FROM Order o
                        Left Join Product p
                        On o.ProductId = p.Id
                        WHERE o.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
        //method that checks to see if shopping cart exists already
         private Order OrderCartExists(int customerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, CustomerId, UserPaymentTypeId
                        FROM [Order]
                        WHERE CustomerId = @customerId AND UserPaymentTypeId IS NULL";
                    cmd.Parameters.Add(new SqlParameter("@customerId", customerId));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Order orderCheck = new Order();

                    while (reader.Read())
                    {
                        orderCheck.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        orderCheck.CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                    }

                    reader.Close();

                    return orderCheck;
                }
            }
        }
        
    }
}
=======
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
>>>>>>> dfc50b8c9a80e013b8d18b004c376e7f54cf7453
