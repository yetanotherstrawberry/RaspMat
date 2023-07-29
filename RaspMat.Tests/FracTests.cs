using NUnit.Framework;
using RaspMat.Models;
using System;

namespace RaspMat.Tests
{
    /// <summary>
    /// Tests for <see cref="Fraction"/>s.
    /// </summary>
    public class FracTests
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
            Assert.False(half == 2);
        }

        [Test]
        public void Multiplication()
        {
            Assert.True(Fraction.Zero * Fraction.Zero == 0);
            Assert.True(_one * Fraction.Zero == 0);
            Assert.True(_one * 0 == 0);
            Assert.False(_one * 1 == 0);
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
                var undefined = new Fraction(1, 0);
            });
            Assert.Throws<DivideByZeroException>(() =>
            {
                var undefined = _one / 0;
            });
        }

    }
}
