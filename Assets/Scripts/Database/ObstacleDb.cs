using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

namespace DataBase
{

    public class ObstaclesDb
    {
        private const String Tag = "Bloup: ObstaclesDb:\t";
        private static SqliteHelper sqliteHelper = SqliteHelper.Instance;
        private const String TABLE_NAME = "Obstacle";
        private const String KEY_ID = "id";
        private const String KEY_IS_TRAP = "is_trap";
        private const String KEY_NAME = "name";
        private const String KEY_WICKEDNESS_INDEX = "wickedness_index";
        private String[] COLUMNS = new String[] { KEY_ID, KEY_NAME, KEY_IS_TRAP, KEY_WICKEDNESS_INDEX };

        static ObstaclesDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            Debug.Log(dbcmd.CommandText);
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NAME + " TEXT, " +
                KEY_IS_TRAP + " BOOL, " +
                KEY_WICKEDNESS_INDEX + " INTEGER ) ";
            dbcmd.ExecuteNonQuery();
        }

        public static void AddObstacle(Obstacle obstacle)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_IS_TRAP + ", "
                + KEY_WICKEDNESS_INDEX + " ) "

                + "VALUES ( '"
                + obstacle.Name + "', '"
                + obstacle.Is_Trap + "', '"
                + obstacle.Wickedness_Index + "' )";
            dbcmd.ExecuteNonQuery();
            Debug.Log("Obtacle ajouté!");
        }

        public static Obstacle GetObstacleById(int id)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Obstacle obstacle = ReadObstacle(reader);
                reader.Close();
                return obstacle;
            }
            throw new KeyNotFoundException();

        }

        public static Obstacle GetObstacleByName(string name)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_NAME + " = '" + name + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Obstacle obstacle = ReadObstacle(reader);
                reader.Close();
                return obstacle;
            }
            throw new KeyNotFoundException();
        }

        public static void DeleteAllObstacle()
        {
            sqliteHelper.DeleteAllData(TABLE_NAME);
        }

        public static List<Obstacle> GetAllLevels()
        {
            IDataReader reader = sqliteHelper.GetAllData(TABLE_NAME);
            List<Obstacle> obstacles = new List<Obstacle>();
            while (reader.Read())
            {
                Obstacle obstacle = ReadObstacle(reader);
                obstacles.Add(obstacle);
            }
            reader.Close();
            return obstacles;
        }

        private static Obstacle ReadObstacle(IDataReader reader)
        {
            Obstacle obstacle = new Obstacle();
            obstacle.Name = reader.GetString(1);
            obstacle.Is_Trap = reader.GetBoolean(2);
            obstacle.Wickedness_Index = reader.GetInt32(3);
            return obstacle;
        }
    }
}

  


 


