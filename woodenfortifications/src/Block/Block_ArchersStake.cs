using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace woodenfortifications
{
    public class Block_ArchersStake : Block
    {
        private const long HitCooldownMs = 1000;

        // Baseline metal for the damage/health scaling ratio. Must have a "game:spear-generic-{metal}" counterpart.
        private const string BaselineMetal = "copper";

        private static readonly HashSet<string> IndestructibleMetals = new() { "iron", "meteoriciron", "steel" };

        private int _damage;
        private bool _damageFromSide;
        private readonly Dictionary<long, long> _lastHitMsByEntityId = new();

        public int MaxHealth { get; private set; }
        public bool IsIndestructible { get; private set; }

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            int baseDamage = Attributes["damage"].AsInt(2);
            int baseHitPoints = Attributes["hit_points"].AsInt(25);
            _damageFromSide = Attributes["damageFromSide"].AsBool(true);

            float tierRatio = GetMetalTierRatio(api);
            _damage = (int)Math.Round(baseDamage * tierRatio);
            MaxHealth = (int)Math.Round(baseHitPoints * tierRatio);

            string metal = Variant["metal"];
            IsIndestructible = WoodenFortificationsModSystem.Config.DisableSpikeDurability
                || (metal != null && IndestructibleMetals.Contains(metal));
        }

        // scales from vanilla spear power
        private float GetMetalTierRatio(ICoreAPI api)
        {
            string metal = Variant["metal"];
            if (metal == null || metal == BaselineMetal) return 1f;

            Item baselineSpear = api.World.GetItem(new AssetLocation("game", "spear-generic-" + BaselineMetal));
            Item metalSpear = api.World.GetItem(new AssetLocation("game", "spear-generic-" + metal));

            if (baselineSpear == null || metalSpear == null || baselineSpear.AttackPower <= 0) return 1f;

            return metalSpear.AttackPower / baselineSpear.AttackPower;
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            var blockEntity = world.BlockAccessor.GetBlockEntity(pos) as BlockEntity_Spike;
            var stacks = base.GetDrops(world, pos, byPlayer, dropQuantityMultiplier);

            if (blockEntity == null)
            {
                stacks[0].Attributes.SetInt("health", MaxHealth);
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
            if (world.Side != EnumAppSide.Server) return;
            if (entity.Properties.Weight < WoodenFortificationsModSystem.Config.MinDamageWeight) return;

            var blockDirection = BlockFacing.FromCode(LastCodePart());

            bool shouldDamage = facing == BlockFacing.UP;
            if (_damageFromSide && facing != blockDirection)
            {
                shouldDamage = true;
            }

            if (!shouldDamage) return;

            long nowMs = world.ElapsedMilliseconds;
            if (_lastHitMsByEntityId.TryGetValue(entity.EntityId, out long lastHitMs) && nowMs - lastHitMs < HitCooldownMs) return;
            _lastHitMsByEntityId[entity.EntityId] = nowMs;

            var damaged = entity.ReceiveDamage(new DamageSource
                {
                    Source = EnumDamageSource.Block, SourceBlock = this,
                    Type = EnumDamageType.PiercingAttack,
                    SourcePos = pos.ToVec3d(),
                    IgnoreInvFrames = !(entity is EntityPlayer)
                }, _damage);

            if (damaged && !IsIndestructible)
            {
                (world.BlockAccessor.GetBlockEntity(pos) as BlockEntity_Spike)?.TakeDamage(1);
            }
        }
    }
}