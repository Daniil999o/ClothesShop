﻿namespace ClothesShop.Models
{
    public class CardsModel
    {
        public bool IsAdmin { get; set; }
        public List<ProductItem> Items { get; set; }

        public ProductItem ItemToCreate { get; set; }

        public string Gender { get; set; }

        public List<Manufacturer> Manufacturers { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}
