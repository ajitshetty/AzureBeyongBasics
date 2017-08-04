using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureBeyongBasics.Controllers
{
    public class StorageController : Controller
    {
        // GET: Storage
        public ActionResult Index()
        {
            //Retrieve the connection string
            var storageConnectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;

            //Retrieve storage account
            var StorageAccount = CloudStorageAccount.Parse(storageConnectionString);

            //Create Table Client
            var tableClient = StorageAccount.CreateCloudTableClient();

            // Create CloudTable object that represents the "Customer"
            var table = tableClient.GetTableReference("customer");
            //Create teh table if it doesn't exist.
            table.CreateIfNotExists();

            //create a new customer entity
            var customer = new CustomerEntity(Guid.NewGuid())
            {
                FirstName = "Ajith",
                LastName = "Shetty",
                Email = "test@test.com"
            };

            // create a insert operation
            var insertOpertaion = TableOperation.Insert(customer);
            //execute teh insert operation
            table.Execute(insertOpertaion);

            var queueClient = StorageAccount.CreateCloudQueueClient();
            var q = queueClient.GetQueueReference("customerqueue");
            q.CreateIfNotExists();

            var message = new CloudQueueMessage(customer.RowKey);
            q.AddMessage(message);

            return View(customer);
        }
    }


    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(Guid employeeId)
        {
            PartitionKey = "Customer";
            RowKey = employeeId.ToString();
        }

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
    }
}