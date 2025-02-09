using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace woodenfortifications
{
    public class BlockEntity_StackablePalisade : BlockEntity
    {
        private const string TOP_CODE = "top";
        private const string MID_CODE = "base";
        
        public virtual bool OnPlayerInteract(IPlayer byPlayer)
        {
            string baseCode = Block.FirstCodePart();
            
            // Is held item equal to the base item being interacted with?
            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;
            
            if (hotbarSlot.Empty) return false;
            
            Block hotbarBlock = hotbarSlot.Itemstack.Block;
            if (hotbarBlock == null) return false;
            
            bool equalStack = hotbarBlock.FirstCodePart() == Block.FirstCodePart();
            
            if (equalStack)
            {
                IncreaseStack(baseCode, hotbarSlot, byPlayer);
                return true;
            }
            
            return false;
        }

        private bool IncreaseStack(string baseCode, ItemSlot hotbarSlot, IPlayer byPlayer)
        {
            BlockPos basePos = GetBaseBlockPos(Pos);
            BlockPos topPos = GetTopBlockPos(Pos);
            
            BlockPos aboveTopPos = topPos.UpCopy();
            BlockEntity aboveBlock = Api.World.BlockAccessor.GetBlockEntity(aboveTopPos);

            if (topPos.Y - basePos.Y >= WoodenFortificationsModSystem.Config.MaxPalisadeHeight - 1) return false;
            
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
                string variant = $"-{Block.LastCodePart()}";
                
                // If there is no rotation code, dont add it to the variant
                if (variant == $"-{MID_CODE}") variant = "";
                if (variant == $"-{TOP_CODE}") variant = "";
                
                string midCode = $"woodenfortifications:{baseCode}-{MID_CODE}{variant}";
                Block midBlockType = Api.World.GetBlock(new AssetLocation(midCode));
                
                string topCode = $"woodenfortifications:{baseCode}-{TOP_CODE}{variant}";
                Block topBlockType = Api.World.GetBlock(new AssetLocation(topCode));
                
                BlockPos currPos = basePos.Copy();
                while (currPos.Y < aboveTopPos.Y)
                {
                    if (Api.World.BlockAccessor.GetBlock(currPos).Code != midCode)
                        Api.World.BlockAccessor.SetBlock(midBlockType.Id, currPos); 
                    
                    currPos = currPos.UpCopy();
                }
                
                // set top
                Api.World.BlockAccessor.SetBlock(topBlockType.Id, currPos);
            }
            else
            {
                Api.World.PlaySoundAt(Block.Sounds.Place, Pos.X + 0.5, Pos.InternalY, Pos.Z + 0.5, null, 0.88f + (float)Api.World.Rand.NextDouble() * 0.24f, 16);   
            }

            return true;
        }
        
        private BlockPos GetBaseBlockPos(BlockPos pos)
        {
            BlockPos baseBlock = pos.Copy();
            while (true)
            {
                pos = pos.DownCopy();
                var nextBlock = Api.World.BlockAccessor.GetBlockEntity(pos);

                if (nextBlock == null)
                    break;

                if (nextBlock.Block.FirstCodePart() == Block.FirstCodePart())
                    baseBlock = nextBlock.Pos;
                else
                    break;
            }

            return baseBlock;
        }

        private BlockPos GetTopBlockPos(BlockPos pos)
        {
            BlockPos topBlock = pos.Copy();
            while (true)
            {
                pos = pos.UpCopy();
                var nextBlock = Api.World.BlockAccessor.GetBlockEntity(pos);

                if (nextBlock == null)
                    break;
                
                if (nextBlock.Block.FirstCodePart() == Block.FirstCodePart())
                    topBlock = nextBlock.Pos;
                else
                    break;
            }

            return topBlock;
        }
    }
}