using Vintagestory.API.Common;

namespace woodenfortifications
{
    public class WoodenFortifications : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockClass("Block_ArchersStake", typeof(Block_ArchersStake));
            
            api.RegisterBlockEntityClass("BlockEntity_Spike", typeof(BlockEntity_Spike));
        }
    }
}