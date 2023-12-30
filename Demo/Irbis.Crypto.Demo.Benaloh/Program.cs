using System.Numerics;

using Irbis.Crypto.Auxiliary.PrimalityTests;
using Irbis.Crypto.Cipher.Benaloh;

const int testsCount = 15;

var randomSource = new Random();

// можно кастомить
var r = 65537;

Console.WriteLine($"Starting tests[{testsCount}]...");
Console.WriteLine($"{string.Concat(Enumerable.Repeat("----", 20))}{Environment.NewLine}");

var tests = new List<Task<(BigInteger, BigInteger, BigInteger)>>(testsCount);
for (var i = 0; i < testsCount; i++)
{
    tests.Add(Task<(BigInteger, BigInteger, BigInteger)>.Factory.StartNew(() =>
    {
        // m e Zr, m e [0, 65536)
        // для r = 2^(8 * k) + 1,
        // в качестве сообщения можно брать k байтов и превращать их в беззнаковое целое
        // здесь k = 2, r = 2^16 + 1 = 65537
        var initialMessage = new BigInteger(randomSource.Next(r - 1));

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        var BenalohImpl = new Benaloh(PrimalityTest.MillerRabin, 0.7, 16, r);

        var encryptedMessage = BenalohImpl.Encrypt(initialMessage);
        var decryptedMessage = BenalohImpl.Decrypt(encryptedMessage);

        return (initialMessage, encryptedMessage, decryptedMessage);
    }));
}

Task.WhenAll(tests).GetAwaiter().GetResult();

foreach (var completedTest in tests)
{
    var (initialMessage, encryptedMessage, decryptedMessage)
        = completedTest.Result;

    Console.WriteLine($"Message: {initialMessage}");

    Console.WriteLine($"{(initialMessage == decryptedMessage ? string.Empty : "!")}" +
                      $"OK, encrypted = {encryptedMessage}, decrypted = {decryptedMessage}{Environment.NewLine}");
}