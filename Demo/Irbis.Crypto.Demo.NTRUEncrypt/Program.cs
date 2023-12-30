using System.Numerics;

using Irbis.Crypto.Cipher.NTRUEncrypt;

var polynomial1 = new NTRUPolynomial(new Dictionary<BigInteger, BigInteger>
{
    { 0, -1 },
    { 1, 1 },
    { 2, 1 },
    { 4, -1 },
    { 6, 1 },
    { 9, 1 },
    { 10, -1 }
});

var polynomial2 = new NTRUPolynomial(new Dictionary<BigInteger, BigInteger>
{
    { 0, 5 },
    { 1, 9 },
    { 2, 6 },
    { 3, 16 },
    { 4, 4 },
    { 5, 15 },
    { 6, 16 },
    { 7, 22 },
    { 8, 20 },
    { 9, 18 },
    { 10, 30 }
});

var polynomial3 = new NTRUPolynomial(new Dictionary<BigInteger, BigInteger>
{
    { 0, 1 },
    { 1, 2 },
    { 3, 2 },
    { 4, 2 },
    { 5, 1 },
    { 7, 2 },
    { 8, 1 },
    { 9, 1 }
});

Console.WriteLine($"poly1: {polynomial1}");
Console.WriteLine($"poly2: {polynomial2}");
Console.WriteLine($"poly3: {polynomial3}");

var sum = NTRUPolynomial.Sum(polynomial1, polynomial2, 11);
var multiplication = NTRUPolynomial.Multiplication(polynomial1, polynomial2, 11, 4);
var invertedMod3 = NTRUPolynomial.Inverted(polynomial2, 11, 3);
var invertedMod32 = NTRUPolynomial.Inverted(polynomial2, 11, 32);

Console.WriteLine($"poly1 + poly2 = {sum}");
Console.WriteLine($"poly1 * poly2 = {multiplication}");
Console.WriteLine($"(poly1)^-1 mod 32 = {invertedMod32}");
Console.WriteLine($"(poly1)^-1 mod 3 = {invertedMod3}");