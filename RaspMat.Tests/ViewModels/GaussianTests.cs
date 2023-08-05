using NUnit.Framework;
using RaspMat.ViewModels;

namespace RaspMat.Tests.ViewModels
{
    /// <summary>
    /// Tests for <see cref="GaussianUserControlViewModel"/>.
    /// </summary>
    public class GaussianTests
    {

        private GaussianUserControlViewModel _userControlViewModel;

        [SetUp]
        public void Setup()
        {
            _userControlViewModel = new GaussianUserControlViewModel(null, null, null, null);
        }

        [Test]
        public void IsFreeTest()
        {
            Assert.True(_userControlViewModel.IsFree);
        }

    }
}
