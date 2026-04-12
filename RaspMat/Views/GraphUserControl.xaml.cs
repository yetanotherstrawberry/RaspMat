using RaspMat.Extensions;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for <see cref="GraphUserControl"/>.
    /// </summary>
    internal partial class GraphUserControl : UserControlBase
    {

        /// <summary>
        /// Used for accessing the UI.
        /// </summary>
        private readonly IViewService _viewService;

        /// <summary>
        /// Used for communication with other componenets.
        /// </summary>
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes XAML and registers services.
        /// </summary>
        public GraphUserControl()
        {
            _viewService = App.GetService<IViewService>().ThrowIfNull();
            _eventService = App.GetService<IEventService>().ThrowIfNull();

            InitializeComponent();
        }

        /// <summary>
        /// Handles the removal of a vertex.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> the requested the operation.</param>
        /// <param name="eventArgs">Additional arguments.</param>
        private void RemoveVertex(object sender, RoutedEventArgs eventArgs) => GraphControl.RemoveVertex(VertexToRemoveName.Text);

        /// <summary>
        /// Handles the creation of a vertex.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> the requested the operation.</param>
        /// <param name="eventArgs">Additional arguments.</param>
        private void AddVertex(object sender, RoutedEventArgs eventArgs) => GraphControl.AddVertex(VertexName.Text);

        /// <summary>
        /// Executes the <paramref name="handler"/> with 2 vertices.
        /// </summary>
        /// <param name="handler">The <see cref="Action{T1, T2}"/> to execute on an edge.</param>
        private void HandleEdge(Action<string, string> handler) => handler(EdgeSource.Text, EdgeTarget.Text);

        /// <summary>
        /// Handles the removal of an edge.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> the requested the operation.</param>
        /// <param name="eventArgs">Additional arguments.</param>
        private void RemoveEdge(object sender, RoutedEventArgs eventArgs) => HandleEdge((source, target) => GraphControl.RemoveEdge(source, target));

        /// <summary>
        /// Handles the creation of an edge.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> the requested the operation.</param>
        /// <param name="eventArgs">Additional arguments.</param>
        private void AddEdge(object sender, RoutedEventArgs eventArgs)
        {
            var probabilityText = EdgeProbability.Text;
            if (string.IsNullOrWhiteSpace(probabilityText)) throw new ArgumentNullException(nameof(EdgeProbability));
            var probability = Fraction.Parse(probabilityText);
            if (probability.Numerator.Sign < 0) throw new ArgumentOutOfRangeException(nameof(EdgeProbability));
            HandleEdge((source, target) => GraphControl.AddEdge(source, target, probability.ToString()));
        }

        /// <summary>
        /// Handles the reduction and UI update of the probability chain.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> the requested the operation.</param>
        /// <param name="eventArgs">Additional arguments.</param>
        private void CalculateNode(object sender, RoutedEventArgs eventArgs)
        {
            var oldVertices = GraphControl.GetVertices();
            var probabilityMatrix = GraphControl.ToProbabilityMatrix(out var elimination);
            var newSourceVertices = probabilityMatrix.Sources.ToDictionary(source => source, source => oldVertices[source]);
            var newTargetVertices = probabilityMatrix.Targets.ToDictionary(target => target, target => oldVertices[target]);
            var newVertices = newSourceVertices.Concat(newTargetVertices).ToDictionary();
            var newEdges = newSourceVertices.Keys.ToDictionary(source => source, source => (IDictionary<string, string>)newTargetVertices.Keys.Select(target => new KeyValuePair<string, Fraction>(target, probabilityMatrix[source, target])).Where(kvp => !kvp.Value.IsZero).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()));
            GraphControl.SetVerticesAndEdges(newVertices, newEdges);
            _eventService.Send(new Events.LoadMatrixEvent(probabilityMatrix));
            _eventService.Send(new Events.LoadStepsEvent(elimination));
        }

        /// <summary>
        /// Toggles the step view.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> the requested the operation.</param>
        /// <param name="eventArgs">Additional arguments.</param>
        private void ToggleStepView(object sender, RoutedEventArgs e)
        {
            _viewService.ToggleStepsView();
        }

    }
}
