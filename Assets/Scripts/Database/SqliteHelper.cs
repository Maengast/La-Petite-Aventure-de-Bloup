using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;


namespace DataBase
{
    public class SqliteHelper
    {
        private const string Tag = "Bloup: SqliteHelper:\t";

        private const string database_name = "bloup_db";

        private static IDbConnection db_connection;

        private static SqliteHelper instance;
        SqliteHelper()
        {
            DbConnection connection = DbConnection.DbConnectionInstance;
            db_connection = connection.SqlConnetionFactory;
            db_connection.Open();
        }

        ~SqliteHelper()
        {
            db_connection.Close();
        }
        public static SqliteHelper Instance
        {
            get
            {
               if (instance == null)
               {
                 instance = new SqliteHelper();
               }
              return instance;
                
            }

        }
        public IDbCommand GetDbCommand()
        {
            return db_connection.CreateCommand();
        }

        public IDataReader GetAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + table_name;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        public virtual void DeleteAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + table_name;
            dbcmd.ExecuteNonQuery();
        }


        public void Close()
        {
            db_connection.Close();
        }
    }
}

