using RaspMat.Extensions;
using System;
using System.Windows;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for <see cref="GraphUserControl"/>.
    /// </summary>
    internal partial class GraphUserControl : UserControlBase
    {

        /// <summary>
        /// Initializes XAML.
        /// </summary>
        public GraphUserControl()
        {
            InitializeComponent();
        }

        private void RemoveVertex(object sender, RoutedEventArgs eventArgs) => GraphControl.RemoveVertex(VertexToRemoveName.Text);

        private void AddVertex(object sender, RoutedEventArgs eventArgs) => GraphControl.AddVertex(VertexName.Text);

        private void HandleEdge(Action<string, string> handler) => handler(EdgeSource.Text, EdgeTarget.Text);

        private void RemoveEdge(object sender, RoutedEventArgs eventArgs) => HandleEdge((source, target) => GraphControl.RemoveEdge(source, target));

        private void AddEdge(object sender, RoutedEventArgs eventArgs) => HandleEdge((source, target) => GraphControl.AddEdge(source, target));

    }
}
