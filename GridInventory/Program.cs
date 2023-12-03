using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;
class GridInventory
{
    public static Inventory inv = new Inventory();
    public static void Main(string[] args)
    {
        Graphics.window.SetVerticalSyncEnabled(true);
        Graphics.window.Closed += (sender, args) => Graphics.window.Close();
        Graphics.window.MouseButtonPressed += inv.ButtonPressed;
        Graphics.window.MouseButtonReleased += inv.ButtonReleased;
        Graphics.window.KeyPressed += EventHandler;

        while (Graphics.window.IsOpen)
        {
            Graphics.window.Clear(Color.Black);
            
            Graphics.DrawGrid();
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
                Item ak47 = new Item();
                ak47.IntRect = new IntRect(0, 0, 192, 64);
                ak47.size = new Vector2i(6, 2);
                inv.AddItem(ak47);
                break;
        }
    }
}
class Graphics
{
    public const int WIDTH = 960;
    public const int HEIGHT = 576;
    public static VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    public static RenderWindow window = new RenderWindow(mode, "Inventory");
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
    public Sprite sprite {get; set;}
    public IntRect intRect;
    public Vector2i size { get; set; }
    public Vector2i oldPos { get; set; }
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
    public Clock clock = new Clock();
    public Item?[,] itemGrid = new Item[30, 18];
    public Item? selectedItem;
    private bool hasItemSelected = false;

    public void MoveItem(Item item, Vector2i gridCoords)
    {
        int itemX = item.oldPos.X;
        int itemY = item.oldPos.Y;
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
        int itemX = item.InventoryPos.X;
        int itemY = item.InventoryPos.Y;

        int startingIndex = 0;
        int count = 0;
        while (count < item.size.X)
        {
            for (int i = startingIndex; i < item.size.X + itemX + startingIndex; i++)
            {
                if (itemGrid[i, 0] != null)
                {
                    break;
                }
                else
                {
                    count++;
                }
            }
            startingIndex++;
        }
        item.InventoryPos = new Vector2i(startingIndex - 1, 0);

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
    public bool IsSquareEmpty(Vector2i gridCoords)
    {
        return itemGrid[gridCoords.X, gridCoords.Y] == null;
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
    public void ButtonReleased(object sender, MouseButtonEventArgs e)
    {
        Vector2i mousePos = Graphics.MousePos();
        switch (e.Button)
        {
            case (Mouse.Button.Left):
                if (selectedItem != null)
                {
                    bool areSquaresAvailable = true;
                    for (int i = mousePos.X; i < mousePos.X + selectedItem.size.X; i++)
                    {
                        for (int j = mousePos.Y; j < mousePos.Y + selectedItem.size.Y; j++)
                        {
                            if (itemGrid[i, j] != null && itemGrid[i, j] != selectedItem)
                            {
                                areSquaresAvailable = false;
                            }
                        }
                    }
                    if (areSquaresAvailable)
                    {
                        MoveItem(selectedItem, mousePos);
                        selectedItem = null;
                        hasItemSelected = false;
                    }
                    else
                    {
                        MoveItem(selectedItem, selectedItem.oldPos);
                        selectedItem = null;
                        hasItemSelected = false;
                    }
                }
            break;
        }
    }

    public void ButtonPressed(object sender, MouseButtonEventArgs e)
    {
        Vector2i mousePos = Graphics.MousePos();
        switch (e.Button)
        {
            case (Mouse.Button.Left):
                selectedItem = GridToItem(mousePos);
                if (selectedItem != null)
                    selectedItem.oldPos = selectedItem.InventoryPos;
                break;
        }

    }
    public void Think()
    {
        if (selectedItem != null)
        {
            Vector2i mousePos = Graphics.MousePos();
            selectedItem.InventoryPos = mousePos;
            RectangleShape rectangle = new RectangleShape((Vector2f)(selectedItem.size * 32));
            rectangle.Position = selectedItem.sprite.Position;
            rectangle.FillColor = new Color(0, 225, 0, 175);
            for (int i = mousePos.X; i < mousePos.X + selectedItem.size.X; i++)
            {
                for (int j = mousePos.Y; j < mousePos.Y + selectedItem.size.Y; j++)
                {
                    if (i < 30 && i > 0 && j < 18 && j > 0)
                    {
                        if (itemGrid[i, j] != null && itemGrid[i, j] != selectedItem)
                        {
                            rectangle.FillColor = new Color(225, 0, 0, 175);
                        }
                    }

                }
            }

            Graphics.window.Draw(rectangle);
        }
    }
}