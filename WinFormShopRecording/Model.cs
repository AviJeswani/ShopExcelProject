using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormShopRecording
{
    public class Item
    {
        [LinqToExcel.Attributes.ExcelColumn("ItemId")]
        public int ItemId { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("PSU")]
        public string PSU { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("Name")]
        public string Name { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("Price")]
        public string Price { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("Type")]
        public string Type { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("DisplayType")]
        public string DisplayType { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("Company")]
        public string CompanyName { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("Description")]
        public string Description { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("HTMLtype")]
        public string HTMLtype { get; set; }

        [LinqToExcel.Attributes.ExcelColumn("ExtraData")]
        public string ExtraData { get; set; }
    }

    public class ItemViewModel : Item
    {
        public ItemViewModel(Item _b)
        {
            this.CompanyName = string.IsNullOrEmpty(_b.CompanyName) ? string.Empty : _b.CompanyName;
            this.DisplayType = _b.DisplayType;
            this.ItemId = _b.ItemId;
            this.Name = _b.Name;
            this.Price = _b.Price;
            this.PSU = _b.PSU;
            this.Quantity = 1;
            this.SubTotal = this.Quantity * Convert.ToInt32(_b.Price);
            this.Type = _b.Type;
            this.Description = _b.Description;
        }
        public int MyProperty { get; set; }

        public int Quantity { get; set; }

        public int SubTotal { get; set; }
    }

}
