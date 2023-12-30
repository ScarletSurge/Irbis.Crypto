using System.Collections;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;

namespace Crypto.Cipher.NTRUEncrypt;

/// <summary>
/// 
/// </summary>
public sealed class NTRUPolynomial : IEquatable<NTRUPolynomial>, IEnumerable<KeyValuePair<BigInteger, BigInteger>>
{

    /// <summary>
    /// 
    /// </summary>
    private ImmutableSortedDictionary<BigInteger, BigInteger> _degreesToCoefficients;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="degreesToCoefficients"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public NTRUPolynomial(
        IDictionary<BigInteger, BigInteger> degreesToCoefficients)
    {
        _degreesToCoefficients = (degreesToCoefficients ?? throw new ArgumentNullException(nameof(degreesToCoefficients)))
            .ToImmutableSortedDictionary(
                degreeToCoefficients => degreeToCoefficients.Key,
                degreeToCoefficients => degreeToCoefficients.Value);

        PutInOrder();
    }
    
    #region Helper methods
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private NTRUPolynomial PutInOrder()
    {
        _degreesToCoefficients = _degreesToCoefficients
            .Where(degreeToCoefficient => degreeToCoefficient.Value != BigInteger.Zero)
            .ToImmutableSortedDictionary(degreeToCoefficient => degreeToCoefficient.Key,
                degreeToCoefficient => degreeToCoefficient.Value);
        
        return this;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="coefficientsModulo"></param>
    /// <returns></returns>
    private NTRUPolynomial CoefficientsByModulo(
        BigInteger coefficientsModulo)
    {
        _degreesToCoefficients = _degreesToCoefficients
            .ToImmutableSortedDictionary(degreeToCoefficient => degreeToCoefficient.Key,
                degreeToCoefficient => degreeToCoefficient.Value % coefficientsModulo);

        return this;
    }
    
    #endregion
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstSummand"></param>
    /// <param name="secondSummand"></param>
    /// <param name="ringModuloMaxDegree"></param>
    /// <returns></returns>
    public static NTRUPolynomial Sum(
        NTRUPolynomial firstSummand,
        NTRUPolynomial secondSummand,
        BigInteger ringModuloMaxDegree)
    {
        if (ringModuloMaxDegree < 1)
        {
            throw new ArgumentException("Ring modulo's max degree must be gt 0.", nameof(ringModuloMaxDegree));
        }

        return new NTRUPolynomial(new Dictionary<BigInteger, BigInteger>(firstSummand._degreesToCoefficients
            .Concat(secondSummand._degreesToCoefficients)
            .GroupBy(degreeToCoefficient => degreeToCoefficient.Key % ringModuloMaxDegree)
            .ToImmutableSortedDictionary(degreeToCoefficients => degreeToCoefficients.Key,
                degreeToCoefficients => degreeToCoefficients
                    .Aggregate(BigInteger.Zero,
                        (accumulator, degreeToCoefficient) => accumulator + degreeToCoefficient.Value)))
        );
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="toMultiply"></param>
    /// <param name="monomialCoefficient"></param>
    /// <param name="monomialDegree"></param>
    /// <returns></returns>
    private static NTRUPolynomial MultiplicationByMonomial(
        NTRUPolynomial toMultiply,
        BigInteger monomialCoefficient,
        BigInteger monomialDegree)
    {
        return new NTRUPolynomial(toMultiply._degreesToCoefficients
            .ToDictionary(degreeToCoefficient => degreeToCoefficient.Key + monomialDegree,
                degreeToCoefficient => degreeToCoefficient.Value * monomialCoefficient));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="ringModuloMaxDegree"></param>
    /// <param name="coefficientsModulo"></param>
    /// <returns></returns>
    public static NTRUPolynomial Multiplication(
        NTRUPolynomial first,
        NTRUPolynomial second,
        BigInteger ringModuloMaxDegree,
        BigInteger coefficientsModulo)
    {
        var accumulator = new NTRUPolynomial(new Dictionary<BigInteger, BigInteger>());

        foreach (var secondDegreeToCoefficient in second)
        {
            accumulator = Sum(accumulator, 
                MultiplicationByMonomial(first, secondDegreeToCoefficient.Value, secondDegreeToCoefficient.Key), ringModuloMaxDegree);
        }

        return accumulator
            .CoefficientsByModulo(coefficientsModulo)
            .PutInOrder();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    private static NTRUPolynomial ExtendedEuclideanAlgorithm(
        NTRUPolynomial first,
        NTRUPolynomial second)
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toInvert"></param>
    /// <param name="ringModuloMaxDegree"></param>
    /// <param name="coefficientsModulo"></param>
    /// <returns></returns>
    public static NTRUPolynomial Inverted(
        NTRUPolynomial toInvert, 
        BigInteger ringModuloMaxDegree, 
        BigInteger coefficientsModulo)
    {
        
    }
    
    #region System.Object overrides

    public override int GetHashCode()
    {
        var hashCode = 0;

        foreach (var (degree, coefficient) in _degreesToCoefficients)
        {
            hashCode = hashCode * 23 + degree.GetHashCode() * 17 + coefficient.GetHashCode();
        }
        
        return hashCode;
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            null => false,
            NTRUPolynomial polynomial => Equals(polynomial),
            _ => false
        };
    }

    public override string ToString()
    {
        var builder = new StringBuilder("[ ");
        
        foreach (var degreeToCoefficient in _degreesToCoefficients/*.Skip(1)*/)
        {
            var sign = degreeToCoefficient.Value < 0
                ? '-'
                : '+';
            var absCoefficient = BigInteger.Abs(degreeToCoefficient.Value);
            var variableString = degreeToCoefficient.Key == BigInteger.Zero
                ? string.Empty
                : "x";
            var circumflexString = degreeToCoefficient.Key > BigInteger.One
                ? "^"
                : string.Empty;
            var degreeString = degreeToCoefficient.Key > BigInteger.One
                ? degreeToCoefficient.Key.ToString()
                : string.Empty;

            builder.Append($"{sign} {absCoefficient}{variableString}{circumflexString}{degreeString} ");
        }

        return builder
            .Append(']')
            .ToString();
    }

    #endregion
    
    #region System.IEquatable<NTRUPolynomial> implementation
    
    /// <inheritdoc cref="IEquatable{T}.Equals(T?)" />
    public bool Equals(NTRUPolynomial? other)
    {
        return other is not null && _degreesToCoefficients.Equals(other._degreesToCoefficients);
    }
    
    #endregion
    
    #region System.Collections.Generic.IEnumerable<out T> implementation
    
    public IEnumerator<KeyValuePair<BigInteger, BigInteger>> GetEnumerator()
    {
        return _degreesToCoefficients.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    #endregion
    
}