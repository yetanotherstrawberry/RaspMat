using Prism.Events;
using RaspMat.DTOs;
using RaspMat.Models;
using System.Collections.Generic;

namespace RaspMat.Helpers
{
    internal static class Events
    {

        public class LoadMatrixEvent : PubSubEvent<Matrix> { }
        public class LoadStepsEvent : PubSubEvent<ICollection<AlgorithmStepDTO<Matrix>>> { }

    }
}
