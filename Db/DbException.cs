using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseXml.Db
{
    enum DbExceptionType
    {
        Connection,
        Query
    }

    class DbException : Exception
    {
        public DbExceptionType Type { get; private set; }
        public string CommandText { get; private set; }

        public DbException(DbExceptionType type, string message, Exception inner, string commandText = null) : base(message, inner)
        {
            Type = type;
            CommandText = commandText;
        }
    }
}
