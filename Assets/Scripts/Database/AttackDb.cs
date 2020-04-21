using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace DataBase
{
    public class AttackDb
    {
        private const string Tag = "Bloup: AttackDb:\t";
        private static SqliteHelper sqliteHelper = SqliteHelper.Instance;
        private const string TABLE_NAME = "Attack";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_DAMAGES= "damages";
        private const string KEY_LEVEL = "level";
        private const string KEY_DESCRIPTION = "description";
        private const string ID_CHARACTER = "id_character";
        private const string KEY_TYPE = "type";
        private const string KEY_COOL_DOWN = "cool_down";
        private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_DAMAGES, KEY_LEVEL, KEY_DESCRIPTION, KEY_TYPE, KEY_COOL_DOWN ,ID_CHARACTER };


        static AttackDb()
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                KEY_NAME + " TEXT, " +
                KEY_DAMAGES + " INTEGER, " +
                KEY_LEVEL + " INTEGER, " +
                KEY_DESCRIPTION + " TEXT, " +
                KEY_TYPE + " TEXT, " +
                KEY_COOL_DOWN + " FLOAT, " +
                ID_CHARACTER + " INTEGER ) ";
            dbcmd.ExecuteNonQuery();
        }


        public static void AddAttack(AttackModel attack)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_DAMAGES + ", "
                + KEY_LEVEL + ", "
                + KEY_DESCRIPTION + ", " 
                + KEY_TYPE + ", "
                + KEY_COOL_DOWN + " ) "

                + "VALUES ( '"
                + attack.Name + "', '"
                + attack.Damage + "', '"
                + attack.Level + "', '"
                + attack.Description + "', '"
                + attack.Type + "', '"
                + attack.Cool_Down + "' )";
            dbcmd.ExecuteNonQuery();
        }

        public static List<AttackModel> GetAllAttacks()
        {
       
            IDataReader reader = sqliteHelper.GetAllData(TABLE_NAME);
            List<AttackModel> attacks = new List<AttackModel>();
            while (reader.Read())
            {
                AttackModel attack = ReadAttack(reader);
                attacks.Add(attack);
            }
            reader.Close();
            return attacks;

        }
        public static List<AttackModel> GetAttacksByIDCharacter(int id_character)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + ID_CHARACTER + " = '" + id_character + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            List<AttackModel> attacks = new List<AttackModel>();
            while (reader.Read())
            {
                AttackModel attack = ReadAttack(reader);
                attacks.Add(attack);
            }
            reader.Close();
            return attacks;

        }

        public static AttackModel GetAttackByName(string name)
        {
            IDbCommand dbcmd = sqliteHelper.GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_NAME + " = '" + name + "'";
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                AttackModel attack = ReadAttack(reader);
                reader.Close();
                return attack;
            }
            throw new KeyNotFoundException();
        }


        private static AttackModel ReadAttack(IDataReader reader)
        {
            AttackModel attack = new AttackModel();
            attack.Name = reader.GetString(1);
            attack.Damage = reader.GetInt32(2);
            attack.Level = reader.GetInt32(3);
            attack.Description = reader.GetString(4);
            //attack.Type = reader.GetString(5);
            attack.Cool_Down = reader.GetFloat(6);
            return attack;
        }

    }
}

