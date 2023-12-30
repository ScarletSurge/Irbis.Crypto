using System.Numerics;

namespace Irbis.Crypto.Auxiliary.PrimalityTests;

/// <summary>
/// 
/// </summary>
public interface IPrimalityTest
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    bool CheckPrimality(BigInteger value, double epsilon);

}