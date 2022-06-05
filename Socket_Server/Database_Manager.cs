﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

public class Database_Manager
{
    public static Database_Manager instance;

    //String con los datos de conexion
    private const string connectionString = "Server=db4free.net;Port=3306;database=unitymultiplayer;Uid=guille_barrasa;password=entienti;SSL Mode=None;connect timeout=3600;default command timeout = 3600;";

    //Clase encargada de la conexion
    private MySqlConnection connection;

    public Database_Manager()
    {
        if(instance == null)
        {
            instance = this;
        }

         connection = new MySqlConnection(connectionString);
    }


    public string Login(string user, string pass)
    {
        try
        {
            //Abro conexion
            connection.Open();
        }
        catch (Exception ex)
        {
            //Gestiono posibles errores de conexión.
            Console.WriteLine(ex.Message);
        }

        //Creo el reader para leer los datos
        MySqlDataReader reader;

        //Creo la instruccion que quiero ejecutar de SQL (clase)
        MySqlCommand command = connection.CreateCommand();

        Console.WriteLine(user + " " + pass);

        //Añado en el atributo de la clase la query a realizar
        command.CommandText = "SELECT id_user FROM users WHERE nick='"+user+"' and pass='"+pass+"';";

        try
        {
            //Ejecuto la query (SQL es asincrono, respondera cuando quiera), interrumpe el programa hasta recibir respuesta.
            reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                connection.Close();
                return "True/" + user;
            }
            else
            {
                connection.Close();
                return "False";
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex);

            connection.Close();
            return "False";
        }

    
    }

    public bool Register(string nick, string pass, int race)
    {
        try
        {
            //Abro conexion
            connection.Open();
        }
        catch (Exception ex)
        {
            //Gestiono posibles errores de conexión.
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("SE ESTÁ REGISTRANDO UN USER");

        //Creo la instruccion que quiero ejecutar de SQL (clase)
        MySqlCommand command = connection.CreateCommand();

        //Añado en el atributo de la clase la query a realizar
        command.CommandText = "INSERT INTO users (nick, pass, race_id) VALUES ('" + nick + "', '" + pass + "', " + race + ");";

        try
        {
            //Ejecuto la instrucción, el NonQuery() evita que el programa se interrumpa y espere respuesta de SQL
            command.ExecuteNonQuery();
            connection.Close();
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            connection.Close();

            return false;
        }
    } 
    
    public string GetRacesData()
    {
        try
        {
            //Abro conexion
            connection.Open();
        }
        catch (Exception ex)
        {
            //Gestiono posibles errores de conexión.
            Console.WriteLine(ex.Message);
        }

        //Creo el reader para leer los datos
        MySqlDataReader reader;

        //Creo la instruccion que quiero ejecutar de SQL (clase)
        MySqlCommand command = connection.CreateCommand();

        //Añado en el atributo de la clase la query a realizar
        command.CommandText = "SELECT * FROM races;";

        string data = "";

        try
        {
            //Ejecuto la query (SQL es asincrono, respondera cuando quiera), interrumpe el programa hasta recibir respuesta.
            reader = command.ExecuteReader();

            //reader.Read() devuelve true mientras haya algo que leer.
            while (reader.Read())
            {
                data += reader.GetString("id_race") + "|";
                data += reader.GetString("health") + "|";
                data += reader.GetString("damage") + "|";
                data += reader.GetString("speed") + "|";
                data += reader.GetString("jump") + "|";
                data += reader.GetString("firerate") + "|";
                data += reader.GetString("race_name");

                data += "&";
            }


            connection.Close();
            return data;
           
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex);

            connection.Close();
            return data;
        }
    }

    public string GetRacesOnGame(string user1, string user2)
    {
        string users = "";

        try
        {
            //Abro conexion
            connection.Open();
        }
        catch (Exception ex)
        {
            //Gestiono posibles errores de conexión.
            Console.WriteLine(ex.Message);
        }

        //Creo el reader para leer los datos
        MySqlDataReader reader1;

        //Creo la instruccion que quiero ejecutar de SQL (clase)
        MySqlCommand command1 = connection.CreateCommand();

        //Añado en el atributo de la clase la query a realizar
        command1.CommandText = "SELECT race_id FROM users WHERE nick='" + user1 + "';";


        try
        {
            //Ejecuto la query (SQL es asincrono, respondera cuando quiera), interrumpe el programa hasta recibir respuesta.
            reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                users += reader1.GetString("race_id");
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex);

            connection.Close();
            return "False";
        }


        connection.Close();

        users += "/";

        connection.Open();

        MySqlDataReader reader2;

        MySqlCommand command2 = connection.CreateCommand();

        //Añado en el atributo de la clase la query a realizar
        command2.CommandText = "SELECT race_id FROM users WHERE nick='" + user2 + "';";


        try
        {
            //Ejecuto la query (SQL es asincrono, respondera cuando quiera), interrumpe el programa hasta recibir respuesta.
            reader2 = command2.ExecuteReader();

            while (reader2.Read())
            {
                Console.WriteLine("ID USUARIO 2: " + reader2.GetString("race_id"));
                users += reader2.GetString("race_id");
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex);

            connection.Close();
            return "False";
        }

        Console.WriteLine("ID DE LOS USERS: " + users);
        connection.Close();
        return users;

    }
}
