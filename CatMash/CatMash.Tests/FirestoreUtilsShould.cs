using NFluent;
using NUnit.Framework;

namespace CatMash.Tests
{
    public class FirestoreUtilsShould
    {
        [Test]
        [TestCase("projects/catmash-plml/databases/(default)/documents/cats/1dp", "1dp")]
        public void ExtractIdFromName_Returns_Expected(string input, string expected) => Check
            .That(FirestoreUtils.ExtractIdFromName(input))
            .IsEqualTo(expected);
    }
}