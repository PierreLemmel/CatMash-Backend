using CatMash.CLI.Models;
using Google.Cloud.Firestore.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatMash.CLI
{
    internal class DbSeeder : IDbSeeder
    {
        private const string CatsCollection = "cats";
        private const string DocumentsRoot = "projects/catmash-plml/databases/(default)/documents";
        private readonly FirestoreClient firestore;

        public DbSeeder(FirestoreClient firestore)
        {
            this.firestore = firestore;
        }

        public async Task SeedCatsCollection(string dataUrl)
        {
            string json;
            using (HttpClient client = new())
            {
                json = await client.GetStringAsync(dataUrl);
            }

            IEnumerable<CatModel> cats = JsonSerializer.Deserialize<JsonModel>(json)!.images;


            ListDocumentsRequest listReq = new ()
            {
                Parent = DocumentsRoot,
                CollectionId = CatsCollection
            };
            IAsyncEnumerable<Document> response = firestore.ListDocumentsAsync(listReq);

            await foreach(Document doc in response)
            {
                await firestore.DeleteDocumentAsync(new DeleteDocumentRequest() { Name = doc.Name });
            }

            foreach(var cat in cats)
            {
                Document newDoc = new();
                newDoc.Fields.Add("url", new() { StringValue = cat.url });

                CreateDocumentRequest createReq = new()
                {
                    Parent = DocumentsRoot,
                    CollectionId = CatsCollection,
                    DocumentId = cat.id,
                    Document = newDoc
                };
                await firestore.CreateDocumentAsync(createReq);
            }
        }
    }
}