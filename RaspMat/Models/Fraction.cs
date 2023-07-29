using RaspMat.Properties;
using System;
using System.Linq;
using System.Numerics;

namespace RaspMat.Models
{
    /// <summary>
    /// A readonly struct containing 2 <see cref="BigInteger"/>s. Do not use the parameterless (default) struct constructor.
    /// </summary>
    internal readonly struct Fraction
    {

        /// <summary>
        /// Numerator (upper part) of this <see cref="Fraction"/>.
        /// </summary>
        public BigInteger Numerator { get; }

        /// <summary>
        /// Denominator (lower part) of this <see cref="Fraction"/>.
        /// </summary>
        public BigInteger Denominator { get; }

        /// <summary>
        /// Creates a new <see cref="Fraction"/>. If the <paramref name="denominator"/> is negative, it and <paramref name="numerator"/> will be multiplied by -1.
        /// </summary>
        /// <param name="numerator">Numerator (upper part) of the fraction.</param>
        /// <param name="denominator">Denominator (lower part) of the fraction. Cannot be 0.</param>
        /// <exception cref="DivideByZeroException">0 equals <paramref name="denominator"/>.</exception>
        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero)
                throw new DivideByZeroException();

            if (denominator < BigInteger.Zero)
            {
                denominator = BigInteger.Negate(denominator);
                numerator = BigInteger.Negate(numerator);
            }

            if (!denominator.IsOne)
            {
                var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
                numerator /= gcd;
                denominator /= gcd;
            }

            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>
        /// Creates a new <see cref="Fraction"/> which is equal to 0.
        /// </summary>
        public static Fraction Zero => new Fraction(BigInteger.Zero, BigInteger.One);

        /// <summary>
        /// Removes all parentheses and creates a new <see cref="Fraction"/> based on <paramref name="fraction"/>.
        /// </summary>
        /// <param name="fraction">Human-readible string representation of a <see cref="Fraction"/>, like "-1/2".</param>
        /// <returns><see cref="Fraction"/> created from the <paramref name="fraction"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="fraction"/> has more than 2 or no parts when split by '/'.</exception>
        public static Fraction Parse(string fraction)
        {
            var integers = Array.ConvertAll(fraction.Replace(" ", string.Empty).Split('/'), str => BigInteger.Parse(str.TrimStart('(').TrimEnd(')')));

            if (!integers.Any())
                throw new ArgumentException(message: Resources.ERR_NO_INTS, paramName: nameof(fraction));
            if (integers.Length > 2)
                throw new ArgumentException(message: Resources.ERR_TOO_MANY_INTS, paramName: nameof(fraction));

            return integers.Length > 1 ? new Fraction(integers.First(), integers.Last()) : new Fraction(integers.First(), BigInteger.One);
        }

        /// <summary>
        /// Returnes a new <see cref="Fraction"/> such that it is equal to 1 when multiplied by the object it was created from.
        /// </summary>
        /// <returns>New <see cref="Fraction"/> based on the object it was called from with <see cref="Denominator"/> and <see cref="Numerator"/> swapped.</returns>
        public Fraction Reciprocal() => new Fraction(Denominator, Numerator);

        public static Fraction operator +(Fraction a, Fraction b)
        {
            var lcm = BigInteger.Abs(a.Denominator * b.Denominator) / BigInteger.GreatestCommonDivisor(a.Denominator, b.Denominator);
            return new Fraction((a.Numerator * (lcm / a.Denominator)) + (b.Numerator * (lcm / b.Denominator)), lcm);
        }

        public static Fraction operator -(Fraction a, Fraction b)
            => a + new Fraction(-b.Numerator, b.Denominator);

        public static Fraction operator -(Fraction a)
            => new Fraction(BigInteger.Negate(a.Numerator), a.Denominator);

        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Fraction operator /(Fraction a, Fraction b)
            => a * new Fraction(b.Denominator, b.Numerator);

        public static bool operator ==(Fraction a, Fraction b)
            => a.Numerator == b.Numerator && a.Denominator == b.Denominator;

        public static bool operator !=(Fraction a, Fraction b)
            => !(a == b);

        public static implicit operator Fraction(long numerator)
            => new Fraction(numerator, BigInteger.One);

        public override bool Equals(object obj)
            => obj is Fraction fraction && this == fraction;

        public override int GetHashCode()
            => (int)((Numerator % (int.MaxValue / 2)) + (Denominator % (int.MaxValue / 2)));

        public override string ToString()
        {
            if (Denominator.IsOne)
            {
                return Numerator.ToString();
            }

            return string.Join("/", Numerator.ToString(), Denominator.ToString());
        }

    }
}
