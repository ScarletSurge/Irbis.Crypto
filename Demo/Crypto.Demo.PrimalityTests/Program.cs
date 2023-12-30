using System.Numerics;

using Crypto.Auxiliary.PrimalityTests;

var tests = new IPrimalityTest[]
{
    new FermatPrimalityTest(),
    new SolovayStrassenPrimalityTest(),
    new MillerRabinPrimalityTest()
};

var valueToTest = BigInteger.Parse("65537");
var primalityProbability = 0.97;

foreach (var test in tests)
{
    Console.WriteLine($"{test}: {test.CheckPrimality(valueToTest, primalityProbability)}");
}