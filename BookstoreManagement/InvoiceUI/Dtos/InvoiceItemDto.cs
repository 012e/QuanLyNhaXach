using BookstoreManagement.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.InvoiceUI.Dtos
{
    public  class InvoiceItemDto: ObservableObject
    {
        public int id {  get; set; }
        public string Name {  get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }
        public decimal Price { get; set; }

        public decimal TotalPrice {  get; set; }
    }
}
