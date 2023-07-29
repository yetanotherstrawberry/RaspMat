using NUnit.Framework;
using RaspMat.Helpers;
using RaspMat.Models;
using System.Linq;

namespace RaspMat.Tests
{
    /// <summary>
    /// Tests for the <see cref="Matrix"/> class.
    /// </summary>
    public class MatTests
    {

        private Matrix equalToIdentity, identity, zero;

        [SetUp]
        public void Setup()
        {
            var mat = new[] {
                1, 2, 2,
                1, 0, 1,
                1, 1, 1 };
            equalToIdentity = new Matrix(3, 3, (row, column) => mat[row + column]);

            identity = Matrix.Identity(3);

            zero = new Matrix(new string[][]{
                new string[]
                {
                    "0", "0/1",
                },
                new string[]
                {
                    "-0/1", "-0/-1",
                }
            });
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
            Assert.AreEqual(Matrix.Identity(3), Algorithms.GaussianElimination(equalToIdentity).Last().Result);
        }

    }
}
