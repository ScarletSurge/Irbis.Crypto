using Irbis.Crypto.Domain;

var randomSource = new Random(782349);

var keyBytes = new byte[16];
randomSource.NextBytes(keyBytes);
var ivBytes = new byte[16];
randomSource.NextBytes(ivBytes);
var dataBytes = new byte[64];
randomSource.NextBytes(dataBytes);

var algorithm = new CipherSample(keyBytes);
CryptoTransformationContext.PerformCipher(algorithm, 4, CipherMode.OutputFeedback, PaddingMode.ISO10126, dataBytes, CipherTransformationMode.Encryption, ivBytes);
CryptoTransformationContext.PerformCipher(algorithm, 4, CipherMode.OutputFeedback, PaddingMode.ISO10126, dataBytes, CipherTransformationMode.Decryption, ivBytes);
var x = 10;