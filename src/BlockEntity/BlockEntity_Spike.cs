using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace woodenfortifications
{
    public class BlockEntity_Spike : BlockEntity
    {
        public int Health { get; private set; }
        private int _maxHealth;

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            
            _maxHealth = Block?.Attributes["hit_points"].AsInt(25) ?? 25;
            if (Health == 0) Health = _maxHealth;
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            dsc.AppendLine($"{Health}/{_maxHealth}");
        }
        
        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetInt("health", Health);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            Health = tree.GetInt("health");
        }

        public override void OnBlockPlaced(ItemStack byItemStack = null)
        {
            base.OnBlockPlaced(byItemStack);

            var clonedItemstack = byItemStack?.Clone();
            if (clonedItemstack == null) return;
            
            Health = clonedItemstack.Attributes.GetInt("health", _maxHealth);
            
            MarkDirty(true);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            MarkDirty(false);
            if (Health <= 0)
                DestroyBlock();
        }

        private void DestroyBlock()
        {
            Api.World.BlockAccessor.BreakBlock(Pos, null, 0);  
        }
    }   
}