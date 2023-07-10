using Newtonsoft.Json;
using RaspMat.Helpers;
using RaspMat.Properties;
using System;
using System.Numerics;

namespace RaspMat.Models
{
    internal readonly struct Fraction
    {

        /// <summary>
        /// Numerator (upper part) of this fraction.
        /// </summary>
        public BigInteger Numerator { get; }

        /// <summary>
        /// Denominator (lower part) of this fraction.
        /// </summary>
        public BigInteger Denominator { get; }

        /// <summary>
        /// Creates a new <see cref="Fraction"/>. If the <paramref name="denominator"/> is negative, it and <paramref name="numerator"/> will be multiplied by -1.
        /// </summary>
        /// <param name="numerator">Numerator (upper part) of the fraction.</param>
        /// <param name="denominator">Denominator (lower part) of the fraction. Cannot be 0.</param>
        /// <exception cref="DivideByZeroException">0 equals <paramref name="denominator"/>.</exception>
        [JsonConstructor]
        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException();
            else if (denominator < 0)
            {
                denominator *= -1;
                numerator *= -1;
            }

            if (denominator != 1)
            {
                var gcd = Algorithms.GreatestCommonDivisor(numerator, denominator);
                numerator /= gcd;
                denominator /= gcd;
            }

            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>
        /// Creates a new <see cref="Fraction"/> which is equal to 0.
        /// </summary>
        public static Fraction Zero => new Fraction(0, 1);

        /// <summary>
        /// Removes all parentheses and creates a new <see cref="Fraction"/> based on <paramref name="fraction"/>.
        /// </summary>
        /// <param name="fraction">Human-readible string representation of a <see cref="Fraction"/>, like "-1/2".</param>
        /// <returns><see cref="Fraction"/> created from the <paramref name="fraction"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="fraction"/> has more than 2 or no parts when split by '/'.</exception>
        public static Fraction Parse(string fraction)
        {
            var integers = Array.ConvertAll(fraction.Replace(" ", string.Empty).Split('/'), str => BigInteger.Parse(str.TrimStart('(').TrimEnd(')')));

            if (integers.Length < 1)
                throw new ArgumentException(message: Resources.ERR_NO_INTS, paramName: nameof(fraction));
            if (integers.Length > 2)
                throw new ArgumentException(message: Resources.ERR_TOO_MANY_INTS, paramName: nameof(fraction));

            return integers.Length > 1 ? new Fraction(integers[0], integers[1]) : new Fraction(integers[0], 1);
        }

        /// <summary>
        /// Returnes a new <see cref="Fraction"/> such that it is equal to 1 when multiplied by the object it was created from.
        /// </summary>
        /// <returns>New <see cref="Fraction"/> based on the object it was called from with <see cref="Denominator"/> and <see cref="Numerator"/> swapped.</returns>
        public Fraction Reciprocal() => new Fraction(Denominator, Numerator);

        public static Fraction operator +(Fraction a, Fraction b)
        {
            var lcm = Algorithms.LeastCommonMultiple(a.Denominator, b.Denominator);
            return new Fraction((a.Numerator * (lcm / a.Denominator)) + (b.Numerator * (lcm / b.Denominator)), lcm);
        }

        public static Fraction operator -(Fraction a, Fraction b)
            => a + new Fraction(-b.Numerator, b.Denominator);

        public static Fraction operator -(Fraction a)
            => a * -1;

        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Fraction operator /(Fraction a, Fraction b)
            => a * new Fraction(b.Denominator, b.Numerator);

        public static bool operator ==(Fraction a, Fraction b)
            => a.Numerator == b.Numerator && a.Denominator == b.Denominator;

        public static bool operator !=(Fraction a, Fraction b)
            => !(a == b);

        public static implicit operator Fraction(int numerator)
            => new Fraction(numerator, 1);

        /// <summary>
        /// Same as <see cref="operator ==(Fraction, Fraction)"/>, but also checks if <paramref name="obj"/> is a <see cref="Fraction"/>.
        /// </summary>
        /// <param name="obj">An <see cref="object"/> to be checked if it is a <see cref="Fraction"/> and equal to the one this method is executed from.</param>
        /// <returns>A <see cref="bool"/> value indicating whether both <see cref="object"/>s are equal <see cref="Fraction"/>s.</returns>
        public override bool Equals(object obj)
            => obj is Fraction fraction && this == fraction;

        public override int GetHashCode()
            => (int)((Numerator % (int.MaxValue / 2)) + (Denominator % (int.MaxValue / 2)));

        /// <summary>
        /// Gets a human-readible representation of a <see cref="Fraction"/>, like "-1/2".
        /// </summary>
        /// <returns><see cref="Fraction"/> converted to <see cref="string"/>.</returns>
        public override string ToString()
        {
            var ret = Numerator.ToString();
            if (Denominator != 1)
                ret += "/" + Denominator;
            return ret;
        }

    }
}
