using System.Numerics;

using Crypto.Auxiliary.Extensions;

namespace Crypto.Auxiliary.PrimalityTests;

/// <summary>
/// 
/// </summary>
public sealed class MillerRabinPrimalityTest : PrimalityTestBase
{
    
    /// <inheritdoc cref="PrimalityTestBase.OneIterationComplexityProbability"/>
    protected override double OneIterationComplexityProbability =>
        0.25;
    
    /// <inheritdoc cref="PrimalityTestBase.Iteration" />
    protected override bool Iteration(BigInteger value)
    {
        // TODO: check if such value 'a' was already used (generate & check on upper level)
        var a = BigIntegerExtensions.RandomInRange(2, value);
        
        var s = BigInteger.Zero;
        var d = value - 1;
        
        while (d.IsEven)
        {
            d /= 2;
            s++;
        }
        
        if (BigInteger.ModPow(a, d, value) == 1)
        {
            return true;
        }
        
        for (var r = 0; r < s; r++)
        {
            // TODO: optimise
            if (BigInteger.ModPow(a, d, value) == value - 1)
            {
                return true;
            }
            
            d *= 2;
        }
        
        return false;
    }
    
    public override string ToString()
    {
        return "Miller-Rabin";
    }
    
}