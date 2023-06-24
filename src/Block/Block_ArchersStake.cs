using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace woodenfortifications
{
    public class ArchersStake : Block
    {

        
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            if (_remainingHits <= 0) return Array.Empty<ItemStack>();
            return base.GetDrops(world, pos, byPlayer, dropQuantityMultiplier);
        }

        public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
        {
            string desc = base.GetPlacedBlockInfo(world, pos, forPlayer);

            desc += $"({_remainingHits}/{_hitPoints})";

            return desc;
        }

        public override void OnEntityCollide(IWorldAccessor world, Entity entity, BlockPos pos, BlockFacing facing, Vec3d collideSpeed, bool isImpact)
        {
            if (!isImpact && entity is EntityPlayer) return;
            
            var blockDirection = BlockFacing.FromCode(LastCodePart());

            if (facing == blockDirection.Opposite || facing == BlockFacing.UP)
            {
                if (world.Side == EnumAppSide.Server)
                {
                    var damaged = entity.ReceiveDamage(new DamageSource 
                        {
                            Source = EnumDamageSource.Block, SourceBlock = this,
                            Type = EnumDamageType.PiercingAttack,
                            SourcePos = pos.ToVec3d()
                        }, _damageToDeal);

                    if (damaged)
                    {
                        DamageThis(world, pos);
                    }
                }
            }
        }

        private void DamageThis(IWorldAccessor world, BlockPos pos)
        {
            _remainingHits--;
            if (_remainingHits <= 0)
                Break(world, pos);
        }
        
        private void Break(IWorldAccessor world, BlockPos pos)
        {
            world.BlockAccessor.BreakBlock(pos, null, 0);  
        }
    }
}