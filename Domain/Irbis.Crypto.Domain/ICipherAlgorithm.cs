namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface ICipherAlgorithm<TInput, TOutput>
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    TOutput Encrypt(TInput input);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="output"></param>
    /// <returns></returns>
    TInput Decrypt(TOutput output);

}