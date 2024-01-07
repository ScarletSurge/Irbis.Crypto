namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
public interface ISymmetricCipherAlgorithm:
    ICipherAlgorithm<byte[], byte[]>
{
    
    /// <summary>
    /// 
    /// </summary>
    public int BlockSize
    {
        get;
    }
    
}