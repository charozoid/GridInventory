using SFML.System;
using SFML.Graphics;
using SFML.Window;

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
    public void ButtonReleased(object sender, MouseButtonEventArgs e)
    {
        Vector2i mousePos = Graphics.MousePos();
        switch (e.Button)
        {
            case Mouse.Button.Right:
                if (!IsSquareEmpty(mousePos) && itemGrid[mousePos.X, mousePos.Y] is Weapon weaponToRemove)
                {
                    weaponToRemove.RemoveAttachment(Item.AttachmentType.Magazine);
                }
                break;
            case Mouse.Button.Left:
                if (selectedItem == null)
                    return;

                if (!IsSquareEmpty(mousePos) && itemGrid[mousePos.X, mousePos.Y] is Weapon weapon && selectedItem is Attachment attachment)
                {
                    if (weapon.attachments[(int)attachment.type] == null && weapon.CanResize(attachment.resizeFactor))
                    {
                        weapon.AddAttachment(attachment);
                        weapon.Resize(attachment.resizeFactor);
                    }
                    else
                    {
                        MoveItem(selectedItem, selectedItem.oldPos);
                    }
                }
                else
                {
                    bool areSquaresAvailable = AreSquaresAvailable(mousePos);

                    if (areSquaresAvailable)
                    {
                        MoveItem(selectedItem, mousePos);
                    }
                    else
                    {
                        MoveItem(selectedItem, selectedItem.oldPos);
                    }
                }
                selectedItem = null;
                hasItemSelected = false;
                break;
        }
    }
    public bool AreSquaresAvailable(Vector2i mousePos)
    {
        bool areSquaresAvailable = true;
        for (int i = mousePos.X; i < mousePos.X + selectedItem.size.X; i++)
        {
            for (int j = mousePos.Y; j < mousePos.Y + selectedItem.size.Y; j++)
            {
                if ((i > 29 | j > 17 || i < 0 || j < 0) || itemGrid[i, j] != null && itemGrid[i, j] != selectedItem)
                {
                    areSquaresAvailable = false;
                }
                

            }
        }
        return areSquaresAvailable;
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
                            if (itemGrid[i, j] is Weapon weapon && selectedItem is Attachment attachment && weapon.IsAttachmentCompatible(attachment) && weapon.IsAttachmentSlotEmpty(attachment.attachmentType) && weapon.CanResize(attachment.resizeFactor))
                            {
                                rectangle.Position = weapon.sprite.Position;
                                rectangle.Size = (Vector2f)weapon.size * 32;
                            }
                            else
                            {
                                rectangle.FillColor = new Color(225, 0, 0, 175);
                            }
                        }
                    }

                }
            }

            Graphics.window.Draw(rectangle);
        }
    }
}