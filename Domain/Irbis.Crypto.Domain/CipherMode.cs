namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
public enum CipherMode
{
    /// <summary>
    /// 
    /// </summary>
    ElectronicCodebook,
    /// <summary>
    /// 
    /// </summary>
    CipherBlockChaining,
    /// <summary>
    /// 
    /// </summary>
    PropagatingCipherBlockChaining,
    /// <summary>
    /// 
    /// </summary>
    CipherFeedback,
    /// <summary>
    /// 
    /// </summary>
    OutputFeedback,
    /// <summary>
    /// 
    /// </summary>
    CounterMode,
    /// <summary>
    /// 
    /// </summary>
    RandomDelta
}