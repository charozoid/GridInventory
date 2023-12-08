using SFML.System;
using SFML.Graphics;

class Item
{
    public enum Type
    {
        Weapon,
        Attachment
    }
    public enum ResizeDirection
    {
        Left, Right, Top, Bottom
    }
    public enum AttachmentType
    {
        Magazine,
        Scope,
        Muzzle
    }
    public RectangleShape shape = new RectangleShape(new Vector2f(0, 0));
    public Sprite sprite { get; set; }
    public IntRect intRect;
    public Vector2i size { get; set; }
    public Vector2i oldPos { get; set; }
    public Vector2i inventoryPos { get; set; }
    public Vector2i spriteGridOffset = new Vector2i(0, 0);
    public Type type;
    public AttachmentType attachmentType;
    public string strRef = "";

    public Vector2i Size
    {
        get { return size; }
        set
        {
            shape.Size = (Vector2f)value * 32;
            size = value;
        }
    }
    public IntRect IntRect
    {
        get { return intRect; }
        set
        {
            intRect = value;
            sprite = new Sprite(Graphics.gunsTexture);
            sprite.Scale = new Vector2f(2, 2);
            sprite.TextureRect = IntRect;
        }
    }
    public virtual Vector2i InventoryPos
    {
        get { return inventoryPos; }
        set
        {
            inventoryPos = value;
            Vector2f spritePos = Graphics.GridToVector2f(value) + (Vector2f)spriteGridOffset * 32;
            shape.Position = Graphics.GridToVector2f(value);
            shape.Size = (Vector2f)size * 32;
            sprite.Position = spritePos;
        }
    }
    public virtual void Draw()
    {
        Graphics.window.Draw(shape);
        shape.FillColor = new Color(30, 20, 40, 225);
        Graphics.window.Draw(sprite);
    }
    public virtual void Move(Vector2i gridCoords)
    {
        InventoryPos = gridCoords;
    }
    public bool CanExpand(ResizeDirection direction, Vector2i addedSize)
    {
        Item[,] itemGrid = GridInventory.inv.itemGrid;
        int startX = InventoryPos.X;
        int startY = InventoryPos.Y;
        int endX = InventoryPos.X + size.X;
        int endY = InventoryPos.Y + size.Y;
        switch (direction)
        {
            case ResizeDirection.Left:
                for (int i = 0; i < addedSize.X; i++)
                {
                    for (int y = startY; y < endY; y++)
                    {
                        if (startX - i - 1 < 0 || itemGrid[startX - i - 1, y] != null)
                            return false;
                    }
                }
                break;
            case ResizeDirection.Right:
                for (int i = 0; i < addedSize.X; i++)
                {
                    for (int y = startY; y < startY + size.Y + addedSize.Y; y++)
                    {
                        if (endX + i > 29 || itemGrid[endX + i, y] != null)
                            return false;
                    }
                }
                break;
            case ResizeDirection.Top:
                for (int i = 0; i < addedSize.Y; i++)
                {
                    for (int x = startX; x < InventoryPos.X + size.X; x++)
                    {
                        if (startY - i - 1 < 0 || itemGrid[x, startY - i - 1] != null)
                            return false;
                    }
                }
                break;
            case ResizeDirection.Bottom:
                for (int i = 0; i < addedSize.Y; i++)
                {
                    for (int x = InventoryPos.X; x < endX; x++)
                    {
                        if (endY + i > 17 || itemGrid[x, endY + i] != null)
                            return false;
                    }
                }
                break;
        }
        return true;
    }
    public void Shrink(ResizeDirection direction, Vector2i shrinkedSize)
    {
        Item[,] itemGrid = GridInventory.inv.itemGrid;
        int startX = InventoryPos.X;
        int startY = InventoryPos.Y;
        int endX = InventoryPos.X + size.X;
        int endY = InventoryPos.Y + size.Y;
        int absX = Math.Abs(shrinkedSize.X);
        int absY = Math.Abs(shrinkedSize.Y);
        switch (direction)
        {
            case ResizeDirection.Left:

                itemGrid[startX, startY].spriteGridOffset += shrinkedSize;
                for (int i = 0; i < absX; i++)
                {
                    for (int y = InventoryPos.Y; y < endY; y++)
                    {
                        itemGrid[startX + i, y] = null;
                    }
                }
                itemGrid[startX + absX, startY].shape.Size += (Vector2f)shrinkedSize * 32;
                itemGrid[startX + absX, startY].shape.Position -= (Vector2f)shrinkedSize * 32;
                inventoryPos -= shrinkedSize;
                break;
            case ResizeDirection.Right:
                for (int i = 0; i < absX; i++)
                {
                    for (int y = InventoryPos.Y; y < endY; y++)
                    {
                        itemGrid[endX - i - 1, y] = null;
                    }
                }
                itemGrid[startX, startY].shape.Size += (Vector2f)shrinkedSize * 32;
                break;
            case ResizeDirection.Top:
                itemGrid[startX, startY].spriteGridOffset += shrinkedSize;
                for (int i = 0; i < absY; i++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        itemGrid[x, startY - i] = null;
                    }
                }
                itemGrid[startX, startY + absY].shape.Size += (Vector2f)shrinkedSize * 32;
                itemGrid[startX, startY + absY].shape.Position -= (Vector2f)shrinkedSize * 32;
                inventoryPos -= shrinkedSize;
                break;
            case ResizeDirection.Bottom:
                for (int i = 0; i < absY; i++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        itemGrid[x, endY - i - 1] = null;
                    }
                }
                itemGrid[startX, startY + absY].shape.Size += (Vector2f)shrinkedSize * 32;
                break;
        }
        size += shrinkedSize;
    }
    public void Expand(ResizeDirection direction, Vector2i addedSize)
    {
        Item[,] itemGrid = GridInventory.inv.itemGrid;
        int startX = InventoryPos.X;
        int startY = InventoryPos.Y;
        int endX = InventoryPos.X + size.X;
        int endY = InventoryPos.Y + size.Y;
        switch (direction)
        {
            case ResizeDirection.Left:
                for (int i = 0; i < addedSize.X; i++)
                {
                    for (int y = startY; y < endY; y++)
                    {
                        itemGrid[startX - i - 1, y] = this;
                    }
                }
                itemGrid[startX, startY].spriteGridOffset += addedSize;
                itemGrid[startX, startY].shape.Size += (Vector2f)addedSize * 32;
                itemGrid[startX, startY].shape.Position -= (Vector2f)addedSize * 32;
                inventoryPos -= addedSize;
                break;
            case ResizeDirection.Right:
                for (int i = 0; i < addedSize.X; i++)
                {
                    for (int y = startY; y < startY + size.Y + addedSize.Y; y++)
                    {
                        itemGrid[endX + i, y] = this;
                    }

                }
                itemGrid[startX, startY].shape.Size += (Vector2f)addedSize * 32;
                break;
            case ResizeDirection.Top:
                for (int i = 0; i < addedSize.Y; i++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        itemGrid[x, startY - i - 1] = this;
                    }
                }
                itemGrid[startX, startY - addedSize.Y].shape.Size += (Vector2f)addedSize * 32;
                itemGrid[startX, startY - addedSize.Y].shape.Position -= (Vector2f)addedSize * 32;
                itemGrid[startX, startY].spriteGridOffset = addedSize;
                inventoryPos -= addedSize;
                break;
            case ResizeDirection.Bottom:
                for (int i = 0; i < addedSize.Y; i++)
                {
                    for (int x = InventoryPos.X; x < endX; x++)
                    {
                        itemGrid[x, endY + i] = this;
                    }
                }
                itemGrid[startX, startY].shape.Size += (Vector2f)addedSize * 32;
                break;
        }
        size += addedSize;
    }
    public bool Resize(ResizeDirection direction, Vector2i addedSize)
    {
        if (addedSize.X > 0 || addedSize.Y > 0)
        {
            if (!CanExpand(direction, addedSize))
                return false;
            Expand(direction, addedSize);
            return true;
        }
        else
        {
            Shrink(direction, addedSize);
            return true;
        }
    }
}
class Weapon : Item
{
    public static Dictionary<string, string[]> attachmentsList = new Dictionary<string, string[]>();
    public Attachment[] attachments = new Attachment[3];
    public static void LoadCompatibleAttachments()
    {
        attachmentsList["weapon_ak47"] = new string[] { "mag_ak47", "silencer_ak47", "holo_ak47" };
        attachmentsList["weapon_glock"] = new string[] { "mag_glock" };
    }
    public override Vector2i InventoryPos
    {
        get { return inventoryPos; }
        set
        {
            inventoryPos = value;
            Vector2f spritePos = Graphics.GridToVector2f(value) + (Vector2f)spriteGridOffset * 32;
            shape.Position = (Vector2f)inventoryPos * 32;
            sprite.Position = spritePos;
            for (int i = 0; i < attachments.Length; i++)
            {
                if (attachments[i] != null)
                {
                    attachments[i].sprite.Position = sprite.Position + attachments[i].spriteOffset;
                }
            }
        }
    }
    public override void Move(Vector2i gridCoords)
    {
        InventoryPos = gridCoords;
    }
    public override void Draw()
    {
        base.Draw();
        foreach (Attachment attachment in attachments)
        {
            if (attachment != null && !attachment.hide)
            {
                Graphics.window.Draw(attachment.sprite);
            }

        }

    }
    public void AddAttachment(Attachment attachment)
    {
        if (!IsAttachmentCompatible(attachment) || attachments[(int)attachment.attachmentType] != null)
            return;

        attachments[(int)attachment.attachmentType] = attachment;
        int attachX = attachment.oldPos.X;
        int attachY = attachment.oldPos.Y;
        attachment.oldPos = new Vector2i(-1, -1);
        attachment.shape.Size = new Vector2f(0, 0);
        for (int i = attachX; i < attachment.size.X + attachX; i++)
        {
            for (int j = attachY; j < attachment.size.Y + attachY; j++)
            {
                GridInventory.inv.itemGrid[i, j] = null;
            }
        }
        Graphics.itemsToDraw.Remove(attachment);

        Vector2f weaponSpritePos = sprite.Position;
        attachment.sprite.Position = new Vector2f(weaponSpritePos.X, weaponSpritePos.Y) + attachment.spriteOffset;
    }
    public void RemoveAttachment(AttachmentType attachmentType)
    {
        if (IsAttachmentSlotEmpty(attachmentType)) return;

        Inventory inv = GridInventory.inv;
        Attachment attachment = attachments[(int)attachmentType];
        attachment.shape.Size = (Vector2f)attachment.size * 32;
        inv.MoveItem(attachment, inv.FindEmptySpot(attachment.size));
        attachments[(int)attachmentType] = null;
        Graphics.itemsToDraw.Add(attachment);
        Resize(attachment.resizeDirection, -attachment.resizeFactor);
    }
    public bool IsAttachmentCompatible(Attachment attachment)
    {
        for (int i = 0; i < attachmentsList[strRef].Length; i++)
        {
            if (attachment.strRef == attachmentsList[strRef][i])
            {
                return true;
            }
        }
        return false;
    }
    public bool IsAttachmentSlotEmpty(AttachmentType attachmentType)
    {
        return attachments[(int)attachmentType] == null;
    }
}
class Attachment : Item
{
    public Vector2f spriteOffset = new Vector2f();
    public Vector2i resizeFactor = new Vector2i();
    public ResizeDirection resizeDirection = ResizeDirection.Left;
    public bool hide = false;
}
class Magazine : Attachment
{
    public Type type = Type.Attachment;
}