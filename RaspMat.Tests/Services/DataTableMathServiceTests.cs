using NUnit.Framework;
using RaspMat.Services;
using System;

namespace RaspMat.Tests.Services
{
    /// <summary>
    /// Tests for the <see cref="DataTableMathService"/> <see langword="class"/>.
    /// </summary>
    internal class DataTableMathServiceTests : IDisposable
    {

        /// <summary>
        /// The <see cref="DataTableMathService"/> instance used for testing.
        /// </summary>
        private DataTableMathService _service;

        [OneTimeSetUp]
        public void Setup()
        {
            _service = new DataTableMathService();
        }

        [Test]
        public void Sum()
        {
            Assert.That(_service.Compute<int>("1 + 1"), Is.EqualTo(2));
        }

        [Test]
        public void Subtract()
        {
            Assert.That(_service.Compute<short>("1 - 3"), Is.EqualTo(-2));
        }

        [Test]
        public void Divide()
        {
            Assert.That(_service.Compute<decimal>("4 / 2"), Is.EqualTo(2m));
        }

        [Test]
        public void Multiply()
        {
            Assert.That(_service.Compute<decimal>("0.5 * 2"), Is.EqualTo(1.0));
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            _service?.Dispose();
        }

    }
}
