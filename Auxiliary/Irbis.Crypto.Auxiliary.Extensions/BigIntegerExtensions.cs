using System.Numerics;

namespace Irbis.Crypto.Auxiliary.Extensions;

/// <summary>
/// 
/// </summary>
public static class BigIntegerExtensions
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static BigInteger RandomInRange1(BigInteger min, BigInteger max)
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }

        // offset to set min = 0
        BigInteger offset = -min;
        min = 0;
        max += offset;

        var value = randomInRangeFromZeroToPositive(max) - offset;
        return value;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    private static BigInteger randomInRangeFromZeroToPositive(BigInteger max)
    {
        BigInteger value;
        var bytes = max.ToByteArray();

        // count how many bits of the most significant byte are 0
        // NOTE: sign bit is always 0 because `max` must always be positive
        byte zeroBitsMask = 0b00000000;

        var mostSignificantByte = bytes[bytes.Length - 1];

        // we try to set to 0 as many bits as there are in the most significant byte, starting from the left (most significant bits first)
        // NOTE: `i` starts from 7 because the sign bit is always 0
        for (var i = 7; i >= 0; i--)
        {
            // we keep iterating until we find the most significant non-0 bit
            if ((mostSignificantByte & (0b1 << i)) != 0)
            {
                var zeroBits = 7 - i;
                zeroBitsMask = (byte)(0b11111111 >> zeroBits);
                break;
            }
        }

        var rndSource = new Random();
        
        do
        {
            rndSource.NextBytes(bytes);

            // set most significant bits to 0 (because `value > max` if any of these bits is 1)
            bytes[bytes.Length - 1] &= zeroBitsMask;

            value = new BigInteger(bytes);

            // `value > max` 50% of the times, in which case the fastest way to keep the distribution uniform is to try again
        } while (value > max);

        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="leftBound">Range's left bound, inclusive.</param>
    /// <param name="rightBound">Range's right bound, exclusive.</param>
    /// <returns></returns>
    public static BigInteger RandomInRange(BigInteger leftBound, BigInteger rightBound)
    {
        return RandomInRange1(leftBound, rightBound);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static int LegendreSymbol(BigInteger a, BigInteger p)
    {
        if (p < 2)
        {
            throw new ArgumentException("p must not be < 2", nameof(p));
        }

        if (a == 0 || a == 1)
        {
            return (int)a;
        }

        BigInteger r;
        if (a % 2 == 0)
        {
            r= LegendreSymbol(a / 2, p);
            
            if (((p * p - 1) & 8) != 0)
            {
                r *= -1;
            }
        }
        else 
        {
            r = LegendreSymbol(p % a, a);
            
            if (((a - 1) * (p - 1) & 4) != 0)
            {
                r *= -1;
            }
        }
        
        return (int)r;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="n"></param>
    public static int JacobiSymbol(BigInteger a, BigInteger n)
    {
        if (BigInteger.GreatestCommonDivisor(a, n) != 1)
        {
            return 0;
        }

        int r = 1;
        if (a < 0)
        {
            a = -a;
            if (n % 4 == 3)
            {
                r = -r;
            }
        }

        do
        {
            int t = 0;
            while (a % 2 == 0)
            {
                t++;
                a = a / 2;
            }
            if (t % 2 == 1)
            {
                var mod = n % 8;
                if (mod == 3 || mod == 5)
                {
                    r = -r;
                }
            }

            var amod4 = a % 4;

            if (amod4 == n % 4 && amod4 == 3)
            {
                r = -r;
            }

            var c = a;
            a = n % c;
            n = c;
        } while (a != 0);

        return r;
    }
    
}