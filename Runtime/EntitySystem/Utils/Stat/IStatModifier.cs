namespace SpookyCore.EntitySystem.Utils.Stat
{
    public interface IStatModifier
    {
        void AddModifier(Modifier modifier);
        bool TryRemoveModifier(Modifier modifier);
        
        void Apply(ref float value);
    }
}