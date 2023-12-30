using System.Collections.ObjectModel;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace Crypto.Cipher.SHACAL1;

/// <summary>
/// 
/// </summary>
public sealed class SHACAL1Cipher /* : <contracts list> */
{
    
    /// <summary>
    /// 
    /// </summary>
    private const int RoundsCount = 80;
    
    #region Fields
    
    #region Static fields
    
    /// <summary>
    /// 
    /// </summary>
    private static readonly ReadOnlyCollection<Func<uint, uint, uint, uint>> EncryptionRoundFunctions;
    
    /// <summary>
    /// 
    /// </summary>
    private static readonly ReadOnlyCollection<Func<uint, uint, uint, uint>> DecryptionRoundFunctions;
    
    /// <summary>
    /// 
    /// </summary>
    private static readonly ReadOnlyCollection<uint> EncryptionConstants;
    
    /// <summary>
    /// 
    /// </summary>
    private static readonly ReadOnlyCollection<uint> DecryptionConstants;
    
    #endregion
    
    /// <summary>
    /// 
    /// </summary>
    private readonly ReadOnlyCollection<uint> _roundKeys;
    
    #endregion
    
    #region Constructors
    
    /// <summary>
    /// 
    /// </summary>
    static SHACAL1Cipher()
    {
        EncryptionRoundFunctions = Array.AsReadOnly(new Func<uint, uint, uint, uint>[]
        {
            (x, y, z) => (x & y) | (~x & z),
            (x, y, z) => x ^ y ^ z,
            (x, y, z) => (x & y) | (x & z) | (y & z),
            (x, y, z) => x ^ y ^ z
        });
        
        DecryptionRoundFunctions = Array.AsReadOnly(new Func<uint, uint, uint, uint>[]
        {
            (x, y, z) => x ^ y ^ z,
            (x, y, z) => (x & y) | (x & z) | (y & z),
            (x, y, z) => x ^ y ^ z,
            (x, y, z) => (x & y) | (~x & z)
        });

        EncryptionConstants = Array.AsReadOnly(new []
        {
            Convert.ToUInt32("5A827999", 16),
            Convert.ToUInt32("6ED9EBA1", 16),
            Convert.ToUInt32("8F1BBCDC", 16),
            Convert.ToUInt32("CA62C1D6", 16)
        });
        
        DecryptionConstants = Array.AsReadOnly(new []
        {
            Convert.ToUInt32("CA62C1D6", 16),
            Convert.ToUInt32("8F1BBCDC", 16),
            Convert.ToUInt32("6ED9EBA1", 16),
            Convert.ToUInt32("5A827999", 16)
        });
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="ArgumentException"></exception>
    public SHACAL1Cipher(byte[] key)
    {
        if (key.Length is < 128 or > 512)
        {
            throw new ArgumentException("Invalid key", nameof(key));
        }
        
        Array.Resize(ref key, 512);
        
        var roundKeys = Enumerable
            .Range(0, RoundsCount)
            .Select(i => BitConverter.ToUInt32(key, i * 4))
            .ToArray();
        
        for (var i = 16; i < RoundsCount; i++)
        {
            roundKeys[i] = roundKeys[i - 3] ^ roundKeys[i - 8] ^
                             roundKeys[i - 14] ^ roundKeys[i - 16];
            
            roundKeys[i] = (roundKeys[i] << 1) | (roundKeys[i] >> (sizeof(uint) * 8 - 1));
        }

        _roundKeys = Array.AsReadOnly(roundKeys);
    }
    
    #endregion
    
    #region Methods
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="abcde"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public uint[] Encrypt(uint[] abcde)
    {
        // TODO: byte arrays
        if (abcde is null)
        {
            throw new ArgumentNullException(nameof(abcde));
        }

        if (abcde.Length != 5)
        {
            throw new ArgumentException("Block size is not equal to 160 bits.", nameof(abcde));
        }
        
        var a = abcde[0];
        var b = abcde[1];
        var c = abcde[2];
        var d = abcde[3];
        var e = abcde[4];

        for (var round = 0; round < RoundsCount; round++)
        {
            var newA = _roundKeys[round] +
                ((a << 5) | (a >> (sizeof(uint) * 8 - 5))) +
                EncryptionRoundFunctions[round / 20](b, c, d) +
                e +
                EncryptionConstants[round / 20];
            var newB = a;
            var newC = (b << 30) | (b >> (sizeof(uint) * 8 - 30));
            var newD = c;
            var newE = d;

            a = newA;
            b = newB;
            c = newC;
            d = newD;
            e = newE;
        }
        
        return new [] { a, b, c, d, e };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="abcde"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public uint[] Decrypt(uint[] abcde)
    {
        // TODO: byte arrays
        if (abcde is null)
        {
            throw new ArgumentNullException(nameof(abcde));
        }

        if (abcde.Length != 5)
        {
            throw new ArgumentException("Block size is not equal to 160 bits.", nameof(abcde));
        }
        
        var a = abcde[0];
        var b = abcde[1];
        var c = abcde[2];
        var d = abcde[3];
        var e = abcde[4];

        for (var round = 0; round < RoundsCount; round++)
        {
            var newA = b;
            var newB = (c >> 30) | (c << (sizeof(uint) * 8 - 30));
            var newC = d;
            var newD = e;
            var newE = a -
                ((b << 5) | (b >> (sizeof(uint) * 8 - 5))) -
                DecryptionRoundFunctions[round / 20](newB, d, e) -
                DecryptionConstants[round / 20] -
                _roundKeys[RoundsCount - 1 - round];

            a = newA;
            b = newB;
            c = newC;
            d = newD;
            e = newE;
        }
        
        return new [] { a, b, c, d, e };
    }
    
    #endregion
    
}