using SpookyCore.SystemLoader;

namespace SpookyCore.Utilities.Events
{
    public class ObjectiveUpdateEvent : GameEventContext
    {
        public string VerboseRequirement;
        public int RequiredAmount;

        public ObjectiveUpdateEvent(string requirement, int amount)
        {
            VerboseRequirement = requirement;
            RequiredAmount = amount;
        }

        public ObjectiveUpdateEvent Override(string requirement, int amount)
        {
            VerboseRequirement = requirement;
            RequiredAmount = amount;
            return this;
        }
    }
}