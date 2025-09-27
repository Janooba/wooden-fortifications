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
            if (!IsSupported(world, pos)) world.BlockAccessor.BreakBlock(pos, null);

            base.OnNeighbourBlockChange(world, pos, neibpos);
        }

        private bool IsSupported(IWorldAccessor world, BlockPos pos)
        {
            Block belowBlock = world.BlockAccessor.GetBlock(pos.DownCopy());
            Block aboveBlock = world.BlockAccessor.GetBlock(pos.UpCopy());
            Block northBlock = world.BlockAccessor.GetBlock(pos.NorthCopy());
            Block southBlock = world.BlockAccessor.GetBlock(pos.SouthCopy());
            Block eastBlock = world.BlockAccessor.GetBlock(pos.EastCopy());
            Block westBlock = world.BlockAccessor.GetBlock(pos.WestCopy());
            
            if (belowBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, BlockFacing.UP) ||
                belowBlock.Code.BeginsWith(WoodenFortificationsModSystem.NAMESPACE, "palisadewall"))
            {
                api.Logger.Debug("Block is supported by the block below");
                return true;
            }
            
            var frontFacing = BlockFacing.FromCode(LastCodePart());
            if (frontFacing == null)
            {
                // If no facing is defined, check all sides for anything that can support the block
                return (aboveBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, BlockFacing.DOWN) ||
                        southBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, BlockFacing.NORTH) ||
                        northBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, BlockFacing.SOUTH) ||
                        westBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, BlockFacing.EAST) ||
                        eastBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, BlockFacing.WEST));
            }
            
            var leftFacing = frontFacing.GetCCW();
            var rightFacing = frontFacing.GetCW();
            
            // Check if front block is solid
            var frontPos = pos.AddCopy(frontFacing);
            Block frontBlock = world.BlockAccessor.GetBlock(frontPos);
            if (frontBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, frontFacing.Opposite))
                return true;
            
            // Check if left and right blocks are solid
            var leftPos = pos.AddCopy(leftFacing);
            Block leftBlock = world.BlockAccessor.GetBlock(leftPos);
            if (leftBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, leftFacing.Opposite) ||
                leftBlock.Code.BeginsWith(WoodenFortificationsModSystem.NAMESPACE, "palisadewall"))
                return true;
            
            var rightPos = pos.AddCopy(rightFacing);
            Block rightBlock = world.BlockAccessor.GetBlock(rightPos);
            if (rightBlock.CanAttachBlockAt(world.BlockAccessor, this, pos, rightFacing.Opposite) ||
                rightBlock.Code.BeginsWith(WoodenFortificationsModSystem.NAMESPACE, "palisadewall"))
                return true;

            return false;
        }
    }   
}