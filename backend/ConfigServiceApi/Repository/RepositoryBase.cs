using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigServiceApi.Utils;

namespace ConfigServiceApi.Repository
{
    public class RepositoryBase
    {
        public OrmHelper Orm = new OrmHelper();
    }
}
