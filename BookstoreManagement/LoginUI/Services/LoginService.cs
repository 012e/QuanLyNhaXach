using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;

namespace BookstoreManagement.LoginUI.Services;

public class LoginService
{
    private readonly ApplicationDbContext db;

    public LoginService(ApplicationDbContext db)
    {
        this.db = db;
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
        return employee;
    }
}
