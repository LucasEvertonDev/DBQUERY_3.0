using DB.Query.CommercialDb.Entities;
using DB.Query.CommercialDb.Storeds;
using DB.Query.Core;
using Microsoft.Data.SqlClient;
using System.Data;
using DB.Query.Extensions;

namespace DB.Query.Example.Examples
{
    public class Example : DBQueryTransaction
    {
        private string connectionString = "Server=localhost;Database=CommercialDB;Integrated Security=True;";

        public List<Product> AddProductAndGetFilteredProducts(string productName, string productDescription, decimal productPrice,
            int stockQuantity, string category, string productCode)
        {
            List<Product> products = new List<Product>();

            // Chamada para a stored procedure
            string storedProcedure = "GetProductsByNameAndDescription";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    connection.Open();

                    transaction = connection.BeginTransaction();

                    string insertQuery = @"
                        INSERT INTO Product (ProductName, ProductDescription, ProductPrice, StockQuantity, Category, ProductCode)
                        VALUES (@ProductName, @ProductDescription, @ProductPrice, @StockQuantity, @Category, @ProductCode)";

                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@ProductName", productName);
                        insertCommand.Parameters.AddWithValue("@ProductDescription", productDescription);
                        insertCommand.Parameters.AddWithValue("@ProductPrice", productPrice);
                        insertCommand.Parameters.AddWithValue("@StockQuantity", stockQuantity);
                        insertCommand.Parameters.AddWithValue("@Category", category);
                        insertCommand.Parameters.AddWithValue("@ProductCode", productCode);

                        insertCommand.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand(storedProcedure, connection, transaction))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductName", productName);
                        command.Parameters.AddWithValue("@ProductDescription", productDescription);

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Product product = new Product()
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                ProductDescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ProductPrice = reader.GetDecimal(3),
                                StockQuantity = reader.GetInt32(4),
                                Category = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ProductCode = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };

                            products.Add(product);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    Console.WriteLine("Erro: " + ex.Message);
                }
            }

            return products;
        }

        public List<Product> AddProductAndGetFilteredProducts2(string productName, string productDescription, decimal productPrice,
         int stockQuantity, string category, string productCode)
        {
            List<Product> products = new List<Product>();

            string storedProcedure = "GetProductsByNameAndDescription";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    connection.Open();

                    transaction = connection.BeginTransaction();

                    var dbTransaction = connection.InstanceDbQueryTransaction(transaction);

                    dbTransaction.Query<Product>()
                        .Insert(new Product
                        {
                            ProductName = productName,
                            ProductDescription = productDescription,
                            Category = category,
                            ProductCode = productCode,
                            ProductPrice = productPrice,
                            StockQuantity = stockQuantity
                        })
                        .Execute();

                    using (SqlCommand command = new SqlCommand(storedProcedure, connection, transaction))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductName", productName);
                        command.Parameters.AddWithValue("@ProductDescription", productDescription);

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Product product = new Product()
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                ProductDescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ProductPrice = reader.GetDecimal(3),
                                StockQuantity = reader.GetInt32(4),
                                Category = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ProductCode = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };

                            products.Add(product);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    Console.WriteLine("Erro: " + ex.Message);
                }
            }

            return products;
        }

        public List<GetProductsByNameAndDescriptionResult> AddProductAndGetFilteredProducts3(string productName, string productDescription, decimal productPrice,
            int stockQuantity, string category, string productCode)
        {
            return OnTransaction((transaction) =>
            {
                transaction.Query<Product>()
                   .Insert(new Product
                   {
                       ProductName = productName,
                       ProductDescription = productDescription,
                       Category = category,
                       ProductCode = productCode,
                       ProductPrice = productPrice,
                       StockQuantity = stockQuantity
                   })
                   .Execute();

                var products = transaction.ExecuteStored<GetProductsByNameAndDescriptionResult>(
                    new GetProductsByNameAndDescriptionParameters
                    {
                        ProductName = productName,
                        ProductDescription = productDescription,
                    });

                return products;
            });
        }
    }
}
