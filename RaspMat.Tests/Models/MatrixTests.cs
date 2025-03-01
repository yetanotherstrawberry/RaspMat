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
            Assert.Equals(zeroInt, zero);
        }

        [Test]
        public void IdentityEqualsIdentity()
        {
            var matI = new Matrix(3, 3, (row, col) => row == col ? 1 : 0);
            Assert.That(matI, Is.EqualTo(identity));
        }

        [Test]
        public void GaussIdentityTest()
        {
            Assert.That(Matrix.Identity(3), Is.EqualTo(equalToIdentity.GaussianElimination().Last().Result));
        }

        [Test]
        public void EqualsTest()
        {
            Assert.That(equalToIdentity.Equals(equalToIdentity));
            Assert.That(equalToIdentity.Equals(identity));
            Assert.That(equalToIdentity.Equals(new object()));
        }

        [Test]
        public void GetHashTest()
        {
            Assert.That(identity.GetHashCode(), Is.EqualTo(identity.GetHashCode()));
            Assert.That(identity.GetHashCode(), Is.Not.EqualTo(equalToIdentity.GetHashCode()));
            Assert.That(equalToIdentity.GetHashCode(), Is.EqualTo(equalToIdentity.GetHashCode()));
        }

        [Test]
        public void TransposeTest()
        {
            Assert.That(identity, Is.EqualTo(Matrix.Transpose(identity)));
            Assert.That(identity, Is.Not.EqualTo(Matrix.Transpose(equalToIdentity)));
            Assert.That(verticalOnes, Is.EqualTo(Matrix.Transpose(horizontalOnes)));
            Assert.That(verticalOnes, Is.Not.EqualTo(horizontalOnes));
        }

        [Test]
        public void AddISliceTest()
        {
            var addedI = Matrix.WithIdentity(identity, onLeft: true);
            Assert.That(identity, Is.EqualTo(Matrix.Slice(addedI, removeLeft: true)));
            Assert.That(identity, Is.EqualTo(Matrix.Slice(addedI, removeLeft: false)));

            addedI = Matrix.WithIdentity(identity, onLeft: false);
            Assert.That(identity, Is.EqualTo(Matrix.Slice(addedI, removeLeft: true)));
            Assert.That(identity, Is.EqualTo(Matrix.Slice(addedI, removeLeft: false)));

            var temp = new[]
            {
                1, 2, 2, 1, 0, 0,
                1, 0, 1, 0, 1, 0,
                1, 1, 1, 0, 0, 1,
            };
            var expected = new Matrix(3, 6, (row, column) => temp[row * 6 + column]);
            addedI = Matrix.WithIdentity(equalToIdentity, onLeft: false);
            Assert.That(expected, Is.EqualTo(addedI));
        }

        [Test]
        public void ToStringTest()
        {
            string ConcatRows(params object[] strings) => string.Join(Environment.NewLine, strings);
            string ConcatColumns(params object[] strings) => string.Join("\t", strings);

            Assert.That(ConcatRows(ConcatColumns(0, 0), ConcatColumns(0, 0)), Is.EqualTo(zero.ToString()));
            Assert.That(ConcatRows(ConcatColumns(1, 0, 0), ConcatColumns(0, 1, 0), ConcatColumns(0, 0, 1)), Is.EqualTo(identity.ToString()));
        }

        [Test]
        public void MultiplicationTest()
        {
            Assert.That(identity, Is.EqualTo(identity * identity));
            Assert.That(equalToIdentity, Is.EqualTo(identity * equalToIdentity));
            Assert.That(equalToIdentity, Is.EqualTo(equalToIdentity * identity));
            Assert.That(zero, Is.EqualTo(zero * verticalOnes));
            Assert.That(zero, Is.EqualTo(verticalOnes * zero));
        }

    }
}
