using SFML.System;
using SFML.Graphics;

class Item
{
    public enum Type
    {
        Weapon,
        Attachment
    }
    public enum AttachmentType
    {
        Magazine,
        Scope,
        Muzzle
    }
    public Vector2i inventoryPos { get; set; }
    public Sprite sprite {get; set;}
    public IntRect intRect;
    public Vector2i size { get; set; }
    public Vector2i oldPos { get; set; }
    public Type type;
    public AttachmentType attachmentType;
    public string strRef = "";
    public IntRect IntRect {  
        get { return intRect; }
        set {
            intRect = value;
            sprite = new Sprite(Graphics.gunsTexture);
            sprite.Scale = new Vector2f(2, 2);
            sprite.TextureRect = IntRect;
        }
    }
    public virtual Vector2i InventoryPos {
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
        attachmentsList["weapon_ak47"] = new string[] { "mag_ak47" };
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
        if (!IsAttachmentCompatible(attachment) || attachments[(int)attachment.type] != null)
            return;

        attachments[(int)attachment.type] = attachment;
        int attachX = attachment.oldPos.X;
        int attachY = attachment.oldPos.Y;
        for (int i = attachX; i < attachment.size.X + attachX; i++)
        {
            for (int j = attachY; j < attachment.size.Y + attachY; j++)
            {
                GridInventory.inv.itemGrid[i, j] = null;
            }
        }
        Vector2f weaponSpritePos = sprite.Position;
        attachment.sprite.Position = new Vector2f(weaponSpritePos.X, weaponSpritePos.Y) + attachment.spriteOffset;
    }
    public void RemoveAttachment(AttachmentType attachmentType)
    {
        if (attachments[(int)attachmentType] == null)
            return;
        Attachment newAttachment = attachments[(int)attachmentType];
        attachments[(int)attachmentType] = null;
        GridInventory.inv.MoveItem(newAttachment, new Vector2i(0, 0));
        Resize(-newAttachment.resizeFactor);
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

        if (endY + addedSize.Y > 31 || endX + addedSize.X > 31)
            canResize = false;
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
    public void Resize(Vector2i addedSize)
    {
        int endX = InventoryPos.X + size.X;
        int endY = InventoryPos.Y + size.Y;
        Item[,] itemGrid = GridInventory.inv.itemGrid;

        if (addedSize.Y < 0)
        {
            for (int i = 0; i < Math.Abs(addedSize.Y); i++)
            {
                for (int x = InventoryPos.X; x < InventoryPos.X + size.X + addedSize.X; x++)
                {
                    itemGrid[x, endY - i - 1] = null;
                }
            }
        }
        else
        {
            for (int i = 0; i < addedSize.Y; i++)
            {
                for (int x = InventoryPos.X; x < InventoryPos.X + size.X + addedSize.X; x++)
                {
                    itemGrid[x, endY + i] = this;
                }
            }
        }
        if (addedSize.X < 0)
        {
            for (int i = 0; i < Math.Abs(addedSize.X); i++)
            {
                for (int y = InventoryPos.Y; y < InventoryPos.Y + size.Y + addedSize.Y; y++)
                {
                    itemGrid[endX - i - 1, y] = null;
                }
            }
        }
        else
        {
            for (int i = 0; i < addedSize.X; i++)
            {
                for (int y = InventoryPos.Y; y < InventoryPos.Y + size.Y + addedSize.Y; y++)
                {
                    itemGrid[endX + i, y] = this;
                }
            }
        }
        size += addedSize;
    }
}
class Attachment : Item
{
    public Vector2f spriteOffset = new Vector2f();
    public Vector2i resizeFactor = new Vector2i();
}
class Magazine : Attachment
{
    public Type type = Type.Attachment;
    public AttachmentType attachmentType = AttachmentType.Magazine;

}