using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueReportController : ControllerBase
    {
        private readonly IConfiguration _config;
        public RevenueReportController(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultCOnnection"));
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    {
                        cmd.CommandText = @"
SELECT pt.Id AS ProductTypeId, ISNULL(SUM(sales.Price), 0) AS Price, pt.[Name] FROM ProductType pt
LEFT JOIN
(
SELECT p.Price, p.ProductTypeId FROM Product p
JOIN OrderProduct op ON op.ProductId = p.Id
JOIN[Order] o ON o.Id = op.OrderId
WHERE o.UserPaymentTypeId is not null
)
Sales ON sales.ProductTypeId = pt.Id
GROUP BY pt.Id, pt.[Name]";
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<RevenueReport> revenueReport = new List<RevenueReport>();
                        RevenueReport reportItem = null;
                        while (reader.Read())
                        {
                            reportItem = new RevenueReport
                            {
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("Id")),
                                ProductType = reader.GetString(reader.GetOrdinal("Name"))
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Revenue")))
                            {
                                reportItem.TotalRevenue = reader.GetDecimal(reader.GetOrdinal("Revenue"));
                            }
                            else
                            {
                                reportItem.TotalRevenue = 0;
                            }
                            revenueReport.Add(reportItem);
                        }
                        reader.Close();
                        return Ok(revenueReport);
                    }
                }
            }
        }
    }
}