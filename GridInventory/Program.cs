using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;
using static Item;

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
            Graphics.window.Clear(new Color(20, 20, 25));
            
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
                ak47.Size = new Vector2i(12, 3);
                ak47.strRef = "weapon_ak47";
                inv.AddItem(ak47);
                break;
            case (Keyboard.Key.Q):
                Weapon glock = new Weapon();
                glock.IntRect = new IntRect(192, 0, 48, 32);
                glock.Size = new Vector2i(3, 2);
                glock.strRef = "weapon_glock";
                inv.AddItem(glock);
                break;
            case (Keyboard.Key.E):
                Magazine akMag = new Magazine();
                akMag.IntRect = new IntRect(32, 64, 32, 32);
                akMag.Size = new Vector2i(2, 2);
                akMag.strRef = "mag_ak47";
                akMag.attachmentType = AttachmentType.Magazine;
                akMag.resizeFactor = new Vector2i(0, 1);
                akMag.spriteOffset = new Vector2f(142, 48);
                akMag.resizeDirection = ResizeDirection.Bottom;
                inv.AddItem(akMag);
                break;
            case (Keyboard.Key.R):
                Magazine glockMag = new Magazine();
                glockMag.IntRect = new IntRect(64, 64, 16, 32);
                glockMag.Size = new Vector2i(1, 2);
                glockMag.strRef = "mag_glock";
                glockMag.attachmentType = AttachmentType.Magazine;
                glockMag.resizeFactor = new Vector2i(0, 0);
                glockMag.spriteOffset = new Vector2f(0, 0);
                glockMag.hide = true;
                inv.AddItem(glockMag);
                break;
            case (Keyboard.Key.T):
                Attachment aksilencer = new Attachment();
                aksilencer.IntRect = new IntRect(240, 0, 48, 16);
                aksilencer.Size = new Vector2i(3, 1);
                aksilencer.strRef = "silencer_ak47";
                aksilencer.attachmentType = AttachmentType.Muzzle;
                aksilencer.resizeFactor = new Vector2i(3, 0);
                aksilencer.spriteOffset = new Vector2f(-80, 25);
                aksilencer.resizeDirection = ResizeDirection.Left;
                inv.AddItem(aksilencer);
                break;
            case (Keyboard.Key.Y):
                Attachment akHolo = new Attachment();
                akHolo.IntRect = new IntRect(288, 0, 32, 32);
                akHolo.Size = new Vector2i(2, 2);
                akHolo.strRef = "holo_ak47";
                akHolo.attachmentType = AttachmentType.Scope;
                akHolo.resizeFactor = new Vector2i(0, 0);
                akHolo.spriteOffset = new Vector2f(175, -7);
                akHolo.resizeDirection = ResizeDirection.Top;
                inv.AddItem(akHolo);
                break;
        }
    }
}
