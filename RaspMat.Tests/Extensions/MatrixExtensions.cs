using NUnit.Framework;
using RaspMat.Extensions;
using RaspMat.Helpers;
using RaspMat.Models;
using System;
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
        public void BasisChangeMatrix3D()
        {
            var from = Matrix.Identity(3);
            var to = Matrix.Parse(
                "1 0 0"
                + Environment.NewLine +
                "0 1 0"
                + Environment.NewLine +
                "0 0 1"
            );
            var expected = Matrix.Identity(3);
            var basisChangeMatrix = Algorithms.BasisChangeMatrix(from, to);
            Assert.That(basisChangeMatrix, Is.EqualTo(expected));
        }

        [Test]
        public void BasisChangeMatrix2D()
        {
            var from = Matrix.Parse(
                "1 1"
                + Environment.NewLine +
                "0 -1"
            );
            var to = Matrix.Parse(
                "0 1"
                + Environment.NewLine +
                "1 1"
            );
            var expected = Matrix.Parse(
                "-1 -2"
                + Environment.NewLine +
                "1 1"
            );
            var basisChangeMatrix = Algorithms.BasisChangeMatrix(from, to);
            Assert.That(basisChangeMatrix, Is.EqualTo(expected));
        }

    }
}
