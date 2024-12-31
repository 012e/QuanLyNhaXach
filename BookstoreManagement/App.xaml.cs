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
using System.Windows;
using System.Windows.Navigation;
using BookstoreManagement.PricingUI;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.PricingUI.Dtos;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.SettingUI;
using Supabase;

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

        builder.Services.AddSingleton<Supabase.Client>(provider =>
        {
            return new Supabase.Client(
                Environment.GetEnvironmentVariable("SUPABASE_URL"),
                Environment.GetEnvironmentVariable("SUPABASE_KEY"),
                new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = true,
                }
            );
        });

        builder.Services.AddSingleton<ImageUploader>();
        builder.Services.AddHostedService<BucketSetupService>();

        builder.Services.AddViewViewModel<MainWindowV, MainWindowVM>();

        builder.Services.AddViewViewModel<AllItemsV, AllItemsVM>();
        builder.Services.AddViewContextualViewModel<EditItemV, EditItemVM, Item>();
        builder.Services.AddViewViewModel<CreateItemV, CreateItemVM>();

        builder.Services.AddViewViewModel<AllInvoicesV, AllInvoicesVM>();
        builder.Services.AddViewContextualViewModel<EditInvoiceV, EditInvoiceVM, Invoice>();
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


        builder.Services.AddViewViewModel<AllPricingV, AllPricingVM>();
        builder.Services.AddViewContextualViewModel<EditPricingV, EditPricingVM, PricingResponseDto>();
        builder.Services.AddSingleton<PricingService>();

        builder.Services.AddViewViewModel<DashBoardV, DashBoardVM>();
        builder.Services.AddViewViewModel<MainV, MainVM>();

        builder.Services.AddViewViewModel<LoginV, LoginVM>();
        builder.Services.AddSingleton<LoginService>();
        builder.Services.AddSingleton<CurrentUserService>();

        builder.Services.AddViewViewModel<DashBoardV, DashBoardVM>();
        builder.Services.AddViewViewModel<AllSettingV, AllSettingVM>();
        builder.Services.AddViewViewModel<MyProfileV, MyProfileVM>();
        builder.Services.AddViewViewModel<AccountV, AccountVM>();
        builder.Services.AddViewViewModel<AllNotificationV, AllNotificationVM>();
        builder.Services.AddKeyedSingleton<NavigatorStore>("default");
        builder.Services.AddKeyedSingleton<NavigatorStore>("global");
        builder.Services.AddKeyedSingleton<NavigatorStore>("setting");

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


        if (NeedLogin())
        {
            var login = AppHost.Services.GetRequiredService<INavigatorService<LoginVM>>();
            login.Navigate("global");
        }
        else
        {
            var mainVM = AppHost.Services.GetRequiredService<INavigatorService<MainVM>>();
            var currentUser = AppHost.Services.GetRequiredService<CurrentUserService>();
            var db = AppHost.Services.GetRequiredService<ApplicationDbContext>();
            currentUser.CurrentUser = db.Employees.Find(1);
            mainVM.Navigate("global");
        }

        var dashboard = AppHost.Services.GetRequiredService<INavigatorService<DashBoardVM>>();
        dashboard.Navigate();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}
