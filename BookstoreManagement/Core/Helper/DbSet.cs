using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Core.Helper;

public static class DbContextExtension
{
    public static IQueryable<T> Set<T>(this DbContext context) where T : class
    {
        // Get the generic type definition 
        MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance)!;

        // Build a method with the specific type argument you're interested in 
        method = method.MakeGenericMethod(typeof(T));

        return method.Invoke(context, null) as IQueryable<T>;
    }
}
