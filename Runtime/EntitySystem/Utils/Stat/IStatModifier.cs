namespace SpookyCore.EntitySystem.Utils.Stat
{
    public interface IStatModifier
    {
        void Apply(ref float value);
    }
}