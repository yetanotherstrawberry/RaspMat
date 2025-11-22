using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Extensions for <see cref="AutomaticGraphLayoutControl"/>.
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
        /// <exception cref="KeyNotFoundException">Element does not exist.</exception>
        public static Graph RemoveEdge(this AutomaticGraphLayoutControl element, string source, string target)
        {
            if (element.Graph is null) element.SetGraph();
            var oldEdges = element.GetEdges();
            var newEdges = oldEdges.Where(edge => !string.Equals(edge.Key, source) && !string.Equals(edge.Value.Key, target)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (oldEdges.Count == newEdges.Count) throw new KeyNotFoundException();
            return element.SetVerticesAndEdges(element.GetVertices(), newEdges);
        }

        /// <summary>
        /// Creates an edge.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="source">The starting vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="label">The attatched label.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        public static Graph AddEdge(this AutomaticGraphLayoutControl element, string source, string target, string label = null)
        {
            if (element.Graph is null) element.SetGraph();
            var edges = element.GetEdges();
            edges.Add(source, new KeyValuePair<string, string>(target, label));
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
            var oldVertices = element.GetVertices();
            var newVertices = oldVertices.Where(vertex => !string.Equals(vertex.Key, name)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (oldVertices.Count == newVertices.Count) throw new KeyNotFoundException();
            return element.SetVerticesAndEdges(newVertices, element.GetEdges());
        }

        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="name">Name of the vertex.</param>
        /// <param name="label">Label of the vertex.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        public static Graph AddVertex(this AutomaticGraphLayoutControl element, string name, string label = null)
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
        /// <returns>Edges with starting (<see cref="IDictionary{TKey, TValue}.Keys"/>) vertices, target (<see cref="KeyValuePair{TKey, TValue}.Key"/>) vertices and labels.</returns>
        private static IDictionary<string, KeyValuePair<string, string>> GetEdges(this AutomaticGraphLayoutControl element)
        {
            return element.Graph.Edges.ToDictionary(edge => edge.Source, edge => new KeyValuePair<string, string>(edge.Target, edge.LabelText));
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to use.</param>
        /// <returns>Vertices (<see cref="IDictionary{TKey, TValue}.Keys"/>) and labels.</returns>
        private static IDictionary<string, object> GetVertices(this AutomaticGraphLayoutControl element)
        {
            return element.Graph.Nodes.ToDictionary(node => node.Id, node => node.UserData);
        }

        /// <summary>
        /// Sets vertices and edges of the <see cref="Graph"/>.
        /// </summary>
        /// <param name="element">The element with <see cref="AutomaticGraphLayoutControl.Graph"/> to modify.</param>
        /// <param name="vertices">Vertices (<see cref="IDictionary{TKey, TValue}.Keys"/>) and labels.</param>
        /// <param name="edges">Edges with starting (<see cref="IDictionary{TKey, TValue}.Keys"/>) vertices, target (<see cref="KeyValuePair{TKey, TValue}.Key"/>) vertices and labels.</param>
        /// <returns>The <see cref="Graph"/> before modifications.</returns>
        public static Graph SetVerticesAndEdges(this AutomaticGraphLayoutControl element, IDictionary<string, object> vertices, IDictionary<string, KeyValuePair<string, string>> edges)
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
            foreach (var edge in edges)
            {
                graph.AddEdge(edge.Key, edge.Value.Value, edge.Value.Key);
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

    }
}
