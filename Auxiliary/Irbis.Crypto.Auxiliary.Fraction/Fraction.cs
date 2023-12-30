using System.Numerics;

namespace Irbis.Crypto.Auxiliary.Fraction;

/// <summary>
/// 
/// </summary>
public readonly struct Fraction : IEquatable<Fraction>, IComparable, IComparable<Fraction>
{

    #region Constructors
    
    /// <summary>
    /// Creates fraction equals to zero.
    /// </summary>
    public Fraction()
    {
        Full = BigInteger.Zero;
        Numerator = BigInteger.Zero;
        Denominator = BigInteger.One;
    }
    
    /// <summary>
    /// Creates fraction with passed full, numerator and denominator parts.
    /// </summary>
    /// <param name="full"></param>
    /// <param name="numerator"></param>
    /// <param name="denominator"></param>
    private Fraction(BigInteger full, BigInteger numerator, BigInteger denominator)
    {
        if ((Denominator = denominator) == BigInteger.Zero)
        {
            throw new ArgumentException("Fraction's denominator can't be equal to 0.", nameof(denominator));
        }
        Full = full;
        Numerator = numerator;

        Reduce();
    }
    
    #region Factory methods
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="full"></param>
    /// <returns></returns>
    public static Fraction CreateFrom(BigInteger full)
    {
        return new Fraction(full, BigInteger.Zero, BigInteger.One);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="numerator"></param>
    /// <param name="denominator"></param>
    /// <returns></returns>
    public static Fraction CreateFrom(BigInteger numerator, BigInteger denominator)
    {
        return new Fraction(BigInteger.Zero, numerator, denominator);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="full"></param>
    /// <param name="numerator"></param>
    /// <param name="denominator"></param>
    /// <returns></returns>
    public static Fraction CreateFrom(BigInteger full, BigInteger numerator, BigInteger denominator)
    {
        return new Fraction(full, numerator, denominator);
    }
    
    #endregion
    
    #endregion

    #region Properties
    
    /// <summary>
    /// Fraction full part.
    /// </summary>
    public BigInteger Full
    {
        get;
    }
    
    /// <summary>
    /// Fraction numerator.
    /// </summary>
    public BigInteger Numerator
    {
        get;
    }
    
    /// <summary>
    /// Fraction denominator. Stores fraction's sign.
    /// </summary>
    public BigInteger Denominator
    {
        get;
    }
    
    #endregion
    
    #region Methods
    
    #region Auxiliary methods
    
    /// <summary>
    /// 
    /// </summary>
    private void Reduce()
    {
        // TODO
    }
    
    #endregion
    
    #region System.Object overrides
    
    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? obj)
    {
        if (obj is Fraction fraction)
        {
            return Equals(fraction);
        }

        return false;
    }
    
    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
    {
        return Full.GetHashCode() * 113 + Numerator.GetHashCode() * 23 + Denominator.GetHashCode();
    }
    
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        return $"{{{Full} {Numerator}/{Denominator}}}";
    }
    
    #endregion
    
    #region IEquatable<Fraction> implementation
    
    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(Fraction other)
    {
        return Full == other.Full
            && Numerator == other.Numerator
            && Denominator == other.Denominator;
    }
    
    #endregion
    
    #region IComparable implementation
    
    /// <inheritdoc cref="IComparable.CompareTo"/>
    public int CompareTo(object? obj)
    {
        throw new NotImplementedException();
    }
    
    #endregion
    
    #region IComparable<Fraction> implementation
    
    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    public int CompareTo(Fraction other)
    {
        throw new NotImplementedException();
    }
    
    #endregion
    
    #endregion
    
}