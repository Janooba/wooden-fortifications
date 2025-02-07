using System;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

namespace woodenfortifications;

public class WoodenFortificationsModSystem : ModSystem
{
    public static WoodenFortificationsConfig Config;
    
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

        TryLoadConfig(api);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Logger.Notification("Hello from template mod server side: " + Lang.Get("woodenfortifications:hello"));
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Logger.Notification("Hello from template mod client side: " + Lang.Get("woodenfortifications:hello"));
    }

    private void TryLoadConfig(ICoreAPI api)
    {
        //It is important to surround the LoadModConfig function in a try-catch. 
        //If loading the file goes wrong, then the 'catch' block is run.
        try
        {
            Config = api.LoadModConfig<WoodenFortificationsConfig>("WoodenFortificationsConfig.json");
            if (Config == null) //if the 'MyConfigData.json' file isn't found...
            {
                Config = new WoodenFortificationsConfig();
            }
            //Save a copy of the mod config.
            api.StoreModConfig<WoodenFortificationsConfig>(Config, "WoodenFortificationsConfig.json");
        }
        catch (Exception e)
        {
            //Couldn't load the mod config... Create a new one with default settings, but don't save it.
            Mod.Logger.Error("Could not load config! Loading default settings instead.");
            Mod.Logger.Error(e);
            Config = new WoodenFortificationsConfig();
        }
    }
}