using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace woodenfortifications
{
    public class Block_ArchersStake : Block
    {
        private int _damage;
        private bool _damageFromSide;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            _damage = Attributes["damage"].AsInt(2);
            _damageFromSide = Attributes["damageFromSide"].AsBool(true);
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            var blockEntity = world.BlockAccessor.GetBlockEntity(pos) as BlockEntity_Spike;
            var stacks = base.GetDrops(world, pos, byPlayer, dropQuantityMultiplier);

            if (blockEntity == null)
            {
                stacks[0].Attributes.SetInt("health", Attributes["hit_points"].AsInt(25));
                return stacks;
            }
            
            if (blockEntity.Health <= 0)
                return Array.Empty<ItemStack>();

            stacks[0].Attributes.SetInt("health", blockEntity.Health);
            return stacks;
        }

        public override void OnEntityCollide(IWorldAccessor world, Entity entity, BlockPos pos, BlockFacing facing, Vec3d collideSpeed, bool isImpact)
        {
            if (!isImpact && entity is EntityPlayer) return;

            var blockDirection = BlockFacing.FromCode(LastCodePart());

            bool shouldDamage = facing == BlockFacing.UP;
            if (_damageFromSide && facing != blockDirection.Opposite)
            {
                shouldDamage = true;
            }
            
            if (shouldDamage)
            {
                if (world.Side == EnumAppSide.Server)
                {
                    var damaged = entity.ReceiveDamage(new DamageSource 
                        {
                            Source = EnumDamageSource.Block, SourceBlock = this,
                            Type = EnumDamageType.PiercingAttack,
                            SourcePos = pos.ToVec3d()
                        }, _damage);

                    if (damaged)
                    {
                        (world.BlockAccessor.GetBlockEntity(pos) as BlockEntity_Spike)?.TakeDamage(1);
                    }
                }
            }
        }
    }
}