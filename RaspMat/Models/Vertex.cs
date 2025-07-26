using System;

namespace RaspMat.Models
{
    internal class Vertex
    {

        public Guid Id { get; }

        public string Name { get; }

        public Vertex(string name) : this(Guid.NewGuid(), name) { }

        public Vertex(Guid id, string name)
        {
            Id = default == id ? throw new ArgumentException(nameof(id)) : id;
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException(nameof(name)) : name;
        }

        public override string ToString() => Name;

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object compared) => compared is Vertex vertex && this == vertex;

        public static bool operator ==(Vertex left, Vertex right) => left.Id == right.Id;

        public static bool operator !=(Vertex left, Vertex right) => !(left == right);

    }
}
