using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAzureAndWebServicesUnitTestProject
{
    class ChooseDataAccessTechnologies_1_1
    {
        [TestCase(3)]
        public static void GetCustomersWithDataAdapter(int customerId)
        {
            // ARRANGE
            DataSet customerData = new DataSet("CustomerData");
            DataTable customerTable = new DataTable("Customer");
            customerData.Tables.Add(customerTable);
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT FirstName, LastName, CustomerId, AccountId");
            sql.Append(" FROM [dbo].[Customer] WHERE CustomerId = @CustomerId ");

            // ACT
            // Assumes an app.config file has connectionString added to <connectionStrings> section named "TestDB"
            using (SqlConnection mainConnection =
            new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString))
            {
                using (SqlCommand customerQuery = new SqlCommand(sql.ToString(), mainConnection))
                {
                    customerQuery.Parameters.AddWithValue("@CustomerId", customerId);
                    using (SqlDataAdapter customerAdapter = new SqlDataAdapter(customerQuery))
                    {
                        try
                        {
                            customerAdapter.Fill(customerData, "Customer");
                        }
                        finally
                        {
                            // This should already be closed even if we encounter an exception
                            // but making it explicit in code.
                            if (mainConnection.State != ConnectionState.Closed)
                            {
                                mainConnection.Close();
                            }
                        }
                    }
                }
            }
            // ASSERT
            Assert.That(customerTable.Rows.Count, Is.EqualTo(1), "We expected exactly 1 record to be returned.");

            Assert.That(customerTable.Rows[0].ItemArray[customerTable.Columns["customerId"].Ordinal],
            Is.EqualTo(customerId), "The record returned has an ID different than expected.");
        }


        [TestCase(3)]
        public static void GetCustomersWithDataReader(int customerId)
        {
            // ARRANGE
            // You should probably use a better data structure than a Tuple for managing your data.
            List<Tuple<string, string, int, int>> results = new List<Tuple<string, string, int,
           int>>();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT FirstName, LastName, CustomerId, AccountId");
            sql.Append(" FROM [dbo].[Customer] WHERE CustomerId = @CustomerId ");

            // ACT
            // Assumes an app.config file has connectionString added to <connectionStrings> section named "TestDB"
            using (SqlConnection mainConnection =
            new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].
           ConnectionString))
            {
                using (SqlCommand customerQuery = new SqlCommand(sql.ToString(),
               mainConnection))
                {
                    customerQuery.Parameters.AddWithValue("@CustomerId", customerId);
                    mainConnection.Open();
                    using (SqlDataReader reader = customerQuery.ExecuteReader(CommandBehavior.
                   CloseConnection))
                    {
                        try
                        {
                            int firstNameIndex = reader.GetOrdinal("FirstName");
                            int lastNameIndex = reader.GetOrdinal("LastName");
                            int customerIdIndex = reader.GetOrdinal("CustomerId");
                            int accountIdIndex = reader.GetOrdinal("AccountId");
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, string, int, int>(
                                (string)reader[firstNameIndex], (string)reader[lastNameIndex],
                                (int)reader[customerIdIndex], (int)reader[accountIdIndex]));
                            }
                        }
                        finally
                        {
                            // This will soon be closed even if we encounter an exception
                            // but making it explicit in code.
                            if (mainConnection.State != ConnectionState.Closed)
                            {
                                mainConnection.Close();
                            }
                        }
                    }
                }
            }
            // ASSERT
            Assert.That(results.Count, Is.EqualTo(1), "We expected exactly 1 record to be returned.");


            Assert.That(results[0].Item3, Is.EqualTo(customerId), "The record returned has an ID different than expected.");
        }

        //[TestCase(3)]
        //public static void GetCustomerById(int customerId)
        //{
        //    // ARRANGE
        //    TestEntities database = new TestEntities();

        //    // ACT
        //    Customer result = database.Customers.SingleOrDefault(cust => cust.CustomerId == customerId);
        //    // ASSERT
        //    Assert.That(result, Is.Not.Null, "Expected a value. Null here indicates no record with this ID.");

        //    Assert.That(result.CustomerId, Is.EqualTo(customerId), "Uh oh!");
        //}

        //[TestCase(3)]
        //public static void GetCustomerByIdOnObjectContext(int customerId)
        //{
        //    // ARRANGE
        //    TestEntities database = new TestEntities();
        //    ObjectContext context = ConvertContext(database);

        //    // ACT
        //    ObjectSet<Customer> customers = context.CreateObjectSet<Customer>("Customers");
        //    Customer result = customers.SingleOrDefault(cust => cust.CustomerId == customerId);

        //    //Customer result = database.Customers.SingleOrDefault(cust => cust.CustomerId == customerId);
        //    // ASSERT
        //    Assert.That(result, Is.Not.Null, "Expected a value. Null here indicates no record with this ID.");

        //    Assert.That(result.CustomerId, Is.EqualTo(customerId), "Uh oh!");
        //}

        //[TestCase(true, 2)]
        //[TestCase(false, 1)]
        //public static void GetAccountsByAliasName(bool useCSharpNullBehavior, int recordsToFind)
        //{
        //    // ARRANGE
        //    TestEntities database = new TestEntities();
        //    ObjectContext context = ConvertContext(database);
        //    ObjectSet<Account> accounts = context.CreateObjectSet<Account>("Accounts");
        //    // ACT
        //    context.ContextOptions.UseCSharpNullComparisonBehavior = useCSharpNullBehavior;
        //    int result = accounts.Count(acc => acc.AccountAlias != "Home");
        //    // ASSERT
        //    Assert.That(result, Is.EqualTo(recordsToFind), "Uh oh!");
        //}
    }
}