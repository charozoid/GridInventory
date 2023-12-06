﻿using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;
class GridInventory
{
    public static Inventory inv = new Inventory();
    public static void Main(string[] args)
    {
        Weapon.LoadCompatibleAttachments();
        Graphics.window.SetVerticalSyncEnabled(true);
        Graphics.window.Closed += (sender, args) => Graphics.window.Close();
        Graphics.window.MouseButtonPressed += inv.ButtonPressed;
        Graphics.window.MouseButtonReleased += inv.ButtonReleased;
        Graphics.window.KeyPressed += EventHandler;

        while (Graphics.window.IsOpen)
        {
            Graphics.window.Clear(Color.Black);
            
            Graphics.DrawGrid();
            //Graphics.DrawItemSquare();
            inv.Think();
            Graphics.DrawItems();
            
            Graphics.window.DispatchEvents();
            Graphics.window.Display();
        }
    }
    public static void EventHandler(object sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case (Keyboard.Key.Space):
                Weapon ak47 = new Weapon();
                ak47.IntRect = new IntRect(0, 0, 192, 64);
                ak47.size = new Vector2i(12, 3);
                ak47.strRef = "weapon_ak47";
                inv.AddItem(ak47);
                break;
            case (Keyboard.Key.Q):
                Weapon glock = new Weapon();
                glock.IntRect = new IntRect(192, 0, 64, 32);
                glock.size = new Vector2i(3, 2);
                glock.strRef = "weapon_glock";
                inv.AddItem(glock);
                break;
            case (Keyboard.Key.E):
                Magazine akMag = new Magazine();
                akMag.IntRect = new IntRect(32, 64, 32, 32);
                akMag.size = new Vector2i(2, 2);
                akMag.strRef = "mag_ak47";
                akMag.resizeFactor = new Vector2i(0, 1);
                akMag.spriteOffset = new Vector2f(142, 48);
                inv.AddItem(akMag);
                break;
            case (Keyboard.Key.R):
                Magazine glockMag = new Magazine();
                glockMag.IntRect = new IntRect(64, 64, 16, 32);
                glockMag.size = new Vector2i(1, 2);
                glockMag.strRef = "mag_glock";
                glockMag.resizeFactor = new Vector2i(0, 0);
                glockMag.spriteOffset = new Vector2f(0, 0);
                inv.AddItem(glockMag);
                break;
        }
    }
}
