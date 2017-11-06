using System;
using System.Collections.Generic;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

namespace TableStorageSample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));
                Console.WriteLine($"storageAccount={storageAccount}");

                // Create the table client.
                var tableClient = storageAccount.CreateCloudTableClient();

                Console.WriteLine($"create table");
                // テーブルを作成する
                var table = tableClient.GetTableReference("samplepeople");

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

//                // データを挿入する
//                var customer1 = new CustomerEntity("Harp", "Walter");
//                customer1.Email = "Walter@contoso.com";
//                customer1.PhoneNumber = "425-555-0101";
//
//                // Create the TableOperation object that inserts the customer entity.
//                var insertOperation = TableOperation.Insert(customer1);
//
//                // Execute the insert operation.
//                table.Execute(insertOperation);

                InsertRows(table);

                // データを取得する
                // Construct the query operation for all customer entities where PartitionKey="Smith".
                TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

                // Print the fields for each customer.
                foreach (CustomerEntity entity in table.ExecuteQuery(query))
                {
                    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                        entity.Email, entity.PhoneNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error. msg={ex.Message}");
            }
            Console.WriteLine($"終了しました(Push any key)");
            Console.ReadKey();
        }

        private static void InsertRows(CloudTable table)
        {
            try
            {
                Console.WriteLine($"insert data");
                // バッチで挿入する
                TableBatchOperation batchOperation = new TableBatchOperation();

                CustomerEntity customer1 = new CustomerEntity("Smith", "Jeff");
                customer1.Email = "Jeff@contoso.com";
                customer1.PhoneNumber = "425-555-0104";

                CustomerEntity customer2 = new CustomerEntity("Smith", "Ben");
                customer2.Email = "Ben@contoso.com";
                customer2.PhoneNumber = "425-555-0102";

                batchOperation.Insert(customer1);
                batchOperation.Insert(customer2);

                table.ExecuteBatch(batchOperation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"挿入時エラー msg={ex.Message}");
            }
        }
    }
}