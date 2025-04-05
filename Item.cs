using System;
namespace Pract1
{
    public class Item
    {
        public string Article { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }

        public Item(string article, string name, decimal unitPrice)
        {
            Article = article;
            Name = name;
            UnitPrice = unitPrice;
        }
    }
}

