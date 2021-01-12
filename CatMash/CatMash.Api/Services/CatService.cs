using CatMash.Api.Models;
using Google.Cloud.Firestore.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatMash.Api.Services
{
    internal class CatService : ICatService
    {
        private readonly FirestoreClient firestore;
        private bool catsCacheInitialized;
        private readonly IList<CatModel> catsCache;

        private const string DocumentsRoot = "projects/catmash-plml/databases/(default)/documents";

        private const string CatsCollection = "cats";
        private const string ProposalsCollection = "proposals";
        private const string VotesCollection = "votes";

        public CatService(FirestoreClient firestore)
        {
            this.firestore = firestore;
            this.catsCache = new List<CatModel>();
        }

        public async Task<ProposalModel> RequireNewPorposal(string? userId)
        {
            if (!catsCacheInitialized)
                await InitializeCatsCache();

            CatModel cat1, cat2;

            do
            {
                cat1 = catsCache.RandomElement();
                cat2 = catsCache.RandomElement();
            }
            while (cat1.Equals(cat2));

            Document newDoc = new();
            
            if (userId != null)
                newDoc.Fields.Add("userId", new() { StringValue = userId });

            newDoc.Fields.Add("cat1Id", new() { StringValue = cat1.CatId });
            newDoc.Fields.Add("cat2Id", new() { StringValue = cat2.CatId });

            CreateDocumentRequest createReq = new()
            {
                Document = newDoc,
                Parent = DocumentsRoot,
                CollectionId = ProposalsCollection
            };
            Document created = await firestore.CreateDocumentAsync(createReq);

            string proposalId = FirestoreUtils.ExtractIdFromName(created.Name);
            return new ProposalModel(proposalId, userId, cat1, cat2);
        }

        public async Task SubmitVote(VoteModel vote)
        {
            Document newDoc = new();
            newDoc.Fields.Add("proposalId", new() { StringValue = vote.ProposalId });
            newDoc.Fields.Add("catId", new() { StringValue = vote.CatId });

            if (vote.UserId is not null)
                newDoc.Fields.Add("userId", new() { StringValue = vote.UserId });

            CreateDocumentRequest createReq = new()
            {
                Document = newDoc,
                Parent = DocumentsRoot,
                CollectionId = VotesCollection
            };
            await firestore.CreateDocumentAsync(createReq);
        }

        private async Task InitializeCatsCache()
        {
            ListDocumentsRequest listReq = new()
            {
                Parent = DocumentsRoot,
                CollectionId = CatsCollection
            };
            IAsyncEnumerable<Document> response = firestore.ListDocumentsAsync(listReq);

            await foreach (Document doc in response)
            {
                string catId = FirestoreUtils.ExtractIdFromName(doc.Name);
                string src = doc.Fields["url"].StringValue;

                catsCache.Add(new(catId, src));
            }

            catsCacheInitialized = true;
        }
    }
}