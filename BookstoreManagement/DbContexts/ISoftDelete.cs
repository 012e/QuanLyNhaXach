using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.DbContexts;

public interface ISoftDelete
{
    public bool Deleted { get; set; }
}
