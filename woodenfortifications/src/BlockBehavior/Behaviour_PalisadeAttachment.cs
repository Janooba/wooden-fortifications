using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace woodenfortifications;

public class Behaviour_PalisadeAttachment : BlockBehaviorHorizontalAttachable
{
    Dictionary<string, Cuboidi> attachmentAreas;
    
    public Behaviour_PalisadeAttachment(Block block) : base(block) { }
    
    public BlockFacing GetAttachedBlockFace(Block aimedBlock)
    {
        return BlockFacing.FromCode(aimedBlock.LastCodePart());
    }

    public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handled)
    {
        handled = EnumHandling.PreventDefault;

        if (!CanBlockStay(world, pos))
        {
            world.BlockAccessor.BreakBlock(pos, null);
        }
    }

    public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel,
        ref EnumHandling handling, ref string failureCode)
    {
        handling = EnumHandling.PreventDefault;

        if (byPlayer.CurrentBlockSelection.Block.FirstCodePart() != "palisadewall")
            return false;
        
        // Prefer selected block face
        if (blockSel.Face.IsHorizontal)
        {
            if (TryAttachTo(world, byPlayer, blockSel, itemstack, ref failureCode)) return true;
        }

        failureCode = "requirehorizontalattachable";

        return false;
    }
    
    protected bool TryAttachTo(IWorldAccessor world, IPlayer player, BlockSelection blockSel, ItemStack itemstack, ref string failureCode)
    {
        BlockFacing oppositeFace = blockSel.Face.Opposite;
        BlockPos blockToAttachToPos = blockSel.Position.AddCopy(oppositeFace);
        
        BlockFacing attachmentFace = GetAttachedBlockFace(player.CurrentBlockSelection.Block);
        BlockPos attachingBlockPos = blockToAttachToPos.AddCopy(attachmentFace);
        
        Block orientedBlock = world.BlockAccessor.GetBlock(block.CodeWithParts(attachmentFace.Opposite.Code));

        var modifiedSelection = blockSel.Clone();
        modifiedSelection.Position = attachingBlockPos;
        if (world.BlockAccessor.GetBlock(attachingBlockPos).IsReplacableBy(block))
        {
            orientedBlock.DoPlaceBlock(world, player, modifiedSelection, itemstack);
            return true;
        }

        return false;
    }

    protected bool CanBlockStay(IWorldAccessor world, BlockPos pos)
    {
        BlockFacing facing = BlockFacing.FromCode(block.LastCodePart());
        Block attachingblock = world.BlockAccessor.GetBlock(pos.AddCopy(facing));

        return attachingblock.FirstCodePart() == "palisadewall";
    }
}