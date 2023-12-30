using System.Numerics;

using Crypto.Auxiliary.Extensions;

namespace Crypto.Auxiliary.PrimalityTests;

/// <summary>
/// 
/// </summary>
public sealed class FermatPrimalityTest : PrimalityTestBase
{
    
    /// <inheritdoc cref="PrimalityTestBase.OneIterationComplexityProbability"/>
    protected override double OneIterationComplexityProbability =>
        0.5;

    /// <inheritdoc cref="PrimalityTestBase.Iteration" />
    protected override bool Iteration(BigInteger value)
    {
        // TODO: check if such value 'a' was already used (generate & check on upper level)
        var a = BigIntegerExtensions.RandomInRange(2, value);

        return BigInteger.ModPow(a, value - 1, value) == 1;
    }
    
    public override string ToString()
    {
        return "Fermat";
    }
    
}