using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

namespace woodenfortifications;

public class WoodenFortificationsModSystem : ModSystem
{
    // Called on server and client
    // Useful for registering block/entity classes on both sides
    public override void Start(ICoreAPI api)
    {
        api.Logger.Notification("Hello from template mod: " + api.Side);
        
        api.RegisterBlockClass(nameof(Block_ArchersStake), typeof(Block_ArchersStake));
        api.RegisterBlockEntityClass(nameof(BlockEntity_Spike), typeof(BlockEntity_Spike));
            
        api.RegisterBlockClass(nameof(Block_StackablePalisade), typeof(Block_StackablePalisade));
        api.RegisterBlockEntityClass(nameof(BlockEntity_StackablePalisade), typeof(BlockEntity_StackablePalisade));
        api.RegisterBlockBehaviorClass(nameof(Behaviour_PalisadeAttachment), typeof(Behaviour_PalisadeAttachment));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Logger.Notification("Hello from template mod server side: " + Lang.Get("woodenfortifications:hello"));
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Logger.Notification("Hello from template mod client side: " + Lang.Get("woodenfortifications:hello"));
    }
}