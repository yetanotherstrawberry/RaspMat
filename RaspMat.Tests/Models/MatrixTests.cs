using NUnit.Framework;
using RaspMat.Models;

namespace RaspMat.Tests.Models
{
    /// <summary>
    /// Tests for the <see cref="Matrix"/> <see langword="class"/>.
    /// </summary>
    public class MatrixTests
    {

        [Test]
        public void ZeroEqualsZero()
        {
            for (var rows = 1; rows <= 3; rows++)
            {
                for (var columns = 1; columns <= 3; columns++)
                {
                    var zeroDefault = new Matrix(rows, columns);
                    var zeroManual = new Matrix(rows, columns, (row, column) => 0);
                    Assert.That(zeroDefault, Is.EqualTo(zeroManual));
                    var sabotage = new Matrix(rows, columns, (row, column) => row + column + 1);
                    Assert.That(zeroDefault, Is.Not.EqualTo(sabotage));
                }
            }
        }

        [Test]
        public void IdentityEqualsIdentity()
        {
            for (var size = 1; size <= 3; size++)
            {
                var identityManual = new Matrix(size, size, (row, col) => row == col ? 1 : 0);
                Assert.That(identityManual, Is.EqualTo(Matrix.Identity(size)));
            }
        }

        [Test]
        public void Multiplication()
        {
            for (var size = 1; size <= 3; size++)
            {
                var matrix = new Matrix(size, size, (row, column) => new Fraction(row + column + 1, column + 1));
                var matrixDouble = new Matrix(size, size, (row, column) => 2 * new Fraction(row + column + 1, column + 1));
                var identity = Matrix.Identity(size);
                var doubleIdentity = new Matrix(identity.Rows, identity.Columns, (row, column) => identity[row, column] * 2);
                var result = matrix * identity;
                Assert.That(result, Is.EqualTo(matrix));
                Assert.That(result, Is.Not.EqualTo(new Matrix(size, size)));
                Assert.That(matrix * doubleIdentity, Is.EqualTo(matrixDouble));
            }
        }

    }
}
