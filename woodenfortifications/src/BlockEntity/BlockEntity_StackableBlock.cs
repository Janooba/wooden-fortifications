using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace woodenfortifications
{
    public class BlockEntity_StackableBlock : BlockEntity
    {
        public virtual bool OnPlayerInteract(IPlayer byPlayer)
        {
            // Recursively interact upwards till it hits the top of the stack
            BlockPos abovePos = Pos.UpCopy();
            BlockEntity aboveEntity = Api.World.BlockAccessor.GetBlockEntity(abovePos);
            if (aboveEntity is BlockEntity_StackableBlock stackableBlock)
            {
                return stackableBlock.OnPlayerInteract(byPlayer);
            }
            
            // Is held item equal to the placed item being interacted with?
            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;
            Block hotbarBlock = hotbarSlot.Itemstack.Block;
            
            bool equalStack = hotbarBlock != null && hotbarBlock.CodeWithoutParts(10) == Block.CodeWithoutParts(10);

            if (!equalStack) return false;
            
            Block thisBlock = Api.World.BlockAccessor.GetBlock(Pos);
            Block aboveBlock = Api.World.BlockAccessor.GetBlock(abovePos);

            if (!aboveBlock.IsReplacableBy(thisBlock)) return false;
            
            if (Api.World is IServerWorldAccessor)
            {
                string variant = Block.CodeEndWithoutParts(1);

                var newBlock = Api.Assets.Get<Block>(new AssetLocation("woodenfortifications", hotbarBlock.CodeWithoutParts(10) +"-"+ variant));
                
                Api.World.BlockAccessor.SetBlock((ushort)newBlock.Id, abovePos);

                hotbarSlot.Itemstack.StackSize--;
            }
            return true;
        }
    }
}