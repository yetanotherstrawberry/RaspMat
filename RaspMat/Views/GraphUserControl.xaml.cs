using RaspMat.Extensions;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// Used for (de)serialization of the <see cref="GraphControl"/>.
        /// </summary>
        private readonly ISerializationService _serializationService;

        /// <summary>
        /// Initializes XAML and registers services.
        /// </summary>
        public GraphUserControl()
        {
            _viewService = App.GetService<IViewService>().ThrowIfNull();
            _serializationService = App.GetService<ISerializationService>().ThrowIfNull();

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
        /// Calculates probabilites for a given node.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> the requested the operation.</param>
        /// <param name="eventArgs">Additional arguments.</param>
        private void CalculateNode(object sender, RoutedEventArgs eventArgs)
        {
            var rootNode = VertexCalculate.Text;
            var matrix = GraphControl.ToMatrix(rootNode, out var indexes);
            var matStr = matrix.ToString();
            matrix = matrix.GaussianElimination().Last().Result;
            var matStr2 = matrix.ToString();
            var rootIndex = indexes[rootNode];
            var vertices = GraphControl.GetVertices();
            var edges = new Dictionary<string, IDictionary<string, string>>()
            {
                { rootNode, new Dictionary<string, string>(vertices.Count) },
            };

            foreach (var vertex in vertices)
            {
                var probability = matrix[rootIndex, indexes[vertex.Key]];
                if (!probability.IsZero)
                {
                    edges[rootNode].Add(vertex.Key, probability.ToString());
                }
            }

            GraphControl.SetVerticesAndEdges(vertices, edges);
        }
        
        /// <summary>
        /// Locks the UI, executes asynchronously the <paramref name="action"/> and unlocks the UI regardless whether the <see cref="Task"/> completed successfully or failed.
        /// </summary>
        /// <param name="action">The <see cref="Task"/> to <see langword="await"/> for before unlocking the UI.</param>
        private async void ExecuteAsync(Func<Task> action)
        {
            try
            {
                _viewService.Execute(() => IsEnabled = false);
                await action();
            }
            finally
            {
                _viewService.Execute(() => IsEnabled = true);
            }
        }

        /// <summary>
        /// Handles the saving of the state.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that requested the operation.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> of the <see langword="event"/>.</param>
        private void SaveHandler(object sender, RoutedEventArgs eventArgs)
        {
            ExecuteAsync(async () =>
            {
                var rootNode = GraphControl.GetVertices().First();
                await _serializationService.SerializeAsync(new ProbabilityChain(GraphControl.ToMatrix(rootNode.Key, out var indexes), indexes.Keys));
            });
        }

        /// <summary>
        /// Handles the loading of the state.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that requested the operation.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> of the <see langword="event"/>.</param>
        private void LoadHandler(object sender, RoutedEventArgs eventArgs)
        {
            var random = new Random();
            ExecuteAsync(async () =>
            {
                var probabilityChain = await _serializationService.DeserializeAsync<ProbabilityChain>();
                if (probabilityChain is null) return;
                var indexes = probabilityChain.GetIndexes().ToDictionary(index => index.Value, index => index.Key);
                /*var vertices = indexes.ToDictionary(index => index.Key.ToString(), index => null as object);
                GraphControl.SetVerticesAndEdges(vertices, probabilityChain.Select((cells, row) => {
                    return new KeyValuePair<string, IDictionary<string, string>>(indexes[row], (IDictionary<string, string>)cells.Select((cell, column) =>
                    {
                        return new KeyValuePair<string, KeyValuePair<string, string>>(indexes[column], new KeyValuePair<string, string>(indexes[column], cell.ToString()));
                    }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                GraphControl.SetProbabilities(probabilityChain);*/
            });
        }

    }
}
