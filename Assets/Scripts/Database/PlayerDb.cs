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
        private static SqliteHelper sqliteHelper = SqliteHelper.Instance;
        private const string TABLE_NAME = "Player";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_LIFE_POINTS = "life_points";
        private const string KEY_ATTACK_MULTIPLIER = "attack_multiplier";
        private const string KEY_SPEED = "speed";
        private const string KEY_JUMP_HEIGHT = "jump_height";
        private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_LIFE_POINTS, KEY_ATTACK_MULTIPLIER, KEY_SPEED, KEY_JUMP_HEIGHT };
		
        /**
         * Construct Player Table
         */
        static PlayerDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NAME + " TEXT, " +
                KEY_LIFE_POINTS + " INTEGER, " +
                KEY_ATTACK_MULTIPLIER + " NUMERIC, " +
                KEY_SPEED + " NUMERIC ," +
                KEY_JUMP_HEIGHT + " NUMERIC )";
            dbcmd.ExecuteNonQuery();
        }
	
        /**
         * Add Player in db
         */
        public static void AddPlayer(PlayerInfo player)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_LIFE_POINTS + ", "
                + KEY_ATTACK_MULTIPLIER + ", "
				+ KEY_SPEED + ", "
	            + KEY_JUMP_HEIGHT + ")"

                + "VALUES ( '"
                + player.Name + "', '"
                + player.MaxLife + "', '"
                + player.Attack_Multiplier + "', '"
                + player.Speed + "', '"
                + player.JumpHeight + "' )";
            dbcmd.ExecuteNonQuery();
        }
		
        /**
         * Get player from db
         */
        public static PlayerInfo GetPlayer()
        {
            IDataReader reader = sqliteHelper.GetAllData(TABLE_NAME);
            while (reader.Read())
            {
                PlayerInfo player = ReadPlayer(reader);
                reader.Close();
                return player;
            }

            return null;
        }

		/**
		 * Reader to convert data in player info class
		 */
        private static PlayerInfo ReadPlayer(IDataReader reader)
        {
            PlayerInfo player = new PlayerInfo();
            player.Name = reader.GetString(1);
            player.MaxLife = reader.GetInt32(2);
            player.Attack_Multiplier = reader.GetFloat(3);
            player.Speed = reader.GetFloat(4);
            player.JumpHeight = reader.GetFloat(5);
            return player;
        }
    }

}