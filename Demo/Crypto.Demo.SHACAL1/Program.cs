using Crypto.Cipher.SHACAL1;

var randomSource = new Random();
var values = new uint[5];
var valueBytes = new byte[sizeof(uint)];
for (var i = 0; i < 5; i++)
{
    randomSource.NextBytes(valueBytes);
    values[i] = BitConverter.ToUInt32(valueBytes);
}

byte[] keyBytes;
randomSource.NextBytes(keyBytes = new byte[randomSource.Next(128, 513)]);

var shacal1 = new SHACAL1Cipher(keyBytes);

var encryptedValues = shacal1.Encrypt(values);
var decryptedValues = shacal1.Decrypt(encryptedValues);

Console.Write("Initial:");
foreach (var value in values)
{
    Console.Write($" {value}");
}
Console.WriteLine();

Console.Write("Encrypted:");
foreach (var value in encryptedValues)
{
    Console.Write($" {value}");
}
Console.WriteLine();

Console.Write("Decrypted:");
foreach (var value in decryptedValues)
{
    Console.Write($" {value}");
}
Console.WriteLine();