using System;

namespace SpookyCore.Runtime.EntitySystem.Utils.Stat
{
    public readonly struct Modifier : IEquatable<Modifier>
    {
        public ModifierType Type { get;}
        public float Value { get; }
        public object Source { get; }

        public Modifier(ModifierType type, float value, object source)
        {
            Type = type;
            Value = value;
            Source = source;
        }
        
        public bool Equals(Modifier other)
        {
            return Type == other.Type && Equals(Source, other.Source) && Value.Equals(other.Value);
        }
        public override bool Equals(object obj)
        {
            return obj is Modifier modifier && Equals(modifier);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Source, Value);
        }

        public static bool operator ==(Modifier left, Modifier right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Modifier left, Modifier right)
        {
            return !left.Equals(right);
        }

        public void Deconstruct(out ModifierType type, out float value, out object source)
        {
            value = Value;
            type = Type;
            source = Source;
        }
    }
}