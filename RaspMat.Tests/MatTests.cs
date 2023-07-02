using NUnit.Framework;
using RaspMat.Helpers;
using RaspMat.Models;
using System.Linq;

namespace RaspMat.Tests
{
    public class MatTests
    {

        private Matrix equalToIdentity, identity, zero;

        [SetUp]
        public void Setup()
        {
            equalToIdentity = new Matrix(new Fraction[][]
            {
                new Fraction[]
                {
                    1, 2, 2,
                },
                new Fraction[]
                {
                    1, 0, 1,
                },
                new Fraction[]
                {
                    1, 1, 1,
                },
            });
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
            var zeroInt = new Matrix(2);
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
            Assert.AreEqual(Matrix.Identity(3), Algorithms.TotalGaussianElimination(equalToIdentity).Last().Result);
        }

    }
}
