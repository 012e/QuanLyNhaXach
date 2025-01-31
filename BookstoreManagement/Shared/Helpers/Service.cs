﻿using BookstoreManagement.Core.Interface;
using BookstoreManagement.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace BookstoreManagement.Core.Helper;

internal class AbstractFactory<E>(Func<E> factory) : IAbstractFactory<E> where E : class
{
    private readonly Func<E> factory = factory;

    public E Create()
    {
        return factory();
    }
}

public static class Service
{

    public static void AddAbstractFactorySingleton<E>(this IServiceCollection services) where E : class
    {
        services.AddSingleton<E>();
        services.AddSingleton<Func<E>>(sp => () => sp.GetRequiredService<E>()!);
        services.AddSingleton<IAbstractFactory<E>, AbstractFactory<E>>();
    }

    public static void AddAbstractFactoryTransient<E>(this IServiceCollection services) where E : class
    {
        services.AddTransient<E>();
        services.AddSingleton<Func<E>>(sp => () => sp.GetRequiredService<E>()!);
        services.AddSingleton<IAbstractFactory<E>, AbstractFactory<E>>();
    }

    public static void AddNavigatorService<T>(this IServiceCollection services) where T : BaseViewModel
    {
        services.AddAbstractFactorySingleton<T>();
        services.AddSingleton<INavigatorService<T>, NavigatorService<T>>();
    }

    public static void AddContextualNavigatorService<TViewModel, TContext>(this IServiceCollection services)
        where TViewModel : BaseViewModel, IContextualViewModel<TContext>
    {
        services.AddAbstractFactorySingleton<TViewModel>();
        services.AddSingleton<IContextualNavigatorService<TViewModel, TContext>, ContextualNavigatorService<TViewModel, TContext>>();
    }

    public static void AddViewViewModel<TView, TViewModel>(this IServiceCollection services)
        where TView : ContentControl, new()
        where TViewModel : BaseViewModel
    {
        services.AddNavigatorService<TViewModel>();
        services.AddAbstractFactorySingleton<TViewModel>();
        services.AddSingleton<TView>(p =>
        {
            var view = new TView
            {
                DataContext = p.GetRequiredService<TViewModel>()!
            };
            return view;
        });
    }
    public static void AddViewContextualViewModel<TView, TViewModel, TContext>(this IServiceCollection services)
        where TView : ContentControl, new()
        where TViewModel : BaseViewModel, IContextualViewModel<TContext>
    {
        services.AddContextualNavigatorService<TViewModel, TContext>();
        services.AddAbstractFactorySingleton<TViewModel>();
        services.AddSingleton<TView>(p =>
        {
            var view = new TView
            {
                DataContext = p.GetRequiredService<TViewModel>()!
            };
            return view;
        });
    }

}
