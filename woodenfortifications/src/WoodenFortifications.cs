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
            
            // api.RegisterBlockClass("Block_StackablePalisade", typeof(Block_StackablePalisade));
            // api.RegisterBlockEntityClass("BlockEntity_StackableBlock", typeof(BlockEntity_StackableBlock));
        }
    }
}