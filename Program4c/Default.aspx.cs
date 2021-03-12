// CSS 436 - Program 4
// Student: Stephanie Moua 
// Date:    02/27/2021
// 
// Purpose: Use various services to deploy a Web App on a Cloud provider (https://program4-smoua.azurewebsites.net/)
//          with Azure Container, Azure Tables, and Endpoint URI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Program4c
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e){}

        // Load data button: 
        // Retrieve data from URL and copy to blob in container in Azure
        // Parse the data into <last> <first> <list of attributes> and loads into Azure Tables
        protected void Button1_Click(object sender, EventArgs e)
        {
            string connectionString = "<k>";
            string containerName = "program4-test";

            // Get data from source URL
            string url = "https://css490.blob.core.windows.net/lab4/input.txt";
            //string url = "https://program4smoua.blob.core.windows.net/program4-test/input.txt";
            var client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Check that there is a valid file
            if (!response.IsSuccessStatusCode) // 2xx or 3xx code 
            {
                Label1.Text = "Could not update database. No file at source URL. " + DateTime.Now;
            }
            else {
                // Retrieve data as a string
                string result = response.Content.ReadAsStringAsync().Result;

                // Upload to blob in Azure container as string
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                var blob = container.GetBlockBlobReference("input.txt");

                blob.DeleteIfExists();
                blob.Properties.ContentType = ".txt";
                blob.UploadText(result); 

                string blobContents = result;

                // Create table 
                var tableClient = storageAccount.CreateCloudTableClient();
                var peopleTable = tableClient.GetTableReference("People");
                peopleTable.CreateIfNotExists();

                // Add data to table: For each line in result, break into [last, first, list of dictionary<>]

                // Break the result into individual lines 
                string[] lines = blobContents.Split('\n');

                char delimiter = '=';
                string last = null;
                string first = null;

                // Process each line to add to table
                foreach (string phrase in lines)
                {
                    var attributes = new Dictionary<string, string>();

                    // 1 - Split by space character
                    string[] words = phrase.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    // 2 - Get first words and check that they don't contain a "=" 
                    int i = 0;
                    if (words.Length > i && !words[i].Contains(delimiter))
                    {
                        last = words[i];
                        i++;
                        if (words.Length > i && !words[i].Contains(delimiter))
                        {
                            first = words[i];
                            i++;
                        }
                        if (words.Length > i)
                        {
                            // 3 - Parse the leftover substring into dictionary as attributes
                            for (int j = i; j < words.Length; j++)
                            {
                                if (words[j].Contains(delimiter)) // check that each attribute is set with = symbol
                                {

                                    string[] property = words[j].Split(delimiter);
                                    string key = property[0];
                                    string value = property[1];

                                    if (!attributes.ContainsKey(key))
                                    {
                                        attributes.Add(key, value);
                                    }
                                }
                            }
                            // 4 - add last, first and dictionary to table 
                            CreateEntity(first, last, attributes, peopleTable);
                        }
                    }
                    else //no last name
                    {
                        Label1.Text = "Invalid line entry, no last name. " + DateTime.Now; ;
                    }
                }

                Label1.Text = "Upload complete. " + DateTime.Now;
            }

        } // End of Upload Data behavior button 

        // Create a new entity for Azure Tables for each line of data
        private static bool CreateEntity(string firstName, string lastName, Dictionary<string,string> attributes, CloudTable table)
        {
            var newEntity = new PeopleEntity(firstName, lastName) 
            { 
                DataItems = attributes
            };

            TableOperation insert = TableOperation.Insert(newEntity);
            try
            {
                table.Execute(insert);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Nested class to handle custom Azure Tables entities
        // Use a dictionary to store X many attributes 
        public class PeopleEntity : TableEntity
        {
            public PeopleEntity()
            {
                PartitionKey = null;
                RowKey = null;
                DataItems = new Dictionary<string, string>();
            }
            public PeopleEntity(string firstName, string lastName)
            {
                PartitionKey = firstName;
                RowKey = lastName;
                DataItems = new Dictionary<string, string>();
            }

            // Dictionary for each attribute <attribute 1, value>
            public Dictionary<string, string> DataItems { get; set; }

            public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                var results = base.WriteEntity(operationContext);
                foreach (var item in DataItems)
                {
                    results.Add("D_" + item.Key, new EntityProperty(item.Value));
                }
                return results;
            }

            public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                base.ReadEntity(properties, operationContext);

                DataItems = new Dictionary<string, string>();

                foreach (var item in properties)
                {
                    if (item.Key.StartsWith("D_"))
                    {
                        string realKey = item.Key.Substring(2);
                        DataItems[realKey] = item.Value.StringValue;
                    }
                }
            }
        }

        // Print dictionary of attributes for one entity as attribute1=value1
        public static string printDictionary (Dictionary<string, string> DataItems)
        {
            string dictionaryString = " {";
            foreach (KeyValuePair<string, string> keyValues in DataItems)
            {
                dictionaryString += keyValues.Key + " = " + keyValues.Value + '\t';
            }
            return dictionaryString.TrimEnd(',', ' ') + "}";
        }

        // Clear data button: 
        // Removes each entity from the table one by one until empty 
        protected void Button2_Click(object sender, EventArgs e)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=program4smoua;AccountKey=563PJFNbpI/J9w1T5Ucenu9skY88bGwiQNNWStouW0cXgk2lLqL4P/wbZGMXJk3oeYLGjPxu9+QqAxDTje+bIA==;EndpointSuffix=core.windows.net";
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();
            var peopleTable = tableClient.GetTableReference("People");
            peopleTable.CreateIfNotExists();

            // Get each partition/row
            TableQuery<PeopleEntity> query = new TableQuery<PeopleEntity>();

            foreach (PeopleEntity entity in peopleTable.ExecuteQuery(query))
            {
                deleteEntity(entity.PartitionKey, entity.RowKey, peopleTable);
            }
            Label1.Text = "Data cleared. " + DateTime.Now; ;
        }

        // Helper method for clear data:
        // Delete an entity given partition key and row key 
        protected void deleteEntity(string partition, string row, CloudTable peopleTable)
        {
            TableOperation retrieve = TableOperation.Retrieve<PeopleEntity>(partition, row);

            TableResult result = peopleTable.Execute(retrieve);

            var deleteEntity = (PeopleEntity)result.Result;

            TableOperation delete = TableOperation.Delete(deleteEntity);

            peopleTable.Execute(delete);
        }

        // Query button: 
        // Retrieve data from textbox 1 & 2 
        // Depending on the number of arguments entered, searches the database to find matches
        // Display results via label element
        protected void Button3_Click(object sender, EventArgs e)
        {
            Label2.Text = "";
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=program4smoua;AccountKey=563PJFNbpI/J9w1T5Ucenu9skY88bGwiQNNWStouW0cXgk2lLqL4P/wbZGMXJk3oeYLGjPxu9+QqAxDTje+bIA==;EndpointSuffix=core.windows.net";
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var peopleTable = tableClient.GetTableReference("People");
            peopleTable.CreateIfNotExists();

            // Get input from user via TextBox
            string first = Convert.ToString(TextBox1.Text);
            string last = Convert.ToString(TextBox2.Text);

            // Characters entered in both boxes, look for exact match
            if (first.Length > 0 && last.Length > 0)
            {
                if (SearchEntity(peopleTable, first, last).Length > 0)
                {
                    Label2.Text = SearchEntity(peopleTable, first, last) + "<br/>- " + DateTime.Now ;
                }
                else Label2.Text = "No match found. " + DateTime.Now;
            }

            // Characters entered in First name only, look for all match with same First name
            else if (first.Length > 0)
            {
                if (SearchFirst(peopleTable, first).Length > 0) {
                    Label2.Text = SearchFirst(peopleTable, first) + "<br/>- " + DateTime.Now;
                }
                else Label2.Text = "No match found. " + DateTime.Now;
            }
            // Characters entered in Last name only, look for all match with same Last name
            else if (last.Length > 0)
            {
                if (SearchLast(peopleTable, last).Length > 0)
                {
                    Label2.Text = SearchLast(peopleTable, last) + "<br/>- " + DateTime.Now;
                }
                else Label2.Text = "No match found. " + DateTime.Now;
            }
            // Boxes left blanks, cannot search database
            else
            {
                Label2.Text = "No input provided. " + DateTime.Now;
            }
        }

        // Helper method for Query button
        // Search matches with both first and last name values
        // Use a combination of filtering condition to retrieve data
        public static string SearchEntity(CloudTable table, string first, string last)
        {
            string filter1 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, last);
            string filter2 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, first);
            TableQuery<PeopleEntity> rangeQuery = new TableQuery<PeopleEntity>().Where(TableQuery.CombineFilters(filter1, TableOperators.And, filter2));

            string result = "";

            // Loop through the results, displaying information about the entity.
            foreach (PeopleEntity entity in table.ExecuteQuery(rangeQuery))
            {
                result += (entity.PartitionKey + " " + entity.RowKey) + printDictionary(entity.DataItems) + "<br/>";
            }
            return result;

        }

        // Helper method for Query button
        // Search matches with last name value
        public static string SearchLast(CloudTable table, string last)
        {
            // Create the table query.
            TableQuery<PeopleEntity> rangeQuery = new TableQuery<PeopleEntity>().Where(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, last));

            string result = "";

            // Loop through the results, displaying information about the entity.
            foreach (PeopleEntity entity in table.ExecuteQuery(rangeQuery))
            {
                result += (entity.PartitionKey + " " + entity.RowKey) + printDictionary(entity.DataItems) + "<br/>";
            }
            return result;

        }

        // Helper method for Query button
        // Search matches with first name value
        public static string SearchFirst(CloudTable table, string first)
        {

            // Create the table query.
            TableQuery<PeopleEntity> rangeQuery = new TableQuery<PeopleEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, first));

            string result = ""; 

            // Loop through the results, displaying information about the entity.
            foreach (PeopleEntity entity in table.ExecuteQuery(rangeQuery))
            {
                result += (entity.PartitionKey + " " + entity.RowKey) + printDictionary(entity.DataItems) + "<br/>";
            }
            return result;

        }


    }
}