using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

namespace DataBase
{
    public class LevelDb
    {
        private const String Tag = "Bloup: LevelDb:\t";
        private static SqliteHelper sqliteHelper = SqliteHelper.Instance;
        private const String TABLE_NAME = "Level";
        private const String KEY_ID = "id";
        private const String KEY_NUMBER = "number";
        private const String KEY_TRAPS_COUNT = "traps_count";
        private const String KEY_ENEMIES_COUNT = "enemies_count";
        private const String KEY_CHESTS_COUNT = "chests_count";
        private const String KEY_MAP_SIZE = "map_size";
        private String[] COLUMNS = new String[] { KEY_ID, KEY_NUMBER, KEY_TRAPS_COUNT, KEY_ENEMIES_COUNT, KEY_CHESTS_COUNT, KEY_MAP_SIZE };

        static LevelDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NUMBER + " INTEGER, " +
                KEY_TRAPS_COUNT + " INTEGER, " +
                KEY_ENEMIES_COUNT + " INTEGER, " +
                KEY_CHESTS_COUNT + " INTEGER, " +
                KEY_MAP_SIZE + " REAL ) ";
            dbcmd.ExecuteNonQuery();
        }


        public static void AddLevel(Level level)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NUMBER + ", "
                + KEY_TRAPS_COUNT + ", "
                + KEY_ENEMIES_COUNT + ", "
                + KEY_CHESTS_COUNT + ", "
                + KEY_MAP_SIZE + " ) "

                + "VALUES ( '"
                + level.Number + "', '"
                + level.Traps_Count + "', '"
                + level.Enemies_Count + "', '"
                + level.Chests_Count + "', '"
                + level.Map_Size + "' )";
            dbcmd.ExecuteNonQuery();
            Debug.Log("Niveau ajouté!");
        }

        public static Level GetLevelById(int id)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Level level = ReadLevel(reader);
                reader.Close();
                return level;
            }
            throw new KeyNotFoundException();
           
        }

        public static Level GetLevelByNumber(int number)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_NUMBER + " = '" + number + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Level level = ReadLevel(reader);
                reader.Close();
                return level;
            }
            throw new KeyNotFoundException();
        }

        public static void DeleteAllLevel()
        {
            sqliteHelper.DeleteAllData(TABLE_NAME);
        }

        public static List<Level> GetAllLevels()
        {
            IDataReader reader = sqliteHelper.GetAllData(TABLE_NAME);
            List<Level> levels = new List<Level>();
            while (reader.Read())
            {
                Level level = ReadLevel(reader);
                levels.Add(level);
            }
            reader.Close();
            return levels;
        }

        private static Level ReadLevel(IDataReader reader)
        {
            Level level = new Level();
            level.Number = reader.GetInt32(1);
            level.Traps_Count = reader.GetInt32(2);
            level.Enemies_Count = reader.GetInt32(3);
            level.Chests_Count = reader.GetInt32(4);
            level.Map_Size = reader.GetFloat(5);
            return level;
        }


    }
}

