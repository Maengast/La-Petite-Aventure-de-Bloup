using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace DataBase
{
    public class BossDb
    {
        private const string Tag = "Bloup: BossDb:\t";
        private static SqliteHelper sqliteHelper = SqliteHelper.Instance;
        private const string TABLE_NAME = "Boss";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_LIFE_POINTS = "life_points";
        private const string KEY_ATTACK_MULTIPLIER = "attack_multiplier";
        private const string KEY_STAMINA = "stamina";
        private const string KEY_SPEED = "speed";
        private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_LIFE_POINTS, KEY_ATTACK_MULTIPLIER, KEY_SPEED};
        static BossDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NAME + " TEXT, " +
                KEY_LIFE_POINTS + " INTEGER, " +
                KEY_STAMINA + " REAL, " +
                KEY_SPEED + " REAL, " +
                KEY_ATTACK_MULTIPLIER + " REAL ) ";
            dbcmd.ExecuteNonQuery();
        }

        public static void AddBoss(BossInfo boss)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_LIFE_POINTS + ", "
                + KEY_STAMINA + ", "
                + KEY_SPEED + ", "
                + KEY_ATTACK_MULTIPLIER + " ) "

                + "VALUES ( '"
                + boss.Name + "', '"
                + boss.MaxLife + "', '"
                + boss.MaxStamina + "', '"
                + boss.Speed + "', '"
                + boss.Attack_Multiplier + "' )";
            dbcmd.ExecuteNonQuery();
        }

        public static BossInfo GetBossByName(string name)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_NAME + " = '" + name + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                BossInfo boss = ReadBoss(reader);
                reader.Close();
                return boss;
            }
            throw new KeyNotFoundException();
        }

        public static List<BossInfo> GetAllBoss()
        {
            IDataReader reader = sqliteHelper.GetAllData(TABLE_NAME);
            List<BossInfo> AllBoss = new List<BossInfo>();
            while (reader.Read())
            {
                BossInfo boss = ReadBoss(reader);

                AllBoss.Add(boss);
            }
            reader.Close();
            return AllBoss;
        }


        private static BossInfo ReadBoss(IDataReader reader)
        {
            BossInfo boss = new BossInfo();
            boss.Name = reader.GetString(1);
            boss.MaxLife = reader.GetInt32(2);
            boss.MaxStamina = reader.GetFloat(3);
            boss.Speed = reader.GetFloat(4);
            boss.Attack_Multiplier = reader.GetFloat(5);
            return boss;
        }
    }
}


