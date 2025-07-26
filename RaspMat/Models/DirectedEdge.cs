using QuikGraph;

namespace RaspMat.Models
{
    internal readonly struct DirectedEdge : IEdge<Vertex>
    {

        public DirectedEdge(Vertex source, Vertex target, Fraction weight)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }

        public Vertex Source { get; }

        public Vertex Target { get; }

        public Fraction Weight { get; }

        public override bool Equals(object compared) => compared is DirectedEdge directedEdge && this == directedEdge;

        public override int GetHashCode() => Source.GetHashCode() ^ Target.GetHashCode() ^ Weight.GetHashCode();

        public static bool operator ==(DirectedEdge left, DirectedEdge right) => left.Source == right.Source && left.Target == right.Target && left.Weight == right.Weight;

        public static bool operator !=(DirectedEdge left, DirectedEdge right) => !(left == right);

    }
}
