using System.Numerics;

namespace Irbis.Crypto.Auxiliary.PrimalityTests;

/// <summary>
/// 
/// </summary>
public abstract class PrimalityTestBase : IPrimalityTest
{

    /// <inheritdoc cref="IPrimalityTest.CheckPrimality" />
    public bool CheckPrimality(BigInteger value, double epsilon)
    {
        ThrowIfValueIsInvalid(value);
        ThrowIfEpsilonIsInvalid(epsilon);

        if (value == 0 || value == 1)
        {
            return false;
        }

        if (value == 2 || value == 3)
        {
            return true;
        }
        
        var iterationsCount = GetIterationsCount(epsilon);

        for (var i = 0; i < iterationsCount; i++)
        {
            if (!Iteration(value))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract bool Iteration(BigInteger value);
    
    /// <summary>
    /// 
    /// </summary>
    protected abstract double OneIterationComplexityProbability
    {
        get;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ThrowIfValueIsInvalid(BigInteger value)
    {
        if (value > 0)
        {
            return;
        }

        throw new ArgumentException("Value value must be greater than 0", nameof(value));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="epsilon"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ThrowIfEpsilonIsInvalid(double epsilon)
    {
        if (epsilon is > 0 and < 1)
        {
            return;
        }

        throw new ArgumentException("Epsilon value must be in (0, 1)", nameof(epsilon));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    private int GetIterationsCount(double epsilon)
    {
        var iterationsCount = 0;
        
        while (epsilon >= 1 - Math.Pow(OneIterationComplexityProbability, ++iterationsCount))
        {
            
        }

        return iterationsCount;
    }

}