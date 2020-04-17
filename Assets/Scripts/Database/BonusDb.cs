using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace DataBase
{
    public class BonusDb
    {

        private const string Tag = "Bloup: BossDb:\t";
        private static SqliteHelper sqliteHelper = SqliteHelper.Instance;
        private const string TABLE_NAME = "Player";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_MULTIPLIER = "multplier";
        private const string KEY_IMPROVED_FEATURE = "improved_feature";
        private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_MULTIPLIER, KEY_IMPROVED_FEATURE };

        static BonusDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NAME + " TEXT, " +
                KEY_MULTIPLIER + " INTEGER, " +
                KEY_IMPROVED_FEATURE + " TEXT ) ";
            dbcmd.ExecuteNonQuery();
        }

        public static void AddBonus(BonusInfo bonus)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_MULTIPLIER + ", "
                + KEY_IMPROVED_FEATURE + " ) "

                + "VALUES ( '"
                + bonus.Name + "', '"
                + bonus.Multipler + "', '"
                + bonus.Improved_Features + "' )";
            dbcmd.ExecuteNonQuery();
        }

        public static BonusInfo GetBonusById(int id)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                BonusInfo bonus = ReadBonus(reader);
                reader.Close();
                return bonus;
            }
            throw new KeyNotFoundException();

        }

        public static List<BonusInfo> GetAllBonus()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME ;
            IDataReader reader = dbcmd.ExecuteReader();
            List<BonusInfo> bonusList = new List<BonusInfo>();
            while (reader.Read())
            {
                BonusInfo bonus = ReadBonus(reader);
                bonusList.Add(bonus);
            }
            reader.Close();
            return bonusList;

        }

        public static BonusInfo GetBonusByName(string name)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_NAME + " = '" + name + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                BonusInfo bonus = ReadBonus(reader);
                reader.Close();
                return bonus;
            }
            throw new KeyNotFoundException();
        }


        private static BonusInfo ReadBonus(IDataReader reader)
        {
            BonusInfo bonus = new BonusInfo();
            bonus.Id = reader.GetInt32(0);
            bonus.Name = reader.GetString(1);
            bonus.Multipler = reader.GetInt32(2);
            bonus.Improved_Features = reader.GetString(3);
            return bonus;
        }
    }

}
