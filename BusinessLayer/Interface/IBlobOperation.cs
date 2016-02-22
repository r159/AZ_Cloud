using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IBlobOperation
    {
        static Dictionary<string, DateTimeOffset> GetContainerList(string accname, string acckey);
    }
}
