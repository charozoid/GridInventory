using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

class GridInventory
{
    public static Clock clock = new Clock();
    public static void Main(string[] args)
    {
        Graphics.window.SetVerticalSyncEnabled(true);
        Graphics.window.Closed += (sender, args) => Graphics.window.Close();
        Inventory inv = new Inventory();
        Item ak47 = new Item();
        ak47.IntRect = new IntRect(0, 0, 192, 64);
        ak47.size = new Vector2i(6, 2);
        inv.AddItem(ak47);
        while (Graphics.window.IsOpen)
        {
            Graphics.window.Clear(Color.Black);
            Graphics.DrawGrid();
            Graphics.DrawItems();
            KeyPressed(inv);
            Graphics.window.DispatchEvents();
            Graphics.window.Display();
        }
    }
    public static void KeyPressed(Inventory inv)
    {
        Time elapsedTime = clock.ElapsedTime;
        if (Mouse.IsButtonPressed(Mouse.Button.Left) && elapsedTime.AsMilliseconds() >= 200)
        {
            inv.LeftClick(Graphics.MousePos());
            clock.Restart();
        }
    }
}
class Graphics
{
    public const int WIDTH = 960;
    public const int HEIGHT = 576;
    public static VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    public static RenderWindow window = new RenderWindow(mode, "Isometric");
    public static Texture gunsTexture = new Texture("../../Assets/guns.png");
    public static List<Item> itemList = new List<Item>();

    public static void DrawGrid()
    {
        Sprite sprite = new Sprite(gunsTexture);
        sprite.TextureRect = new IntRect(0, 64, 32, 32);
        for (int i = 0; i < WIDTH / 32;  i++)
        {
            for (int j = 0; j < HEIGHT / 32; j++)
            {
                sprite.Position = new Vector2f(i * 32, j * 32);
                window.Draw(sprite);
            }
        }
    }
    public static void CreateItem()
    {

    }
    public static void DrawItems()
    {
        foreach (Item item in itemList)
        {
            item.Draw();
        }
    }
    public static Vector2f GridToVector2f(Vector2i gridCoords)
    {
        return new Vector2f(gridCoords.X * 32, gridCoords.Y * 32);
    }
    public static Vector2i MousePos()
    {
        Vector2i mousePos = Mouse.GetPosition(window);
        if (mousePos.X > 900 || mousePos.X < 0 || mousePos.Y > 576 || mousePos.Y < 0)
            return new Vector2i(0, 0);
        return mousePos / 32;
    }
}
class Item
{
    public Vector2i inventoryPos { get; set; }
    private Sprite sprite;
    public IntRect intRect;
    public Vector2i size { get; set; }
    public IntRect IntRect {  
        get { return intRect; }
        set {
            intRect = value;
            sprite = new Sprite(Graphics.gunsTexture);
            sprite.TextureRect = IntRect;
        }
    }
    public Vector2i InventoryPos {
        get { return inventoryPos; }
        set {
            inventoryPos = value;
            sprite.Position = Graphics.GridToVector2f(value);
        } 
    }
    public void Draw()
    {
        Graphics.window.Draw(sprite);
    }
    public void Move(Vector2i gridCoords)
    {
        InventoryPos = gridCoords;
    }
}

class Inventory
{
    public Item?[,] itemGrid = new Item[30, 18];
    public Item selectedItem;
    private bool hasItemSelected = false;
    public void MoveItem(Item item, Vector2i gridCoords)
    {
        int itemX = item.InventoryPos.X;
        int itemY = item.InventoryPos.Y;
        for (int i = itemX; i < item.size.X + itemX; i++)
        {
            for (int j = itemY; j < item.size.Y + itemY; j++)
            {
                itemGrid[i, j] = null;
            }
        }
        int gridX = gridCoords.X;
        int gridY = gridCoords.Y;
        for (int i = gridX; i < item.size.X + gridX; i++)
        {
            for (int j = gridY; j < item.size.Y + gridY; j++)
            {
                itemGrid[i, j] = item;
            }
        }
        item.Move(gridCoords);
    }
    public void AddItem(Item item)
    {
        item.InventoryPos = new Vector2i(2, 2);
        int itemX = item.InventoryPos.X;
        int itemY = item.InventoryPos.Y;
        for (int i = itemX; i < item.size.X + itemX; i++)
        {
            for (int j = itemY; j < item.size.Y + itemY; j++)
            {
                itemGrid[i, j] = item;
            }
        }
        Graphics.itemList.Add(item);
    }
    public void RemoveItem(Item item)
    {
        if (item == null)
            return;

        for (int i = item.InventoryPos.X; i < item.size.X; i++)
        {
            for (int j = item.InventoryPos.Y; j < item.size.Y; j++)
            {
                itemGrid[i, j] = null;
            }
        }
        Graphics.itemList.Remove(item);
    }
    public Item? GridToItem(Vector2i gridCoords)
    {
        if (itemGrid[gridCoords.X, gridCoords.Y] == null)
        {
            return null;
        }           
        return itemGrid[gridCoords.X, gridCoords.Y];
    }
    public void LeftClick(Vector2i mousePos)
    {
        if (selectedItem != null)
        {
            MoveItem(selectedItem, mousePos);
            selectedItem = null;
            hasItemSelected = false;
        }
        else if (GridToItem(mousePos) != null)
        {
            selectedItem = GridToItem(mousePos);
        }

    }
}