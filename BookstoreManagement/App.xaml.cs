﻿using BookstoreManagement.DbContexts;
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

        builder.Services.AddSingleton<NavigatorStore>();

        AppHost = builder.Build();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        AppHost.Start();
        AppHost.Services.GetRequiredService<MainWindowV>().Show();

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
