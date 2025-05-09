using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public abstract class BaseScriptedSequence<TSequenceContext> : ScriptedSequence, IScriptedSequence where TSequenceContext : IScriptedSequenceContext
    {
        protected EntitySequenceRunner _runner;
        protected TSequenceContext _ctx;

        public override void Initialize(EntitySequenceRunner runner, BaseSequenceContext ctx)
        {
            if (ctx is TSequenceContext sequenceContext)
            {
                _ctx = sequenceContext;
            }
            else
            {
                Debug.Log($"Wrong context passed for {name}, {typeof(TSequenceContext)} expected, got {ctx.GetType()} instead.");
            }
            _runner = runner;
            
        }
    }
}