using CatMash.Api.Models;
using Google.Cloud.Firestore.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace CatMash.Api.Services
{
    internal class CatService : ICatService
    {
        private readonly FirestoreClient firestore;

        private const string DocumentsRoot = "projects/catmash-plml/databases/(default)/documents";

        private const string CatsCollection = "cats";
        private const string ProposalsCollection = "proposals";
        private const string VotesCollection = "votes";

        public CatService(FirestoreClient firestore)
        {
            this.firestore = firestore;
        }

        public async Task<ProposalModel> RequireNewPorposal(string? userId)
        {
            RunQueryRequest queryReq = CreateCatsQuery("url");

            IAsyncEnumerable<RunQueryResponse> responseStream = firestore.RunQuery(queryReq).GetResponseStream();
            IList<CatModel> cats = await responseStream.Select(response =>
            {
                Document doc = response.Document;

                string catId = FirestoreUtils.ExtractIdFromName(doc.Name);
                string src = doc.Fields["url"].StringValue;

                return new CatModel(catId, src);
            }).ToList();

            CatModel cat1, cat2;

            do
            {
                cat1 = cats.RandomElement();
                cat2 = cats.RandomElement();
            }
            while (cat1.Equals(cat2));


            string proposalId = "";
            async Task CreateProposalTask()
            {
                proposalId = await CreateProposal(userId, cat1, cat2);
            }

            await Task.WhenAll(
                IncreaseCatMatches(cat1.CatId),
                IncreaseCatMatches(cat2.CatId),
                CreateProposalTask()
            );

            return new ProposalModel(proposalId, userId, cat1, cat2);
        }

        public async Task<IReadOnlyCollection<CatStatModel>> GetCatStats()
        {
            RunQueryRequest queryReq = CreateCatsQuery("url", "matches", "votes");

            IAsyncEnumerable<RunQueryResponse> responseStream = firestore.RunQuery(queryReq).GetResponseStream();
            List<CatStatModel> cats = await responseStream.Select(response =>
            {
                Document doc = response.Document;

                string catId = FirestoreUtils.ExtractIdFromName(doc.Name);
                int matches = (int)doc.Fields["matches"].IntegerValue;
                int votes = (int)doc.Fields["votes"].IntegerValue;
                string src = doc.Fields["url"].StringValue;

                return new CatStatModel(catId, matches, votes, src);
            }).ToList();

            return cats
                .OrderByDescending(cat => cat.WinRate)
                .ThenByDescending(cat => cat.Matches)
                .ToList();
        }

        private static RunQueryRequest CreateCatsQuery(params string[] fields)
        {
            Projection select = new();
            select.Fields.Add(fields.Select(field => new FieldReference() { FieldPath = field }));

            StructuredQuery structure = new()
            {
                Select = select
            };
            structure.From.Add(new CollectionSelector()
            {
                CollectionId = CatsCollection
            });

            RunQueryRequest queryReq = new()
            {
                Parent = DocumentsRoot,
                StructuredQuery = structure,
            };
            return queryReq;
        }

        private async Task<string> CreateProposal(string? userId, CatModel cat1, CatModel cat2)
        {
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
            return proposalId;
        }

        private async Task IncreaseCatMatches(string catId) => await IncreaseCatField(catId, "matches");
        private async Task IncreaseCatVotes(string catId) => await IncreaseCatField(catId, "votes");

        private async Task IncreaseCatField(string catId, string field)
        {
            GetDocumentRequest getDocReq = new()
            {
                Name = $"{DocumentsRoot}/{CatsCollection}/{catId}",
            };

            Document catDoc = await firestore.GetDocumentAsync(getDocReq);

            Value matchesValue = catDoc.Fields.GetValue(field);
            matchesValue.IntegerValue++;

            UpdateDocumentRequest update = new() { Document = catDoc };
            await firestore.UpdateDocumentAsync(update);
        }

        public async Task SubmitVote(VoteModel vote)
        {
            (string proposalId, string catId, string? userId) = vote;

            await Task.WhenAll(
                CreateVote(proposalId, catId, userId),
                IncreaseCatVotes(catId)
            );
        }

        private async Task CreateVote(string proposalId, string catId, string? userId)
        {
            Document newDoc = new();
            newDoc.Fields.Add("proposalId", new() { StringValue = proposalId });
            newDoc.Fields.Add("catId", new() { StringValue = catId });

            if (userId is not null)
                newDoc.Fields.Add("userId", new() { StringValue = userId });

            CreateDocumentRequest createReq = new()
            {
                Document = newDoc,
                Parent = DocumentsRoot,
                CollectionId = VotesCollection
            };
            await firestore.CreateDocumentAsync(createReq);
        }
    }
}