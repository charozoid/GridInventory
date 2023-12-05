using SFML.System;
using SFML.Graphics;
using SFML.Window;

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
            /*RectangleShape shape = new RectangleShape(new Vector2f(item.size.X * 32, item.size.Y * 32));
            shape.Position = item.sprite.Position;
            shape.FillColor = new Color(225, 225, 225, 225);
            window.Draw(shape);*/
            item.Draw();
        }
    }
    public static void DrawItemSquare()
    {
        Item[,] itemGrid = GridInventory.inv.itemGrid;
        for (int i = 0; i < itemGrid.GetLength(0); i++)
        {
            for (int j = 0; j < itemGrid.GetLength(1); j++)
            {
                if (itemGrid[i, j] != null)
                {
                    RectangleShape rectangle = new RectangleShape(new Vector2f(32, 32));
                    rectangle.Position = new Vector2f(i * 32, j * 32);
                    rectangle.FillColor = new Color(125, 125, 0, 255);
                    window.Draw(rectangle);
                }
            }
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
