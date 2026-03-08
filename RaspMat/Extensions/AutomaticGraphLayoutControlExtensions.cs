using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using RaspMat.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="AutomaticGraphLayoutControl"/>.
    /// </summary>
    internal static class AutomaticGraphLayoutControlExtensions
    {

        /// <summary>
        /// Removes an edge.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="source">The starting vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        /// <exception cref="KeyNotFoundException">Edge not found.</exception>
        public static Graph RemoveEdge(this AutomaticGraphLayoutControl element, string source, string target)
        {
            if (element.Graph is null) element.SetGraph();
            var edges = element.GetEdges();
            var success = edges.TryGetValue(source, out var sourceEdges) && sourceEdges.Remove(target);
            if (!success) throw new KeyNotFoundException();
            return element.SetVerticesAndEdges(element.GetVertices(), edges);
        }

        /// <summary>
        /// Creates an edge.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="source">The starting vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="label">The attatched label.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        /// <exception cref="InvalidOperationException">An edge between <paramref name="source"/> and <paramref name="target"/> already exists.</exception>
        public static Graph AddEdge(this AutomaticGraphLayoutControl element, string source, string target, string label = null)
        {
            if (element.Graph is null) element.SetGraph();
            var edges = element.GetEdges();
            var targetEdges = edges.TryGetValue(source, out var dictionary) ? dictionary : null;
            if (targetEdges is null)
            {
                targetEdges = new Dictionary<string, string>();
                edges.Add(source, targetEdges);
            }
            else
            {
                if (targetEdges.ContainsKey(target)) throw new DuplicateNameException();
            }
            targetEdges.Add(target, label);
            return element.SetVerticesAndEdges(element.GetVertices(), edges);
        }

        /// <summary>
        /// Removes a vertex.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="name">The vertex to remove.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        /// <exception cref="KeyNotFoundException">Element does not exist.</exception>
        public static Graph RemoveVertex(this AutomaticGraphLayoutControl element, string name)
        {
            if (element.Graph is null) element.SetGraph();
            var vertices = element.GetVertices();
            if (!vertices.Remove(name)) throw new KeyNotFoundException();
            return element.SetVerticesAndEdges(vertices, element.GetEdges());
        }

        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="name">Name of the vertex.</param>
        /// <param name="label">Label of the vertex.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        public static Graph AddVertex(this AutomaticGraphLayoutControl element, string name, object label = null)
        {
            if (element.Graph is null) element.SetGraph();
            var vertices = element.GetVertices();
            vertices.Add(name, label);
            return element.SetVerticesAndEdges(vertices, element.GetEdges());
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to use.</param>
        /// <returns>A <see langword="new"/> <see cref="IDictionary{TKey, TValue}"/> with source edges, target edges and labels.</returns>
        public static IDictionary<string, IDictionary<string, string>> GetEdges(this AutomaticGraphLayoutControl element)
        {
            return element.Graph.Edges
                .GroupBy(edge => edge.Source)
                .ToDictionary(grouping => grouping.Key, grouping => (IDictionary<string, string>)grouping.ToDictionary(group => group.Target, group => group.LabelText));
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to use.</param>
        /// <returns>Vertices (<see cref="IDictionary{TKey, TValue}.Keys"/>) and labels.</returns>
        public static IDictionary<string, object> GetVertices(this AutomaticGraphLayoutControl element)
        {
            return element.Graph.Nodes.ToDictionary(node => node.Id, node => node.UserData);
        }

        /// <summary>
        /// Sets vertices and edges of the <see cref="Graph"/>.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="vertices">Vertices (<see cref="IDictionary{TKey, TValue}.Keys"/>) and labels.</param>
        /// <param name="edges">Edges with starting vertices, target vertices and labels.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        public static Graph SetVerticesAndEdges(this AutomaticGraphLayoutControl element, IDictionary<string, object> vertices, IDictionary<string, IDictionary<string, string>> edges)
        {
            var graph = new Graph();
            foreach (var vertex in vertices)
            {
                graph.AddNode(new Node(vertex.Key)
                {
                    UserData = vertex.Value,
                    LabelText = vertex.Value is null ? vertex.Key : string.Join(Environment.NewLine, vertex.Key, vertex.Value.ToString()),
                });
            }
            foreach (var edgeSource in edges)
            {
                foreach (var edgeTargetAndLabel in edgeSource.Value)
                {
                    graph.AddEdge(edgeSource.Key, edgeTargetAndLabel.Value, edgeTargetAndLabel.Key);
                }
            }
            return element.SetGraph(graph);
        }

        /// <summary>
        /// Sets the <see cref="Graph"/> to <paramref name="graph"/> or to a <see langword="new"/> <see cref="Graph"/> is <see langword="null"/> is passed.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="graph">The <see cref="Graph"/> to be set or <see langword="null"/> to create a <see langword="new"/> one.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        private static Graph SetGraph(this AutomaticGraphLayoutControl element, Graph graph = null)
        {
            var oldGraph = element.Graph;
            element.Graph = graph ?? new Graph();
            return oldGraph;
        }

        /// <summary>
        /// Creates a probability <see cref="Matrix"/>. Rows represent source nodes, columns represent target nodes, cells represent probabilities.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="indexes">Maps nodes to indexes of the <see cref="Matrix"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        public static Matrix ToMatrix(this AutomaticGraphLayoutControl element, string root, out IDictionary<string, int> indexes)
        {
            var vertices = element.GetVertices().OrderBy(vertex => vertex.Key).ToArray();
            var edges = element.GetEdges();
            indexes = vertices.Select((vertex, index) => new KeyValuePair<string, int>(vertex.Key, index)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var rootIndex = indexes[root];

            return new Matrix(vertices.Length, vertices.Length + 1, (row, column) =>
            {
                if (column >= vertices.Length) return 1;
                if (row == rootIndex && column == rootIndex) return 1;
                if (edges.TryGetValue(vertices[row].Key, out var targetEdges))
                {
                    if (targetEdges.TryGetValue(vertices[column].Key, out var targetEdge))
                    {
                        return string.IsNullOrEmpty(targetEdge) ? 1 : Fraction.Parse(targetEdge);
                    }
                }
                return 0;
            });
        }
        /*
        public static ProbabilityChain ToProbabilityChain(this AutomaticGraphLayoutControl element, string mainVertex)
        {
            var edges = element.GetEdges();
            var vertices = element.GetVertices();
            var fractions = vertices.Select(vertex =>
            {
                var targets = edges.TryGetValue(vertex.Key, out var dictionary) ? dictionary : new Dictionary<string, string>(0);
                var sum = targets.Sum(kvp => Fraction.Parse(kvp.Value));
                if (!sum.IsOne && !sum.IsZero) throw new ArithmeticException(nameof(sum));

            });
        }
        */
        /// <summary>
        /// Modifies the probabilites of edges.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="matrix">The pobability <see cref="Matrix"/>. Rows correspond to source vertices.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        /// <exception cref="ArgumentException">Invalid <paramref name="matrix"/> size.</exception>
        public static Graph SetProbabilities(this AutomaticGraphLayoutControl element, Matrix matrix)
        {
            var vertices = element.GetVertices();
            var sortedVertices = vertices.OrderBy(vertex => vertex.Key).ToArray();
            var edges = element.GetEdges();
            if (!matrix.IsSquare || matrix.Rows != vertices.Count)
            {
                throw new ArgumentException(nameof(matrix));
            }
            for (var row = 0; row < matrix.Rows; row++)
            {
                for (var column = 0; column < matrix.Columns; column++)
                {
                    var probability = matrix[row, column];
                    var source = sortedVertices[row].Key;
                    var target = sortedVertices[column].Key;
                    var targetEdges = edges[source];
                    if (probability.IsZero)
                    {
                        if (!targetEdges.Remove(target))
                        {
                            throw new KeyNotFoundException(target);
                        }
                    }
                    else
                    {
                        if (probability.IsNegative) throw new ConstraintException(nameof(probability.IsNegative));
                        else targetEdges[target] = probability.ToString();
                    }
                }
            }
            return element.SetVerticesAndEdges(vertices, edges);
        }

    }
}
