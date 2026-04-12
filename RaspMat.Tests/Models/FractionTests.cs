using NUnit.Framework;
using RaspMat.Models;
using System;

namespace RaspMat.Tests.Models
{
    /// <summary>
    /// Tests for <see cref="Fraction"/>s.
    /// </summary>
    public class FractionTests
    {

        [Test]
        public void ZeroEqualsZero()
        {
            Assert.That((Fraction)0 == 0L);
            Assert.That(new Fraction(0, 1), Is.EqualTo((Fraction)0));
            Assert.That(Fraction.Parse("0"), Is.Not.EqualTo((Fraction)1));
            Assert.That(default(Fraction), Is.EqualTo(-new Fraction(0, 10)));
        }

        [Test]
        public void TwoHalvesEqualOne()
        {
            var first = new Fraction(1, 2);
            var second = Fraction.Parse("1/2");
            var result = first + second;
            Assert.That(result == 1);
            Assert.That(result != 2);
            Assert.That(result != 0);
            Assert.That(result != -1);
            Assert.That(result != -new Fraction(1, 1));
        }

        [Test]
        public void Reciprocal()
        {
            var half = new Fraction(1, 2);
            Assert.That(half.Reciprocal() == 2);
            Assert.That(new Fraction(100, 100), Is.EqualTo(new Fraction(2, 2).Reciprocal()));
            Assert.That(half != 2);
        }

        [Test]
        public void Multiplication()
        {
            Assert.That(new Fraction(1, 2) * 2 == 1);
            Assert.That(new Fraction(1, 2) * new Fraction(1, 2), Is.EqualTo(new Fraction(1, 4)));
            Assert.That(new Fraction(1, 2) * new Fraction(1, 2) == new Fraction(1, 3), Is.False);
            Assert.That(-2 * new Fraction(1, 2) == -1);
        }

        [Test]
        public void Divide()
        {
            Assert.That(new Fraction(1, 2), Is.EqualTo(new Fraction(1, 1) / 2));
            Assert.That(new Fraction(11, 2), Is.EqualTo(new Fraction(11, 1) / 2));
            Assert.That(new Fraction(100, 50) / 2 == new Fraction(2, 2));
            Assert.That(new Fraction(600, 2) / 3 == 100);
            Assert.That(new Fraction(600, 2) / 3 == 20, Is.False);
        }

        [Test]
        public void Sum()
        {
            var one = new Fraction(1, 1);
            Assert.That(one.Reciprocal() + one == 2);
            Assert.That(new Fraction(1, 2), Is.EqualTo(new Fraction(1, 4) + new Fraction(1, 4)));
            Assert.That(new Fraction(10, 2), Is.EqualTo(Fraction.Parse("-2/-2") + 4));
            Assert.That(Fraction.Parse("0/-2") + 8 == 2, Is.False);
            Assert.That(Fraction.Parse("-1/1") + 10 == 9);
        }

        [Test]
        public void Subtraction()
        {
            var one = new Fraction(1, 1);
            var two = new Fraction(2, 1);
            var twothirds = new Fraction(2, 3);
            Assert.That(one - two == -1);
            Assert.That(one - twothirds, Is.EqualTo(new Fraction(1, 3)));
            Assert.That(new Fraction(10, 2) - one == 4);
            Assert.That(two - 2, Is.EqualTo(one - 1));
            Assert.That(two - 3 != one);
            Assert.That(two - 3 == -1);
            Assert.That(two - 4 != -10);
        }

        [Test]
        public void ZeroDivisionException()
        {
            Assert.Throws<DivideByZeroException>(() =>
            {
                new Fraction(1, 0).GetHashCode();
            });
            Assert.Throws<DivideByZeroException>(() =>
            {
                (Fraction.Parse("1") / 0).GetHashCode();
            });
        }

        [Test]
        public void HashesAreEqual()
        {
            Assert.That(new Fraction().GetHashCode(), Is.EqualTo(default(Fraction).GetHashCode()));
            var fraction = new Fraction(1, 2);
            var fractionFromString = Fraction.Parse("1/2");
            Assert.That(fraction.GetHashCode(), Is.EqualTo(fractionFromString.GetHashCode()));
            Assert.That(((Fraction)3).GetHashCode(), Is.EqualTo(new Fraction(3, 1).GetHashCode()));
            Assert.That((-(Fraction)3).GetHashCode(), Is.EqualTo(((Fraction)(-3)).GetHashCode()));
        }

        [Test]
        public void StringTest()
        {
            Assert.That(default(Fraction).ToString(), Is.EqualTo("0"));
            Assert.That(new Fraction(1, 2).ToString(), Is.EqualTo("1/2"));
            Assert.That((-new Fraction(2, 2)).ToString(), Is.EqualTo("-1"));
            Assert.That(new Fraction(2, -3).ToString(), Is.EqualTo("-2/3"));
        }

    }
}
