using NUnit.Framework;
using RaspMat.Extensions;
using RaspMat.Helpers;
using RaspMat.Models;
using System.Linq;

namespace RaspMat.Tests.Extensions
{
    /// <summary>
    /// Tests for extensions of the <see cref="Matrix"/> <see langword="class">.
    /// </summary>
    public class MatrixExtensions
    {

        [Test]
        public void GaussianElimination()
        {
            var identity = Matrix.Identity(3);
            var reduced = identity.GaussianElimination().Last().Result;
            Assert.That(reduced, Is.EqualTo(identity));
            var tripleIdentity = new Matrix(identity.Rows, identity.Columns, (row, column) => identity[row, column] * 3);
            var tripleReduced = tripleIdentity.GaussianElimination().Last().Result;
            Assert.That(tripleReduced, Is.EqualTo(identity.Transpose()));
            var halfIdentity = new Matrix(identity.Rows, identity.Columns, (row, column) => identity[row, column] / 2);
            var halfReduced = halfIdentity.GaussianElimination().Last().Result;
            Assert.That(halfReduced, Is.EqualTo(identity));
        }

        [Test]
        public void BasisChangeMatrix()
        {
            var from = new Fraction[][]
            {
                new Fraction[] { 1, 0 },
                new Fraction[] { 0, 1 }
            };
            var to = new Fraction[][]
            {
                new Fraction[] { 3, 1 },
                new Fraction[] { -2, 1 }
            };
            /*var expected = new Matrix(new Fraction[][]
            {
                new Fraction[] { 1, 1 },
                new Fraction[] { 1, -1 }
            });*/
            var basisChangeMatrix = Algorithms.BasisChangeMatrix(from, to);
        }

    }
}
