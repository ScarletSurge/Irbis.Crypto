// ReSharper disable IdentifierTypo

using System.Numerics;
using Irbis.Crypto.Auxiliary.Extensions;
using Irbis.Crypto.Auxiliary.PrimalityTests;

namespace Irbis.Crypto.Cipher.Benaloh;

/// <summary>
/// 
/// </summary>
public sealed class Benaloh
{
    
    #region Nested
    
    /// <summary>
    /// 
    /// </summary>
    public struct Keys
    {
        
        // p * q
        public BigInteger n { get; set; }
        
        // public key
        public BigInteger y { get; set; }
        
        public BigInteger r { get; set; }
        
        // private key
        public BigInteger f { get; set; }
        
        public BigInteger x { get; set; }
    
    }
    
    /// <summary>
    /// 
    /// </summary>
    private sealed class KeysGenerator
    {
        
        private readonly IPrimalityTest _primalityTest;
        private readonly double _probability;
        private readonly ulong _numSize;

        public KeysGenerator(PrimalityTest mode, double minProbability, ulong size)
        {
            _primalityTest = mode switch
            {
                PrimalityTest.Fermat => new FermatPrimalityTest(),
                PrimalityTest.SolovayStrassen => new SolovayStrassenPrimalityTest(),
                PrimalityTest.MillerRabin => new MillerRabinPrimalityTest(),
                _ => throw new ArgumentException("Invalid test.", nameof(mode))
            };

            _probability = minProbability;
            _numSize = size;
        }

        public Keys Generate(BigInteger r)
        {
            BigInteger p, q;

            // p generation
            p = GetPrimeNumberP(r);

            // q generation
            q = GetPrimeNumberQ(r, p);

            var n = p * q;
            var phi = (p - 1) * (q - 1);
            BigInteger y;

            do
            {
                y = BigIntegerExtensions.RandomInRange(1, n - 1);
            } while (BigInteger.ModPow(y, phi / r, n) == 1);

            var x = BigInteger.ModPow(y, phi / r, n);

            return new Keys
            {
                x = x,
                y = y,
                n = n,
                f = phi,
                r = r
            };
        }

        public BigInteger GetPrimeNumberP(BigInteger r)
        {
            var random = new Random();
            var buffer =
                new byte[_numSize / 8 /*размер на входе лучше указывать в битиках*/ +
                         1 /*чтобы выпилить знаковый бит*/];

            random.NextBytes(buffer);
            buffer[^1] = 0b00000000; // делаем число положительным
            var pCandidate = new BigInteger(buffer);

            // p >= r, так как должно выполняться условие "r делит (p - 1)"
            if (pCandidate <= r)
            {
                pCandidate += r;
            }

            // подгоним p под условие "r делит (p - 1)", чтобы p при этом было нечётным:
            var modulo = (pCandidate - 1) % r;
            pCandidate -= modulo;
            if (pCandidate < r)
            {
                pCandidate += r;
            }

            if (pCandidate.IsEven)
            {
                pCandidate += r;
            }

            // проверим второе условие и продолжим подгонять p для его выполнения:
            while (BigInteger.GreatestCommonDivisor(r, (pCandidate - 1) / r) != 1)
            {
                pCandidate += 2 * r; // <2 *> для того чтобы p осталось нечётным
            }

            // оба условия выполнены
            // теперь проверяем p на простоту
            while (!_primalityTest.CheckPrimality(pCandidate, _probability))
            {
                // если вдруг p не простое:

                // меняем p с сохранением первого условия
                pCandidate += 2 * r; // <2 *> для того чтобы p осталось нечётным

                // снова добьёмся выполнения второго условия:
                while (BigInteger.GreatestCommonDivisor(r, (pCandidate - 1) / r) != 1)
                {
                    pCandidate += 2 * r; // <2 *> для того чтобы p осталось нечётным
                }

                // снова проверяем p на простоту
            }

            // ура
            return pCandidate;
        }

        public BigInteger GetPrimeNumberQ(BigInteger r, BigInteger p)
        {
            var random = new Random();
            var buffer =
                new byte[_numSize / 8 /*размер на входе лучше указывать в битиках*/ +
                         1 /*чтобы выпилить знаковый бит*/];

            var qCandidate = default(BigInteger);
            // генерируем псевдослучайное q, не равное p (условие 1)
            do
            {
                random.NextBytes(buffer);
                buffer[^1] = 0b00000000; // делаем число положительным
                qCandidate = new BigInteger(buffer);
                if (qCandidate.IsEven)
                {
                    qCandidate++; // и нечётным
                }
            } while (qCandidate == p);

            // проверяем q на соответствие второму условию, если нужно, подгоняем:
            while (BigInteger.GreatestCommonDivisor(r, qCandidate - 1) != 1)
            {
                qCandidate += 2;

                if (qCandidate == p)
                {
                    qCandidate += 2;
                }
            }

            // оба условия выполнены
            // проверяем q на простоту
            while (!_primalityTest.CheckPrimality(qCandidate, _probability))
            {
                // если вдруг q не простое:

                // меняем q с сохранением первого условия
                qCandidate += 2;
                if (qCandidate == p)
                {
                    qCandidate += 2;
                }

                // и с сохранением второго условия
                while (BigInteger.GreatestCommonDivisor(r, qCandidate - 1) != 1)
                {
                    qCandidate += 2;

                    if (qCandidate == p)
                    {
                        qCandidate += 2;
                    }
                }

                // снова проверяем q на простоту
            }

            // ура
            return qCandidate;
        }
        
    }
    
    #endregion
    
    private Keys keys;
    private BigInteger u, a;

    public Benaloh(PrimalityTest test, double minProbability, ulong size, BigInteger r)
    {
        keys = new KeysGenerator(test, minProbability, size)
            .Generate(r);
    }

    public BigInteger Encrypt(BigInteger message) //(y^m mod n) * (u^r mod n) mod n
    {
        while (true)
        {
            u = BigIntegerExtensions.RandomInRange(2, keys.n - 1);
            // TODO: ?!
            if (BigInteger.GreatestCommonDivisor(u, keys.n) == 1)
                break;
        }

        var left = BigInteger.ModPow(keys.y, message, keys.n);
        var right = BigInteger.ModPow(u, keys.r, keys.n);

        return BigInteger.Multiply(left, right) % keys.n;
    }

    public BigInteger Decrypt(BigInteger message)
    {
        a = BigInteger.ModPow(message, keys.f / keys.r, keys.n);

        var i = BigInteger.Zero;
        for (; i < keys.r; i++)
        {
            if (BigInteger.ModPow(keys.x, i, keys.n) == a)
            {
                return i;
            }
        }

        return BigInteger.MinusOne;
    }

}