using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace DataBase
{
    public class BossDb
    {
        private const string Tag = "Bloup: BossDb:\t";
        private static SqliteHelper sqliteHelper = new SqliteHelper();
        private const string TABLE_NAME = "Player";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_LIFE_POINTS = "life_points";
        private const string KEY_ATTACK_MULTIPLIER = "attack_multiplier";
        private const string KEY_STAMINA = "stamina";
        private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_LIFE_POINTS, KEY_ATTACK_MULTIPLIER };
        static BossDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            Debug.Log(dbcmd.CommandText);
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NAME + " TEXT, " +
                KEY_ATTACK_MULTIPLIER + " INTEGER, " +
                KEY_LIFE_POINTS + " INTEGER, " +
                KEY_STAMINA + " FLOAT ) ";
            dbcmd.ExecuteNonQuery();
        }

        public static void AddBoss(Boss boss)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_LIFE_POINTS + ", "
                + KEY_STAMINA + ", "
                + KEY_ATTACK_MULTIPLIER + " ) "

                + "VALUES ( '"
                + boss.Name + "', '"
                + boss.MaxLife + "', '"
                + boss.MaxStamina + "', '"
                + boss.Attack_Multiplier + "' )";
            dbcmd.ExecuteNonQuery();
        }

        public static Boss GetBossById(int id)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Boss boss = ReadBoss(reader);
                reader.Close();
                return boss;
            }
            throw new KeyNotFoundException();

        }

        public static Boss GetBossByName(string name)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_NAME + " = '" + name + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Boss boss = ReadBoss(reader);
                reader.Close();
                return boss;
            }
            throw new KeyNotFoundException();
        }


        private static Boss ReadBoss(IDataReader reader)
        {
            Boss boss = new Boss();
            boss.Id = reader.GetInt32(0);
            boss.Name = reader.GetString(1);
            boss.Attack_Multiplier = reader.GetInt32(2);
            boss.MaxLife = reader.GetInt32(3);
            boss.MaxStamina = reader.GetFloat(4);
            boss.Attacks = AttackDb.GetAttacksByIDCharacter(boss.Id);
            return boss;
        }
    }
}


