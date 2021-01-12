using System;

namespace CatMash
{
    public static class FirestoreUtils
    {
        public static string ExtractIdFromName(string name) => name.Split("/").Last();
    }
}