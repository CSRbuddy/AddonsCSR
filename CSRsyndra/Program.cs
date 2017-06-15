using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using static CSRsyndra.Menus;
using CSRsyndra.Managers;
using EloBuddy.SDK.Menu.Values;

namespace CSRsyndra
{
    internal class Loader
    {


        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs bla)
        {
            if (Player.Instance.Hero != Champion.Syndra) return;
            SpellsManager.InitializeSpells();
            Menus.CreateMenu();
            ModeManager.InitializeModes();
            DrawingsManager.InitializeDrawings();
            EventsManager.Initialize();
            ItemManager.Initialize();
            YasuoWallManager.Initialize();
            DamageIndicator.Init();


            Chat.Print("<font color='#FA5858'>CSRsyndra loaded</font>");
            Chat.Print("Credits to wladis");
        }
    }
}