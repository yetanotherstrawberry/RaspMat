using QuikGraph;
using System;
using System.Linq;

namespace RaspMat.Models
{
    [Serializable]
    internal class DirectedGraph : BidirectionalGraph<Vertex, DirectedEdge>
    {

        public void AddEdge(Vertex source, Vertex target)
        {
            base.AddEdge(new DirectedEdge(source, target, 1));
        }

        public void AddVertex(string name)
        {
            Vertex vertex;
            do vertex = new Vertex(name);
            while (ContainsVertex(vertex));
            if (!base.AddVertex(vertex)) throw new InvalidOperationException(nameof(name));
        }

        public void RemoveEdge(Vertex source, Vertex target)
        {
            if (TryGetEdge(source, target, out DirectedEdge edge)) base.RemoveEdge(edge);
            else throw new InvalidOperationException(nameof(source));
        }

        public new void RemoveVertex(Vertex vertex)
        {
            if (!base.RemoveVertex(vertex)) throw new InvalidOperationException(nameof(vertex));
        }

        public Fraction[][] ToFractionMatrix()
        {
            var column = 0;
            var guidToInt = Vertices.ToDictionary(vertex => vertex.Id, vertex => column++);
            var matrix = Enumerable.Range(0, column).Select(index => new Fraction[column]).ToArray();

            foreach (var edge in Edges)
            {
                matrix[guidToInt[edge.Source.Id]][guidToInt[edge.Target.Id]] = edge.Weight;
            }

            return matrix;
        }

    }
}
