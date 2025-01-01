using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using System.Windows;

namespace BookstoreManagement.LoginUI.Services;

public class LoginService
{
    private readonly ApplicationDbContext db;
    private CurrentUserService CurrentUserService;
    private readonly MailService mailService;

    public LoginService(ApplicationDbContext db, CurrentUserService currentUserService, MailService mailService)
    {
        this.db = db;
        this.CurrentUserService = currentUserService;
        this.mailService = mailService;
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

    public void ForgotPasswordAsync(string email)
    {
        var employee = db.Employees.Where(e => e.Email == email).FirstOrDefault()
            ?? throw new ArgumentException("Email not found");
        mailService.SendAsync(email, "Forgot password", $"Your password is {employee.Password}");
    }
}
