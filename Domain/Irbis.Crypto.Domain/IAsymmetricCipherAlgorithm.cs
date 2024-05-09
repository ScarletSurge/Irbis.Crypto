namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TCiphertext"></typeparam>
public interface IAsymmetricCipherAlgorithm<TData, TCiphertext>:
    ICipherAlgorithm<TData, TCiphertext>
{
    
}