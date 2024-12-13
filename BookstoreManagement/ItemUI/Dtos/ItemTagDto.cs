using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreManagement.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookstoreManagement.ItemUI.Dtos;

public partial class ItemTagDto: ObservableObject
{
    public Tag Tag { get; set; } = null;
    [ObservableProperty]
    private bool _isChecked  = false;
}
