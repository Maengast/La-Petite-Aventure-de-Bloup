using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;

namespace DataBase
{
    public class PlayerDb
    {
        private const string Tag = "Bloup: PlayersDb:\t";
        private static SqliteHelper sqliteHelper = new SqliteHelper();
        private const string TABLE_NAME = "Player";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_LIFE_POINTS = "life_points";
        private const string KEY_ATTACK_MULTIPLIER = "attack_multiplier";
        private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_LIFE_POINTS, KEY_ATTACK_MULTIPLIER };

        static PlayerDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NAME + " TEXT, " +
                KEY_LIFE_POINTS + " INTEGER, " +
                KEY_ATTACK_MULTIPLIER + " INTEGER ) ";
            dbcmd.ExecuteNonQuery();
        }

        public void AddPlayer(Player player)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_LIFE_POINTS + ", "
                + KEY_ATTACK_MULTIPLIER + " ) "

                + "VALUES ( '"
                + player.Name + "', '"
                + player.MaxLife + "', '"
                + player.Attack_Multiplier + "' )";
            dbcmd.ExecuteNonQuery();
        }

        public Player GetPlayerById(int id)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Player player = ReadPlayer(reader);
                reader.Close();
                return player;
            }
            throw new KeyNotFoundException();

        }

        public Player GetPlayerByName(string name)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_NAME + " = '" + name + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                Player player = ReadPlayer(reader);
                reader.Close();
                return player;
            }
            throw new KeyNotFoundException();
        }


        private Player ReadPlayer(IDataReader reader)
        {
            Player player = new Player();
            player.Id = reader.GetInt32(0);
            player.Name = reader.GetString(1);
            player.MaxLife = reader.GetInt32(2);
            player.Attack_Multiplier = reader.GetInt32(3);
            player.Attacks = AttackDb.GetAttacksByIDCharacter(player.Id);
            return player;
        }
    }

}