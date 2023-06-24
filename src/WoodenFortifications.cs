using Vintagestory.API.Common;

namespace woodenfortifications
{
    public class WoodenFortifications : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockClass("ArchersStake", typeof(ArchersStake));
        }
    }
}