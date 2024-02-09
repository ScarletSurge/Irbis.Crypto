namespace Irbis.Crypto.Domain;

public sealed class CipherSample:
    ISymmetricCipherAlgorithm
{

    private readonly byte[] _key;

    public CipherSample(
        byte[] key)
    {
        _ = key ?? throw new ArgumentNullException(nameof(key));
        if (key.Length != BlockSize)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        _key = key;
    }
    
    public void Encrypt(
        byte[] input,
        ref byte[] output)
    {
        _ = input ?? throw new ArgumentNullException(nameof(input));
        if (input.Length != BlockSize)
        {
            throw new ArgumentOutOfRangeException(nameof(input));
        }
        
        if (output is null || output.Length != BlockSize)
        {
            output = new byte[BlockSize];
        }

        for (var i = 0; i < BlockSize; i++)
        {
            output[i] = input[i];
            output[i] ^= _key[i];
        }
    }

    public void Decrypt(
        byte[] input,
        ref byte[] output)
    {
        _ = input ?? throw new ArgumentNullException(nameof(input));
        if (input.Length != BlockSize)
        {
            throw new ArgumentOutOfRangeException(nameof(input));
        }
        
        if (output is null || output.Length != BlockSize)
        {
            output = new byte[BlockSize];
        }

        for (var i = 0; i < BlockSize; i++)
        {
            output[i] = input[i];
            output[i] ^= _key[i];
        }
    }

    public int BlockSize =>
        16;
    
}