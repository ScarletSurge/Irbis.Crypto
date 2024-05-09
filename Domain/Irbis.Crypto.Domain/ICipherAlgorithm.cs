namespace Irbis.Crypto.Domain;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TCiphertext"></typeparam>
public interface ICipherAlgorithm<TData, TCiphertext>
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    void Encrypt(
        TData input,
        ref TCiphertext output);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    void Decrypt(
        TCiphertext input,
        ref TData output);

}