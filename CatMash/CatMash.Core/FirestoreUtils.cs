using Google.Cloud.Firestore.V1;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;

namespace CatMash
{
    public static class FirestoreUtils
    {
        public static string ExtractIdFromName(string name) => name.Split("/").Last();

        public static Value GetValue(this MapField<string, Value> fields, string key) => fields.GetValueOrDefault(key)
            ?? throw new InvalidOperationException($"Missing field '{key}'");
    }
}