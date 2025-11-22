using RaspMat.Extensions;
using System;
using System.Numerics;

namespace RaspMat.Models
{
    /// <summary>
    /// Representation of 2 <see cref="BigInteger"/>s.
    /// </summary>
    internal readonly struct Fraction
    {

        /// <summary>
        /// Separator used between <see cref="Numerator"/> and <see cref="Denominator"/>.
        /// </summary>
        public const string FRACTION_SEPARATOR = "/";

        /// <summary>
        /// Format used by <see cref="BigInteger.ToString(string)"/>.
        /// </summary>
        private const string INTEGER_TOSTRING_FORMAT = "R"; // Allow more than 50 digits.

        /// <summary>
        /// Numerator (upper part) of the <see cref="Fraction"/>.
        /// </summary>
        public BigInteger Numerator { get; }

        /// <summary>
        /// Denominator (lower part) of the <see cref="Fraction"/>. Will never be zero.
        /// </summary>
        public BigInteger Denominator => _denominator.IsZero ? BigInteger.One : _denominator; // 0 is possible if default (parameterless) constructor was used.

        /// <summary>
        /// Field for <see cref="Denominator"/>.
        /// </summary>
        private readonly BigInteger _denominator;

        /// <summary>
        /// Indicates (<see langword="true"/>) a one.
        /// </summary>
        public bool IsOne => Numerator.IsOne && Denominator.IsOne;

        /// <summary>
        /// Indicates (<see langword="true"/>) a zero.
        /// </summary>
        public bool IsZero => Numerator.IsZero;

        /// <summary>
        /// Creates a new <see cref="Fraction"/>. If the <paramref name="denominator"/> is negative, it and <paramref name="numerator"/> will be multiplied by -1.
        /// </summary>
        /// <param name="numerator">Numerator (upper part) of the fraction.</param>
        /// <param name="denominator">Denominator (lower part) of the fraction. Cannot be 0.</param>
        /// <exception cref="DivideByZeroException">Zero <paramref name="denominator"/>.</exception>
        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero) throw new DivideByZeroException(nameof(denominator));

            if (denominator.Sign < 0)
            {
                denominator = -denominator;
                numerator = -numerator;
            }

            var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;

            Numerator = numerator;
            _denominator = denominator;
        }

        /// <summary>
        /// Trims all whitespace characters, removes parentheses and creates a new <see cref="Fraction"/> based on <paramref name="input"/>.
        /// </summary>
        /// <param name="input">Human-readible string representation of a <see cref="Fraction"/>, like "-1/2".</param>
        /// <returns><see cref="Fraction"/> created from the <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="input"/> has more than 2 or no parts when split by <see cref="FRACTION_SEPARATOR"/>.</exception>
        public static Fraction Parse(string input)
        {
            var integers = Array.ConvertAll(input.RemoveWhitespaces().Split(new[] {
                FRACTION_SEPARATOR,
            }, StringSplitOptions.RemoveEmptyEntries), split => BigInteger.Parse(split.TrimStart('(').TrimEnd(')')));

            switch (integers.Length)
            {
                case 1:
                    return new Fraction(integers[0], BigInteger.One);
                case 2:
                    return new Fraction(integers[0], integers[1]);
                default:
                    throw new ArgumentException(nameof(input));
            }
        }

        /// <summary>
        /// Parses <paramref name="numerator"/> and <paramref name="denominator"/> using <see cref="BigInteger.Parse(string)"/> and executes constructor of <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="numerator">Numerator (upper part) to be parsed.</param>
        /// <param name="denominator">Denominator (lower part) to be parsed. If <see cref="string.IsNullOrWhiteSpace(string)"/>, it will be parsed as 1.</param>
        /// <returns>New <see cref="Fraction"/> based on the input.</returns>
        public static Fraction Parse(string numerator, string denominator = null)
        {
            return new Fraction(BigInteger.Parse(numerator), string.IsNullOrWhiteSpace(denominator) ? BigInteger.One : BigInteger.Parse(denominator));
        }

        /// <summary>
        /// Returnes a reciprocal of the <see cref="Fraction"/>.
        /// </summary>
        /// <returns>A <see langword="new"/> <see cref="Fraction"/>.</returns>
        public Fraction Reciprocal() => new Fraction(Denominator, Numerator);

        public static Fraction operator +(Fraction a, Fraction b)
        {
            var lcm = BigInteger.Abs(a.Denominator * b.Denominator / BigInteger.GreatestCommonDivisor(a.Denominator, b.Denominator));
            return new Fraction((a.Numerator * (lcm / a.Denominator)) + (b.Numerator * (lcm / b.Denominator)), lcm);
        }

        public static Fraction operator -(Fraction a)
            => new Fraction(-a.Numerator, a.Denominator);

        public static Fraction operator -(Fraction a, Fraction b)
            => a + (-b);

        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Fraction operator /(Fraction a, Fraction b)
            => a * b.Reciprocal();

        public static bool operator ==(Fraction left, Fraction right)
            => left.Numerator == right.Numerator && left.Denominator == right.Denominator;

        public static bool operator !=(Fraction a, Fraction b)
            => !(a == b);

        public static bool operator ==(Fraction fraction, BigInteger integer)
            => fraction.Numerator == integer && fraction.Denominator == 1;

        public static bool operator !=(Fraction fraction, BigInteger integer)
            => !(fraction == integer);

        public static implicit operator Fraction(BigInteger numerator)
            => new Fraction(numerator, BigInteger.One);

        public static implicit operator Fraction(long numerator)
            => new BigInteger(numerator);

        /// <inheritdoc/>
        public override bool Equals(object comapred)
            => comapred is Fraction fraction && this == fraction;

        /// <inheritdoc/>
        public override int GetHashCode()
            => BigInteger.Add(BigInteger.Pow(Numerator, 2), Denominator).GetHashCode();

        /// <summary>
        /// Returns a <see cref="string"/> representation of <see langword="this"/> <see cref="Fraction"/>, like "-1/2" or "1".
        /// </summary>
        /// <returns>A <see langword="new"/> <see cref="string"/>.</returns>
        public override string ToString()
        {
            var numerator = Numerator.ToString(INTEGER_TOSTRING_FORMAT);
            return Denominator.IsOne ? numerator : string.Join(FRACTION_SEPARATOR, numerator, Denominator.ToString(INTEGER_TOSTRING_FORMAT));
        }

    }
}
