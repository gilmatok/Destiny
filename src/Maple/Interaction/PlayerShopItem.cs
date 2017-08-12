namespace Destiny.Maple.Interaction
{
    public sealed class PlayerShopItem : Item
    {
        public short Bundles { get; set; }
        public int MerchantPrice { get; private set; }

        public PlayerShopItem(int mapleID, short bundles, short quantity, int price)
            : base(mapleID, quantity)
        {
            this.Bundles = bundles;
            this.MerchantPrice = price;
        }
    }
}
