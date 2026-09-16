using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace woodenfortifications;

public class Behaviour_PalisadeAttachment : BlockBehaviorHorizontalAttachable
{
    public Behaviour_PalisadeAttachment(Block block) : base(block) { }

    // always drop the "near" reach variant facing "north", regardless of the broken block's reach and side
    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropChanceMultiplier, ref EnumHandling handling)
    {
        string reach = block.Variant["reach"];
        string side = block.Variant["side"];
        if (reach == null || side == null) return null;
        if (reach == "near" && side == "north") return null;

        Block dropBlock = world.BlockAccessor.GetBlock(block.CodeWithVariants(new Dictionary<string, string> {
            { "reach", "near" },
            { "side", "north" }
        }));
        if (dropBlock == null) return null;

        handling = EnumHandling.PreventDefault;
        return new ItemStack[] { new ItemStack(dropBlock) };
    }

    // use the "far" reach variant on the inner side of the wall
    public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ref EnumHandling handling)
    {
        base.OnBlockPlaced(world, blockPos, ref handling);

        string side = block.Variant["side"];
        string reach = block.Variant["reach"];
        if (side == null || reach == null) return;

        // "side" is the direction toward the wall (matches BlockBehaviorHorizontalAttachable's own CanBlockStay).
        BlockFacing towardWall = BlockFacing.FromCode(side);
        Block neighbor = world.BlockAccessor.GetBlock(blockPos.AddCopy(towardWall));
        if (neighbor == null) return;

        if (neighbor.FirstCodePart() != "palisadewall" && neighbor.FirstCodePart() != "palisadecorner") return;

        string wallSide = neighbor.Variant["side"];
        if (wallSide == null) return;

        bool isFar = BlockFacing.FromCode(wallSide) == towardWall;
        string correctReach = isFar ? "far" : "near";
        if (reach == correctReach) return;

        Block corrected = world.BlockAccessor.GetBlock(block.CodeWithVariant("reach", correctReach));
        if (corrected != null)
        {
            world.BlockAccessor.ExchangeBlock(corrected.Id, blockPos);
        }
    }
}
