using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.UI.ItemUI;
using BookstoreManagement.UI.MainWindow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BookstoreManagement.Core.Helper;
using BookstoreManagement.Services;
using dotenv.net.Utilities;
using Microsoft.EntityFrameworkCore;
using dotenv.net;
using Microsoft.Extensions.Options;
using BookstoreManagement.UI.InvoicesUI;
using BookstoreManagement.UI;
using BookstoreManagement.UI.TagUI;
using BookstoreManagement.UI.DashboardUI;
using BookstoreManagement.UI.CustomerUI;
using BookstoreManagement.UI.PromotionUI;

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

        builder.Services.AddViewViewModel<MainWindowView, MainWindowVM>();

        builder.Services.AddViewViewModel<AllItemsView, AllItemsVM>();
        builder.Services.AddViewContextualViewModel<EditItemView, EditItemVM, Item>();
        builder.Services.AddViewViewModel<CreateItemView, CreateItemVM>();

        builder.Services.AddViewViewModel<AllInvoicesView, AllInvoicesVM>();
        builder.Services.AddViewContextualViewModel<EditInvoiceView, EditInvoiceVM, Invoice>();
        builder.Services.AddViewContextualViewModel<AddInvoiceItemView, AddInvoiceItemVM, Invoice>();
        builder.Services.AddViewViewModel<CreateInvoiceView, CreateInvoiceVM>();

        builder.Services.AddViewViewModel<AllTagsView, AllTagsVM>();    
        builder.Services.AddViewContextualViewModel<EditTagView, EditTagVM, Tag>();
        builder.Services.AddViewViewModel<CreateTagView, CreateTagVM>();

        builder.Services.AddSingleton<NavigatorStore>();

        // dashboard
        builder.Services.AddViewViewModel<DashBoardV, DashBoardVM>();
        //builder.Services.AddViewContextualViewModel<>

        // Construct View, VM Customer
        builder.Services.AddViewViewModel<CustomerV, CustomerVM>();

        // edit customer
        builder.Services.AddViewContextualViewModel<EditCustomerView, EditCustomerVM, int>();

        // Create Customer
         builder.Services.AddViewViewModel<CreateCustomerView, CreateCustomerVM>();


        // Construct View, ViewModel Promotion
        builder.Services.AddViewViewModel<PromotionView,PromotionVM>();
        AppHost = builder.Build();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        AppHost.Start();
        AppHost.Services.GetRequiredService<MainWindowView>().Show();

        var navigator = AppHost.Services.GetRequiredService<NavigatorStore>();
        var allItemsVM = AppHost.Services.GetRequiredService<AllItemsVM>();
        navigator.CurrentViewModel = allItemsVM;

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}
