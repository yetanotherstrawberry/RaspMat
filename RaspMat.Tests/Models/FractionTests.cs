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
            Assert.True(Fraction.Zero == 0);
            Assert.AreEqual(Fraction.Zero, Fraction.Zero);
            Assert.AreNotEqual(Fraction.Zero, _one);
        }

        [Test]
        public void ZeroPlusMinusOne()
        {
            var ret = Fraction.Zero + 1;
            Assert.True(ret == 1);
            Assert.AreEqual(Fraction.Zero, ret - 1);
        }

        [Test]
        public void TwoHalvesEqualOne()
        {
            var half1 = new Fraction(1, 2);
            var half2 = _one / 2;
            Assert.AreEqual(_one, half1 + half2);
            Assert.AreEqual(half2, half1);
            Assert.False(half1 == _one);
            Assert.AreNotEqual(_one, half2);
        }

        [Test]
        public void Reciprocal()
        {
            var half = new Fraction(1, 2);
            Assert.True(half.Reciprocal() == 2);
            Assert.AreEqual(_one, new Fraction(2, 2).Reciprocal());
            Assert.True(half != 2);
        }

        [Test]
        public void Multiplication()
        {
            Assert.True(Fraction.Zero * Fraction.Zero == 0);
            Assert.True(_one * Fraction.Zero == 0);
            Assert.True(_one * 0 == 0);
            Assert.True(_one * 1 != 0);
        }

        [Test]
        public void Divide()
        {
            Assert.AreEqual(new Fraction(1, 2), _one / 2);
            Assert.AreEqual(new Fraction(11, 2), new Fraction(11, 1) / 2);
            Assert.True(new Fraction(100, 50) / 2 == _one);
            Assert.True(new Fraction(600, 2) / 3 == 100);
            Assert.False(new Fraction(600, 2) / 3 == 20);
        }

        [Test]
        public void Sum()
        {
            Assert.True(_one.Reciprocal() + _one == 2);
            Assert.AreEqual(new Fraction(1, 2), new Fraction(1, 4) + new Fraction(1, 4));
            Assert.AreEqual(new Fraction(10, 2), _one + 4);
            Assert.False(_one + 8 == 2);
            Assert.True(_one + 8 == 9);
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
