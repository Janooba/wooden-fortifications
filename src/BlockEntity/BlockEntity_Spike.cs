using Vintagestory.API.Common;

namespace woodenfortifications
{
    public class BlockEntity_Spike : BlockEntity
    {
        private int _damageToDeal = 2;
        private int _hitPoints = 25;
        private int _remainingHits;

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            _remainingHits = _hitPoints;
        }
    }   
}