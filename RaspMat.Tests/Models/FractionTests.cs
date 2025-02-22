using NUnit.Framework;
using RaspMat.Models;
using System;
using System.Numerics;

namespace RaspMat.Tests.Models
{
    /// <summary>
    /// Tests for <see cref="Fraction"/>.
    /// </summary>
    public class FractionTests
    {

        private readonly Fraction _one = new Fraction(1, 1);

        [Test]
        public void ZeroEqualsZero()
        {
            Assert.That(Fraction.Zero == 0);
            Assert.That(Fraction.Zero, Is.EqualTo(Fraction.Zero));
            Assert.That(Fraction.Zero, Is.Not.EqualTo(_one));
        }

        [Test]
        public void ZeroPlusMinusOne()
        {
            var ret = Fraction.Zero + 1;
            Assert.That(ret == 1);
            Assert.That(Fraction.Zero, Is.EqualTo(ret - 1));
        }

        [Test]
        public void TwoHalvesEqualOne()
        {
            var half1 = new Fraction(1, 2);
            var half2 = _one / 2;
            Assert.That(_one, Is.EqualTo(half1 + half2));
            Assert.That(half2, Is.EqualTo(half1));
            Assert.That(half1 == _one, Is.False);
            Assert.That(_one, Is.Not.EqualTo(half2));
        }

        [Test]
        public void Reciprocal()
        {
            var half = new Fraction(1, 2);
            Assert.That(half.Reciprocal() == 2);
            Assert.That(_one, Is.EqualTo(new Fraction(2, 2).Reciprocal()));
            Assert.That(half != 2);
        }

        [Test]
        public void Multiplication()
        {
            Assert.That(Fraction.Zero * Fraction.Zero == 0);
            Assert.That(_one * Fraction.Zero == 0);
            Assert.That(_one * 0 == 0);
            Assert.That(_one * 1 != 0);
        }

        [Test]
        public void Divide()
        {
            Assert.That(new Fraction(1, 2), Is.EqualTo(_one / 2));
            Assert.That(new Fraction(11, 2), Is.EqualTo(new Fraction(11, 1) / 2));
            Assert.That(new Fraction(100, 50) / 2 == _one);
            Assert.That(new Fraction(600, 2) / 3 == 100);
            Assert.That(new Fraction(600, 2) / 3 == 20, Is.False);
        }

        [Test]
        public void Sum()
        {
            Assert.That(_one.Reciprocal() + _one == 2);
            Assert.That(new Fraction(1, 2), Is.EqualTo(new Fraction(1, 4) + new Fraction(1, 4)));
            Assert.That(new Fraction(10, 2), Is.EqualTo(_one + 4));
            Assert.That(_one + 8 == 2, Is.False);
            Assert.That(_one + 8 == 9);
        }

        [Test]
        public void Subtraction()
        {
            Assert.True(_one - 1 == 0);
            Assert.AreEqual(Fraction.Zero, _one - 1);
            Assert.True(_one - 2 == -1);
            Assert.AreEqual(new Fraction(5, 6), new Fraction(3, 2) - new Fraction(2, 3));
            Assert.AreEqual(-new Fraction(950, 100), new Fraction(1, 2) - new Fraction(10, 1));
        }

        [Test]
        public void ZeroDivision()
        {
            Assert.Throws<DivideByZeroException>(() =>
            {
                var badValue = new Fraction(1, 0);
            });
            Assert.Throws<DivideByZeroException>(() =>
            {
                var badValue = _one / 0;
            });
        }

        [Test]
        public void Equality()
        {
            Assert.True(_one.Equals(_one));
            Assert.True(_one.Equals(new Fraction(1, 1)));
            Assert.False(_one.Equals(new Fraction(0, 1)));
            Assert.False(_one.Equals(new object()));
        }

        [Test]
        public void Hash()
        {
            Assert.AreEqual(_one.GetHashCode(), _one.GetHashCode());
            Assert.AreEqual(_one.GetHashCode(), _one.Reciprocal().GetHashCode());
            Assert.AreNotEqual(_one.GetHashCode(), new Fraction(1, 2).GetHashCode());
            Assert.DoesNotThrow(() =>
            {
                new Fraction(int.MaxValue, int.MaxValue).GetHashCode();
                new Fraction((long)int.MaxValue * 2, long.MaxValue).GetHashCode();
            });
        }

        [Test]
        public void StringTest()
        {
            Assert.AreEqual(1.ToString(), _one.ToString());
            Assert.AreEqual("-1/2", new Fraction(-1, 2).ToString());
        }

        [Test]
        public void Getters()
        {
            Assert.AreEqual(BigInteger.One, _one.Numerator);
            Assert.AreEqual(BigInteger.One, _one.Denominator);
            Assert.AreNotEqual(BigInteger.Zero, _one.Numerator);
            Assert.AreNotEqual(BigInteger.Zero, _one.Denominator);
        }

    }
}
