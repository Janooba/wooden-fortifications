using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace woodenfortifications
{
    public class Block_StackablePalisade : Block
    {
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BlockEntity be = world.BlockAccessor.GetBlockEntity(blockSel.Position);

            if (be is BlockEntity_StackablePalisade stackableBlock)
            {
                return stackableBlock.OnPlayerInteract(byPlayer);
            }

            return false;
        }
        
        public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
        {
            if (neibpos.Y < pos.Y)
            {
                Block belowBlock = world.BlockAccessor.GetBlock(pos.DownCopy());
                if (!belowBlock.CanAttachBlockAt(world.BlockAccessor, this, pos.DownCopy(), BlockFacing.UP))
                {
                    world.BlockAccessor.BreakBlock(pos, null);
                    return;
                }
            }

            base.OnNeighbourBlockChange(world, pos, neibpos);
        }
    }   
}