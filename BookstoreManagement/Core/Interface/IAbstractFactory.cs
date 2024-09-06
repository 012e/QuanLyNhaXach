using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Core.Interface;

public interface IAbstractFactory<T>
where T : class
{
    T Create(); 
}
