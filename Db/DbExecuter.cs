using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseXml.Db
{
    class DbExecuter : IDbExecuter
    {
        public DbExecuter(string conStr)
        {
            _conStr = conStr;
        }

        ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(string sql, params DbParam[] ps)
        {
            using (OracleConnection con = new OracleConnection(_conStr))
            {
                foreach (var p in ps)
                    sql = sql.Replace("@" + p.Name, ParamPrefix + p.Name);

                try
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    throw new DbException(DbExceptionType.Connection, ex.Message, ex);
                }
                using (OracleCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    foreach (var p in ps)
                        cmd.Parameters.Add(p.Name, p.Value);
                    string commandText = CommandText(cmd);
                    log.Debug("Выполнение команды: " + commandText);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new DbException(DbExceptionType.Query, ex.Message, ex, commandText);
                    }
                }
            }
        }

        public string ParamPrefix
        {
            get { return ":"; }
        }

        private string CommandText(OracleCommand cmd)
        {
            string ps = "";
            foreach (OracleParameter p in cmd.Parameters)
            {
                if (ps.Length > 0)
                    ps += ", ";
                ps += p.ParameterName + "=" + p.Value;
            }

            return cmd.CommandText + (ps.Length > 0 ? "\nПараметры: " + ps : "");
        }

        string _conStr;
    }
}
