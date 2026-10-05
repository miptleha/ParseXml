using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseXml.Db
{
    interface IDbExecuter
    {
        void Execute(string sql, params DbParam[] ps);
        string ParamPrefix { get; }
    }
}
