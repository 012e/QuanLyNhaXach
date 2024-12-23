using BookstoreManagement.Core.Helper;
using BookstoreManagement.LoginUI;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.MainUI;
using BookstoreManagement.ImportUI;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.UI.CustomerUI;
using BookstoreManagement.UI.DashboardUI;
using BookstoreManagement.UI.EmployeeUI;
using BookstoreManagement.UI.InvoicesUI;
using BookstoreManagement.UI.ItemUI;
using BookstoreManagement.UI.MainWindow;
using BookstoreManagement.UI.ProviderUI;
using BookstoreManagement.UI.TagUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace BookstoreManagement;

public partial class App : Application
{

    private IHost AppHost { get; }

    public App()
    {
        DotNetEnv.Env.TraversePath().Load();
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION")), ServiceLifetime.Transient);

        builder.Services.AddViewViewModel<MainWindowV, MainWindowVM>();

        builder.Services.AddViewViewModel<AllItemsV, AllItemsVM>();
        builder.Services.AddViewContextualViewModel<EditItemV, EditItemVM, Item>();
        builder.Services.AddViewViewModel<CreateItemV, CreateItemVM>();

        builder.Services.AddViewViewModel<AllInvoicesV, AllInvoicesVM>();
        builder.Services.AddViewContextualViewModel<EditInvoiceV, EditInvoiceVM, Invoice>();
        builder.Services.AddViewContextualViewModel<AddInvoiceItemV, AddInvoiceItemVM, Invoice>();
        builder.Services.AddViewViewModel<CreateInvoiceV, CreateInvoiceVM>();

        builder.Services.AddViewViewModel<AllTagsV, AllTagsVM>();
        builder.Services.AddViewContextualViewModel<EditTagV, EditTagVM, Tag>();
        builder.Services.AddViewViewModel<CreateTagV, CreateTagVM>();

        builder.Services.AddViewViewModel<AllProviderV, AllProviderVM>();
        builder.Services.AddViewContextualViewModel<EditProviderV, EditProviderVM, Provider>();
        builder.Services.AddViewViewModel<CreateProviderV, CreateProviderVM>();

        builder.Services.AddViewViewModel<AllEmployeeV, AllEmployeeVM>();
        builder.Services.AddViewContextualViewModel<EditEmployeeV, EditEmployeeVM, Employee>();
        builder.Services.AddViewViewModel<CreateEmployeeV, CreateEmployeeVM>();

        builder.Services.AddViewViewModel<AllCustomersV, AllCustomersVM>();
        builder.Services.AddViewContextualViewModel<EditCustomerV, EditCustomerVM, Customer>();
        builder.Services.AddViewViewModel<CreateCustomerV, CreateCustomerVM>();

        builder.Services.AddViewViewModel<AllImportV, AllImportVM>();
        builder.Services.AddViewContextualViewModel<EditImportV, EditImportVM, Import>();
        builder.Services.AddViewViewModel<CreateImportV, CreateImportVM>();


        builder.Services.AddViewViewModel<DashBoardV, DashBoardVM>();
        builder.Services.AddViewViewModel<MainV, MainVM>();

        builder.Services.AddViewViewModel<LoginV, LoginVM>();
        builder.Services.AddSingleton<LoginService>();
        builder.Services.AddSingleton<CurrentUserService>();

        builder.Services.AddViewViewModel<DashBoardV, DashBoardVM>();

        builder.Services.AddKeyedSingleton<NavigatorStore>("default");
        builder.Services.AddKeyedSingleton<NavigatorStore>("global");

        AppHost = builder.Build();
    }

    private bool NeedLogin()
    {
        var loginEnv = Environment.GetEnvironmentVariable("LOGIN_REQUIRED");
        if (loginEnv is null)
        {
            return true;
        }

        if (bool.TryParse(loginEnv, out bool loginRequired))
        {
            return loginRequired;
        }

        return true;
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        AppHost.Start();
        AppHost.Services.GetRequiredService<MainWindowV>().Show();


        var globalNavigator = AppHost.Services.GetRequiredKeyedService<NavigatorStore>("global");

        if (NeedLogin())
        {
            var loginVM = AppHost.Services.GetRequiredService<LoginVM>();
            globalNavigator.CurrentViewModel = loginVM;
        }
        else
        {
            var mainVM = AppHost.Services.GetRequiredService<MainVM>();
            var currentUser = AppHost.Services.GetRequiredService<CurrentUserService>();
            var db = AppHost.Services.GetRequiredService<ApplicationDbContext>();
            currentUser.CurrentUser = db.Employees.Find(1);

            globalNavigator.CurrentViewModel = mainVM;
        }

        var navigator = AppHost.Services.GetRequiredKeyedService<NavigatorStore>("default");
        var allItemsVM = AppHost.Services.GetRequiredService<DashBoardVM>();
        navigator.CurrentViewModel = allItemsVM;



        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}
