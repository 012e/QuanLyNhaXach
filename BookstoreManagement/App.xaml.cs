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
        builder.Services.AddViewViewModel<EditItemView, EditItemVM>();
        builder.Services.AddViewViewModel<CreateItemView, CreateItemVM>();
        builder.Services.AddNavigatorService<AllItemsVM>();
        builder.Services.AddNavigatorService<CreateItemVM>();
        builder.Services.AddContextualNavigatorService<EditItemVM, int>();


        builder.Services.AddSingleton<NavigatorStore>();

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
