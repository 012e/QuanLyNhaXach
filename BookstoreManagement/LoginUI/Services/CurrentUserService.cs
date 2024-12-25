using BookstoreManagement.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.LoginUI.Services;

public class CurrentUserService
{
    public Employee? CurrentUser { get; set; } = null;
    public bool IsAdmin => CurrentUser.IsManager;
    public bool IsLoggedIn()
    {
        return CurrentUser != null;
    }
}
