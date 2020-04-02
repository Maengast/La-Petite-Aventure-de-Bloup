using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

namespace DataBase
{
    class DbConnection
    {
        private const string database_name = "bloup_db";
        private static DbConnection instance;
        private static readonly object padlock = new object();
        private string db_connection_string;
        private static SqliteConnection sqlConnection;

        DbConnection()
        {
            db_connection_string = "URI=file:" + Application.persistentDataPath + "/" + database_name;
            sqlConnection = new SqliteConnection(db_connection_string);
        }
        public SqliteConnection SqlConnetionFactory
        {
            get { return sqlConnection; }
        }

        public static DbConnection DbConnectionInstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DbConnection();
                    }
                    return instance;
                }
            }

        }

    }
}

