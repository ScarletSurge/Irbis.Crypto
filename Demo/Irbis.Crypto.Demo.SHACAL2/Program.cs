using Irbis.Crypto.Domain;

var randomSource = new Random(782349);

var keyBytes = new byte[16];
randomSource.NextBytes(keyBytes);
var ivBytes = new byte[16];
randomSource.NextBytes(ivBytes);
var ivBytesCopied = (byte[])ivBytes.Clone();
var dataBytes = new byte[64];
randomSource.NextBytes(dataBytes);

var algorithm = new CipherSample(keyBytes);
await CryptoTransformationContext.PerformCipherAsync(algorithm, 4, CipherMode.CounterMode, PaddingMode.ISO10126, CipherTransformationMode.Encryption, ivBytes, "in.jpg", "out.jpg").ConfigureAwait(false);
await CryptoTransformationContext.PerformCipherAsync(algorithm, 4, CipherMode.CounterMode, PaddingMode.ISO10126, CipherTransformationMode.Decryption, ivBytesCopied, "out.jpg", "in1.jpg").ConfigureAwait(false);