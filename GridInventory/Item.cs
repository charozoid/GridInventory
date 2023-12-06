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
    public Vector2i inventoryPos { get; set; }
    public Sprite sprite { get; set; }
    public IntRect intRect;
    public Vector2i size { get; set; }
    public Vector2i oldPos { get; set; }
    public Type type;
    public AttachmentType attachmentType;
    public string strRef = "";
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
            sprite.Position = Graphics.GridToVector2f(value);
        }
    }
    public void Draw()
    {
        Graphics.window.Draw(sprite);
    }
    public virtual void Move(Vector2i gridCoords)
    {
        InventoryPos = gridCoords;

    }
}
class Weapon : Item
{
    public static Dictionary<string, string[]> attachmentsList = new Dictionary<string, string[]>();
    public Attachment[] attachments = new Attachment[3];
    public static void LoadCompatibleAttachments()
    {
        attachmentsList["weapon_ak47"] = new string[] { "mag_ak47", "silencer_ak47" };
        attachmentsList["weapon_glock"] = new string[] { "mag_glock" };
    }
    public override Vector2i InventoryPos
    {
        get { return inventoryPos; }
        set
        {
            inventoryPos = value;
            sprite.Position = Graphics.GridToVector2f(value);
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
    public void AddAttachment(Attachment attachment)
    {
        if (!IsAttachmentCompatible(attachment) || attachments[(int)attachment.attachmentType] != null)
            return;

        attachments[(int)attachment.attachmentType] = attachment;
        int attachX = attachment.oldPos.X;
        int attachY = attachment.oldPos.Y;
        attachment.oldPos = new Vector2i(-1, -1);
        for (int i = attachX; i < attachment.size.X + attachX; i++)
        {
            for (int j = attachY; j < attachment.size.Y + attachY; j++)
            {
                GridInventory.inv.itemGrid[i, j] = null;
            }
        }
        if (attachment.hide)
        {
            Graphics.itemsToDraw.Remove(attachment);
        }
        Vector2f weaponSpritePos = sprite.Position;
        attachment.sprite.Position = new Vector2f(weaponSpritePos.X, weaponSpritePos.Y) + attachment.spriteOffset;
    }
    public void RemoveAttachment(AttachmentType attachmentType)
    {
        Inventory inv = GridInventory.inv;
        if (attachments[(int)attachmentType] == null)
            return;
        Attachment newAttachment = attachments[(int)attachmentType];
        attachments[(int)attachmentType] = null;
        Graphics.itemsToDraw.Add(newAttachment);
        inv.MoveItem(newAttachment, inv.FindEmptySpot(newAttachment.size));
        Resize(newAttachment.resizeDirection, -newAttachment.resizeFactor);
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
    public bool CanResize(Vector2i addedSize)
    {
        bool canResize = true;
        int endX = InventoryPos.X + size.X;
        int endY = InventoryPos.Y + size.Y;
        Item[,] itemGrid = GridInventory.inv.itemGrid;

        if (endY + addedSize.Y > 18 || endX + addedSize.X > 30)
            return false;
        for (int i = 0; i < addedSize.Y; i++)
        {
            for (int x = InventoryPos.X; x < InventoryPos.X + size.X + addedSize.X; x++)
            {
                if (itemGrid[x, endY + i] != null)
                {
                    canResize = false;
                }
            }
        }
        for (int i = 0; i < addedSize.X; i++)
        {
            for (int y = InventoryPos.Y; y < InventoryPos.Y + size.Y + addedSize.Y; y++)
            {
                if (itemGrid[endX, y + i] != null)
                {
                    canResize = false;
                }
            }
        }
        return canResize;
    }
    public void Shrink(ResizeDirection direction, Vector2i shrinkedSize)
    {
        Item[,] itemGrid = GridInventory.inv.itemGrid;
        int startX = InventoryPos.X;
        int startY = InventoryPos.Y;
        int endX = InventoryPos.X + size.X;
        int endY = InventoryPos.Y + size.Y;
        switch (direction)
        {
            case ResizeDirection.Left:
                for (int i = 0; i < Math.Abs(shrinkedSize.X); i++)
                {
                    for (int y = InventoryPos.Y; y < InventoryPos.Y + size.Y; y++)
                    {
                        itemGrid[startX + i, y] = null;
                    }
                }
                inventoryPos -= shrinkedSize;
                break;
            case ResizeDirection.Right:
                for (int i = 0; i < Math.Abs(shrinkedSize.X); i++)
                {
                    for (int y = InventoryPos.Y; y < InventoryPos.Y + size.Y; y++)
                    {
                        itemGrid[endX - i - 1, y] = null;
                    }
                }
                break;
            case ResizeDirection.Top:
                for (int i = 0; i < Math.Abs(shrinkedSize.Y); i++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        itemGrid[x, startY - i - 1] = null;
                    }
                }
                break;
            case ResizeDirection.Bottom:
                for (int i = 0; i < Math.Abs(shrinkedSize.Y); i++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        itemGrid[x, endY - i - 1] = null;
                    }
                }
                break;
        }
        size += shrinkedSize;
    }
    public void Expand(Item.ResizeDirection direction, Vector2i addedSize)
    {
        Item[,] itemGrid = GridInventory.inv.itemGrid;
        int startX = InventoryPos.X;
        int startY = InventoryPos.Y;
        int endX = InventoryPos.X + size.X;
        int endY = InventoryPos.Y + size.Y;
        switch (direction)
        {
            case Item.ResizeDirection.Left:
                for (int i = 0; i < addedSize.X; i++)
                {
                    for (int y = startY; y < endY; y++)
                    {
                        itemGrid[startX - i - 1, y] = this;
                    }
                }
                inventoryPos -= addedSize;
                break;
            case Item.ResizeDirection.Right:
                for (int i = 0; i < addedSize.X; i++)
                {
                    for (int y = startY; y < startY + size.Y + addedSize.Y; y++)
                    {
                        itemGrid[endX + i, y] = this;
                    }
                }
                break;
            case Item.ResizeDirection.Top:
                for (int i = 0; i < addedSize.Y; i++)
                {
                    for (int x = startX; x < InventoryPos.X + size.X; x++)
                    {
                        itemGrid[x, startY - i - 1] = this;
                    }
                }
                break;
            case Item.ResizeDirection.Bottom:
                for (int i = 0; i < addedSize.Y; i++)
                {
                    for (int x = InventoryPos.X; x < endX; x++)
                    {
                        itemGrid[x, endY + i] = this;
                    }
                }
                break;
        }
        size += addedSize;
    }
    public void Resize(Item.ResizeDirection direction, Vector2i addedSize)
    {
        if (addedSize.X > 0 || addedSize.Y > 0)
        {
            Expand(direction, addedSize);
        }
        else
        {
            Shrink(direction, addedSize);
        }

    }
}
class Attachment : Item
{
    public Vector2f spriteOffset = new Vector2f();
    public Vector2i resizeFactor = new Vector2i();
    public ResizeDirection resizeDirection = ResizeDirection.Left;
    public AttachmentType attachmentType;
    public bool hide = false;
}
class Magazine : Attachment
{
    public Type type = Type.Attachment;
}