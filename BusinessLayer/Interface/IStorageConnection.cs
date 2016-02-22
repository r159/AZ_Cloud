using BusinessLayer.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IStorageConnection
    {
        //storagecredentials CheckvalidStorageAcc(string acckey, string accname);

        bool CheckvalidStorageAcc(string acckey, string accname); 
    }
}
