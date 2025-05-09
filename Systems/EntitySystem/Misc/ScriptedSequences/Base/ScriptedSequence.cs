using UnityEngine;

namespace SpookyCore.EntitySystem
{
    //The sole existence of this class is thanks to the fact that Unity doesn't support polymorphic serialization.
    public abstract class ScriptedSequence : ScriptableObject
    {
        [field: SerializeField] public SequenceID SequenceID;
        public virtual ScriptedSequence Clone()
        {
            return Instantiate(this);
        }
        
        public virtual void Initialize(EntitySequenceRunner runner, BaseSequenceContext ctx) { }
        
        public abstract void OnStart();
        public abstract void OnEnd();
        public abstract void OnInterrupted();
        /// <summary>
        /// Returns true if the sequence ends after this update call
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public abstract bool UpdateSequence(float deltaTime);
    }
}