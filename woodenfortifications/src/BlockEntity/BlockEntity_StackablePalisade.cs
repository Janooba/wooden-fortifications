using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace woodenfortifications
{
    public class BlockEntity_StackablePalisade : BlockEntity
    {
        public virtual bool OnPlayerInteract(IPlayer byPlayer)
        {
            string baseCode = Block.CodeWithoutParts(10);
            
            // Get base block
            BlockEntity baseBlock = GetBaseBlock(Pos, baseCode);
            
            // Is held item equal to the base item being interacted with?
            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;
            
            if (hotbarSlot.Empty) return false;
            
            Block hotbarBlock = hotbarSlot.Itemstack.Block;
            bool equalStack = hotbarBlock != null && hotbarBlock.CodeWithoutParts(10) == baseBlock.Block.CodeWithoutParts(10);
            
            if (equalStack)
            {
                IncreaseStack(baseCode, hotbarSlot, byPlayer, baseBlock);
                return true;
            }
            
            return false;
        }

        private bool IncreaseStack(string baseCode, ItemSlot hotbarSlot, IPlayer byPlayer, BlockEntity baseBlock)
        {
            BlockEntity topBlock = GetTopBlock(Pos, baseCode);
            BlockPos aboveTopPos = topBlock.Pos.UpCopy();
            BlockEntity aboveBlock = Api.World.BlockAccessor.GetBlockEntity(aboveTopPos);

            if (topBlock.Pos.Y - baseBlock.Pos.Y >= 4) return false;
            
            // Check if the block above is replaceable by the block being placed
            if (aboveBlock != null && !aboveBlock.Block.IsReplacableBy(Block)) return false;

            if (hotbarSlot.Itemstack == null) return false;

            if (byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                hotbarSlot.TakeOut(1);
                hotbarSlot.OnItemSlotModified(null);
            }
            
            if (Api.World is IServerWorldAccessor)
            {
                // Duplicate base up to top
                BlockPos currPos = baseBlock.Pos;
                while (currPos.Y < aboveTopPos.Y)
                {
                    if (Api.World.BlockAccessor.GetBlock(currPos).Code != baseBlock.Block.Code)
                        Api.World.BlockAccessor.SetBlock(baseBlock.Block.Id, currPos); 
                    
                    currPos = currPos.UpCopy();
                }
                
                // set top
                string variant = Block.CodeEndWithoutParts(10);

                string newCode = "woodenfortifications:" + baseCode + "-top-" + variant;
                
                var newBlock = Api.World.GetBlock(newCode);
                Api.World.BlockAccessor.SetBlock((ushort)newBlock.Id, currPos);
            }

            return true;
        }
        
        private BlockEntity GetBaseBlock(BlockPos pos, string blockBaseCode)
        {
            BlockEntity baseBlock = Api.World.BlockAccessor.GetBlockEntity(pos);
            while (true)
            {
                pos = pos.DownCopy();
                var nextBlock = Api.World.BlockAccessor.GetBlockEntity(pos);

                if (nextBlock == null)
                    break;

                if (nextBlock.Block.CodeWithoutParts(10) == blockBaseCode)
                    baseBlock = nextBlock;
                else
                    break;
            }

            return baseBlock;
        }

        private BlockEntity GetTopBlock(BlockPos pos, string blockBaseCode)
        {
            BlockEntity topBlock = Api.World.BlockAccessor.GetBlockEntity(pos);
            while (true)
            {
                pos = pos.UpCopy();
                var nextBlock = Api.World.BlockAccessor.GetBlockEntity(pos);

                if (nextBlock == null)
                    break;
                
                if (nextBlock.Block.CodeWithoutParts(10) == blockBaseCode)
                    topBlock = nextBlock;
                else
                    break;
            }

            return topBlock;
        }
    }
}