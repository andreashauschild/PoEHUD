using PoeHUD.Models.Enums;
using PoeHUD.Poe.Elements;
using System.Collections.Generic;

namespace PoeHUD.Poe.RemoteMemoryObjects
{
    public class Inventory : RemoteMemoryObject
    {
        public long ItemCount => M.ReadLong(Address + 0x410, 0x5F0, 0x50);

        private InventoryType GetInvType()
        {
            switch (this.AsObject<Element>().Parent.ChildCount)
            {
                case 0x6f:
                    return InventoryType.EssenceStash;
                case 0x26:
                    return InventoryType.CurrencyStash;
                case 0x18:
                    return InventoryType.PlayerInventory;
                case 0x05:
                    return InventoryType.DivinationStash;
                case 0x01:
                    // Normal Stash and Quad Stash is same.
                    return InventoryType.NormalStash;
                default:
                    return InventoryType.InvalidInventory;
            }
        }

        public InventoryType InvType => GetInvType();

        // Shows Item details of visible inventory/stashes
        public List<Element> VisibleInventoryItems
        {
            get
            {
                var list = new List<Element>();
                var InventoryItemRoot = this.AsObject<Element>();
                if (!InventoryItemRoot.IsVisible)
                    return list;
                switch (InvType)
                {
                    case InventoryType.PlayerInventory:
                    case InventoryType.NormalStash:
                    case InventoryType.QuadStash:
                        foreach (var item in InventoryItemRoot.Children)
                        {
                            list.Add(item.AsObject<NormalInventoryItem>());
                        }
                        break;
                    case InventoryType.CurrencyStash:
                        foreach (var item in InventoryItemRoot.Parent.Children)
                        {
                            if (item.ChildCount > 0)
                                list.Add(item.Children[0].AsObject<CurrencyInventoryItem>());
                        }
                        break;
                    case InventoryType.EssenceStash:
                        foreach (var item in InventoryItemRoot.Parent.Children)
                        {
                            if (item.ChildCount > 0)
                                list.Add(item.Children[0].AsObject<EssenceInventoryItem>());
                        }
                        break;
                    case InventoryType.DivinationStash:
                        //Implementation TODO.
                        break;
                }
                return list;
            }
        }

        // Works even if inventory is currently not in view.
        // As long as game have fetched inventory data from Server.
        // Will return the item based on x,y format.
        // Give more controll to user what to do with
        // dublicate items (items taking more than 1 slot)
        // or slots where items doesn't exists (return null).
        public Entity this[int x, int y, int xLength]
        {
            get
            {
                long invAddr = M.ReadLong(Address + 0x410, 0x5F0, 0x30);
                y = y * xLength;
                long itmAddr = M.ReadLong(invAddr + ((x + y) * 8));
                return ReadObject<Entity>(itmAddr);
            }
        }
    }
}