using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace woodenfortifications;

public class Behaviour_PalisadeAttachment : BlockBehaviorHorizontalAttachable
{
    public Behaviour_PalisadeAttachment(Block block) : base(block) { }

    // always drop the "near" reach variant, regardless of which reach variant broke
    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropChanceMultiplier, ref EnumHandling handling)
    {
        string reach = block.Variant["reach"];
        if (reach == null || reach == "near") return null;

        Block nearBlock = world.BlockAccessor.GetBlock(block.CodeWithVariant("reach", "near"));
        if (nearBlock == null) return null;

        handling = EnumHandling.PreventDefault;
        return new ItemStack[] { new ItemStack(nearBlock) };
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
