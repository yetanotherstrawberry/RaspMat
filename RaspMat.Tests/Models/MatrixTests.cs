using NUnit.Framework;
using RaspMat.Helpers;
using RaspMat.Models;
using System;
using System.Linq;

namespace RaspMat.Tests.Models
{
    /// <summary>
    /// Tests for <see cref="Matrix"/>.
    /// </summary>
    public class MatrixTests
    {

        private Matrix equalToIdentity, identity, zero, verticalOnes, horizontalOnes;

        [SetUp]
        public void Setup()
        {
            int[] mat = null;

            Func<int, int, Fraction> func = (int row, int column) => mat[row * 3 + column];

            mat = new[] {
                1, 2, 2,
                1, 0, 1,
                1, 1, 1,
            };
            equalToIdentity = new Matrix(3, 3, func);

            func = (int row, int column) => mat[row * 2 + column];

            mat = new[]
            {
                0, 1,
                0, 1,
            };
            verticalOnes = new Matrix(2, 2, func);

            mat = new[]
            {
                0, 0,
                1, 1,
            };
            horizontalOnes = new Matrix(2, 2, func);

            identity = Matrix.Identity(3);

            mat = new[]
            {
                0, 0,
                0, 0,
            };
            zero = new Matrix(2, 2, func);
        }

        [Test]
        public void ZeroEqualsZero()
        {
            var zeroInt = new Matrix(2, 2);
            Assert.AreEqual(zeroInt, zero);
        }

        [Test]
        public void IdentityEqualsIdentity()
        {
            var matI = new Matrix(3, 3, (row, col) => row == col ? Fraction.Parse("1") : 0);
            Assert.AreEqual(matI, identity);
        }

        [Test]
        public void GaussIdentityTest()
        {
            Assert.AreEqual(Matrix.Identity(3), equalToIdentity.GaussianElimination().Last().Result);
        }

        [Test]
        public void EqualsTest()
        {
            Assert.True(equalToIdentity.Equals(equalToIdentity));
            Assert.False(equalToIdentity.Equals(identity));
            Assert.False(equalToIdentity.Equals(new object()));
        }

        [Test]
        public void GetHashTest()
        {
            Assert.AreEqual(identity.GetHashCode(), identity.GetHashCode());
            Assert.AreNotEqual(identity.GetHashCode(), equalToIdentity.GetHashCode());
            Assert.AreEqual(equalToIdentity.GetHashCode(), equalToIdentity.GetHashCode());
        }

        [Test]
        public void TransposeTest()
        {
            Assert.AreEqual(identity, Matrix.Transpose(identity));
            Assert.AreNotEqual(identity, Matrix.Transpose(equalToIdentity));
            Assert.AreEqual(verticalOnes, Matrix.Transpose(horizontalOnes));
            Assert.AreNotEqual(verticalOnes, horizontalOnes);
        }

        [Test]
        public void AddISliceTest()
        {
            var addedI = Matrix.AddI(identity, onLeft: true);
            Assert.AreEqual(identity, Matrix.Slice(addedI, removeLeft: true));
            Assert.AreEqual(identity, Matrix.Slice(addedI, removeLeft: false));

            addedI = Matrix.AddI(identity, onLeft: false);
            Assert.AreEqual(identity, Matrix.Slice(addedI, removeLeft: true));
            Assert.AreEqual(identity, Matrix.Slice(addedI, removeLeft: false));

            var temp = new[]
            {
                1, 2, 2, 1, 0, 0,
                1, 0, 1, 0, 1, 0,
                1, 1, 1, 0, 0, 1,
            };
            var expected = new Matrix(3, 6, (row, column) => temp[row * 6 + column]);
            addedI = Matrix.AddI(equalToIdentity, onLeft: false);
            Assert.AreEqual(expected, addedI);
        }

        [Test]
        public void ToStringTest()
        {
            string ConcatRows(params object[] strings) => string.Join(Environment.NewLine, strings);
            string ConcatColumns(params object[] strings) => string.Join("\t", strings);

            Assert.AreEqual(ConcatRows(ConcatColumns(0, 0), ConcatColumns(0, 0)), zero.ToString());
            Assert.AreEqual(ConcatRows(ConcatColumns(1, 0, 0), ConcatColumns(0, 1, 0), ConcatColumns(0, 0, 1)), identity.ToString());
        }

        [Test]
        public void MultiplicationTest()
        {
            Assert.AreEqual(identity, identity * identity);
            Assert.AreEqual(equalToIdentity, identity * equalToIdentity);
            Assert.AreEqual(equalToIdentity, equalToIdentity * identity);
            Assert.AreEqual(zero, zero * verticalOnes);
            Assert.AreEqual(zero, verticalOnes * zero);
        }

    }
}
