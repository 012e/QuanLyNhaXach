using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace BookstoreManagement.LoginUI.Services;

public class LoginService
{
    private readonly ApplicationDbContext db;
    private CurrentUserService CurrentUserService; 
    public LoginService(ApplicationDbContext db, CurrentUserService currentUserService)
    {
        this.db = db;
        this.CurrentUserService = currentUserService;
    }

    public Employee? Login(LoginDto loginDto)
    {
        var email = loginDto.Email;
        var password = loginDto.Password;

        var employee = db.Employees.FirstOrDefault(e => e.Email == email);
        if (employee == null)
        {
            return null;
        }
        if (employee.Password != password)
        {
            return null;
        }
        CurrentUserService.CurrentUser = employee;
        return employee;
    }

    public async Task<Employee?> LoginAsync(LoginDto loginDto)
    {
        var email = loginDto.Email;
        var password = loginDto.Password;

        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Email == email);
        if (employee == null)
        {
            return null;
        }
        if (employee.Password != password)
        {
            return null;
        }
        CurrentUserService.CurrentUser = employee;
        return employee;
    }
}
