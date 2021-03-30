using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using RoR2;




namespace FasterBossWait
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.Chris-Stetvenson-Git.FasterBossWait", "Faster Boss Wait", "1.0.1")]
    public class FasterBossWait : BaseUnityPlugin
    {

        public static ConfigEntry<float> PercentIncrease { get; set; }

        public void Awake()
        {

            this.initConfig();

            bool zone = false;

            bool hasTeleporter = false;

            float chargedKill = PercentIncrease.Value * 0.01f;

            On.RoR2.SceneDirector.PlaceTeleporter += (orig, self) =>
            {
                hasTeleporter = true;
                orig(self);
            };
            On.RoR2.BossGroup.OnDefeatedServer += (orig, self) =>
            {
                zone = true;
                orig(self);
            };
            On.RoR2.HoldoutZoneController.OnDisable += (orig, self) =>
            {
                zone = false;
                orig(self);
            };


            On.RoR2.CharacterBody.OnDeathStart += (orig, self) =>
            {
                if (zone && hasTeleporter)
                {
                    float chargeVal = (float)typeof(HoldoutZoneController).GetProperty("charge").GetValue(TeleporterInteraction.instance.holdoutZoneController);

                    float newCharge;

                    if(chargedKill + chargeVal >= 1f)
                    {
                        newCharge = 1f;
                    } else
                    {
                        newCharge = chargeVal + chargedKill;
                    }
                        
                    
                    typeof(HoldoutZoneController).GetProperty("charge").SetValue(TeleporterInteraction.instance.holdoutZoneController, newCharge);
                    
                }
                  

                orig(self);
            };
        }

        private void initConfig()
        {
            PercentIncrease = Config.Bind<float>(
                "Increase Value",
                "Percent",
                1f,
                "How many percent the teleporter should charge for each kill after the boss has been defeated."
                );
        }
    }
}
