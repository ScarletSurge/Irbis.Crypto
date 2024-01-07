namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IAsymmetricCipherAlgorithm<TInput, TOutput>:
    ICipherAlgorithm<TInput, TOutput>
{
    
}