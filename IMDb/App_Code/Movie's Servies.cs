﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Data;

/// <summary>
/// Summary description for Movie_s_Servies
/// </summary>
public class Movie_s_Servies
{
    DataSet ds;//בונה דטה סט
    OleDbCommand Command;//פקדות בסיס הנתונים
    OleDbConnection Connection;// מקשר בין הקוד לבסיס הנתונים
    OleDbDataAdapter Adapter;//מקשר בין בסיס הנתונים לדטא סט
    OleDbTransaction tr;//מקשר בין פקודות
	public Movie_s_Servies()
	{
		Connection= new OleDbConnection(Connect.GetInfo());
	}//פעולה בונה
    public DataSet GetAllTheCatlog()//טענת יציאה: שולח את התמונה והשם ומספר הid של סרט
	{
        ds = new DataSet();
        try
        {
            Connection.Open();
            Command = new OleDbCommand("ShowCatalog",Connection);
            Command.CommandType= CommandType.StoredProcedure;
            Adapter = new OleDbDataAdapter(Command);
            Adapter.Fill(ds,"MoviesTbl");
            ds.Tables["MoviesTbl"].PrimaryKey = new DataColumn[] { ds.Tables["MoviesTbl"].Columns["MovieId"] };
            return ds;

        }
        catch(Exception Err){throw Err;}
        finally{Connection.Close();}
    }
    public MovieDetails GetMovieDetails(int MovieID)
    {
         ds = new DataSet();
         MovieDetails Movie = new MovieDetails();
        try
    {
        Connection.Open();
        tr = Connection.BeginTransaction();
        Command = new OleDbCommand("GetMoivesDetails",Connection);
            Command.CommandType= CommandType.StoredProcedure;
            Command.Transaction = tr;
            OleDbParameter parm;
            parm = Command.Parameters.Add("@MovieId", OleDbType.Integer);
            parm.Value = MovieID;
            Adapter = new OleDbDataAdapter(Command);
            Adapter.Fill(ds,"MoviesTbl");
            Movie.MovieId = MovieID;
            Movie.MoivesName = ds.Tables["MoviesTbl"].Rows[0]["MoiveName"].ToString();
            Movie.TrailerAdd = ds.Tables["MoviesTbl"].Rows[0]["TrailerAdd"].ToString();
            Movie.Addres = ds.Tables["MoviesTbl"].Rows[0]["Addres"].ToString();
            Movie.YearRate = ds.Tables["MoviesTbl"].Rows[0]["YearRate"].ToString();
            Movie.Category = ds.Tables["MoviesTbl"].Rows[0]["CategoryName"].ToString();
            Movie.Date = DateTime.Parse(ds.Tables["MoviesTbl"].Rows[0]["Date"].ToString());
            Movie.description = ds.Tables["MoviesTbl"].Rows[0]["description"].ToString();
            Movie.UrlPicture = ds.Tables["MoviesTbl"].Rows[0]["UrlPicture"].ToString();
            ds = new DataSet();
            ds = GetCharacter(ds, MovieID);
            ds = GetRevew(ds, MovieID);
            Movie.PlayersNRevive = ds;
            tr.Commit();
            return Movie;
    }
        catch (Exception err) { throw err; tr.Rollback(); }
        finally { Connection.Close(); }
    }
	//טענת כניסה: מקבל id של סרט
	// אענת יציאה:חזיר את כל הפרטים על הסרט המקובל
	public DataSet GetCharacter(DataSet ds,int MovieID)
    {
        try
        {
            Command = new OleDbCommand("ShowPlayersOnMoive", Connection);
            Command.CommandType = CommandType.StoredProcedure;
            Command.Transaction = tr;
            OleDbParameter parm;
            parm = Command.Parameters.Add("@MovieId", OleDbType.Integer);
            parm.Value = MovieID;
            Adapter = new OleDbDataAdapter(Command);
            Adapter.Fill(ds, "PlayersTBbl");
            return ds;
        }
        catch (Exception err) { throw err; }
    }
	//טעת כניסה: מקבל מספר סרט
	// טענת יציאה: מחיז טבלה של שחקנים
	public DataSet GetRevew(DataSet ds, int MovieID)
    {
        try
        {
            Command = new OleDbCommand("GetRevivewOnMovie", Connection);
            Command.CommandType = CommandType.StoredProcedure;
            Command.Transaction = tr;
            OleDbParameter parm;
            parm = Command.Parameters.Add("@MovieId", OleDbType.Integer);
            parm.Value = MovieID;
            Adapter = new OleDbDataAdapter(Command);
            Adapter.Fill(ds, "ReviewsTBbl");
            return ds;
        }
        catch (Exception err) { throw err; }
    }
	//טעת כניסה: מקבל מספר סרט
	// טענת יציאה: מחיז טבלה של ביקורות
	public string GetUrlAddres(int MovieId)
	{
		try
		{
			Connection.Open();
			Command = new OleDbCommand("GetAdders", Connection);
			Command.CommandType = CommandType.StoredProcedure;
			Command.Transaction = tr;
			OleDbParameter parm;
			parm = Command.Parameters.Add("@MovieId", OleDbType.BSTR);
			parm.Value = MovieId;
			return Command.ExecuteScalar().ToString();
		}
		catch (Exception Err) { throw Err; }
		finally { Connection.Close(); }

	}//מקבל את מספר הסרט ומחזיר קישור
	public DataTable GetMoiveName(int[] MovieID)
    {
        DataTable send = new DataTable("Movies");
        DataColumn col = new DataColumn(("MovieID"), typeof(int));
        col.Unique = true;
        send.Columns.Add(col);

         DataColumn col1 = new DataColumn(("MovieName"), typeof(string));
        send.Columns.Add(col1);

       
        Connection.Open();
        try
        {
            for (int i = 0; i < MovieID.Length; i++)
            {
                Command = new OleDbCommand("GetMoiveName", Connection);
                Command.CommandType = CommandType.StoredProcedure;
                OleDbParameter parm;
                parm = Command.Parameters.Add("@MoiveId", OleDbType.Integer);
                parm.Value = MovieID[i];
               string name=Command.ExecuteScalar().ToString();
            //   DataRow row= send.NewRow();
                send.Rows.Add(MovieID[i],name);
            }
            send.PrimaryKey = new DataColumn[] { send.Columns[0] };
            return send;
        }
        catch (Exception ERR) { throw ERR; }
        finally{Connection.Close();}
    }// מחזיר שם הסרט 
    public int GetMoiveLenght()
    {
        try
        {
            Connection.Open();
            Command = new OleDbCommand("GetMoiveLenght", Connection);
            Command.CommandType = CommandType.StoredProcedure;
            return int.Parse(Command.ExecuteScalar().ToString());
        }
        catch (Exception ERR) { throw ERR; }
        finally { Connection.Close(); }

	}//פעולה מחזירה כמה סרטים יש באתר
	public DataSet GetCategory()//מחזיר את כל הקטגריות הסרטים של האתר
	{
		ds = new DataSet();
		try
		{
			Connection.Open();
			Command = new OleDbCommand("ExpleCategory", Connection);
			Command.CommandType = CommandType.StoredProcedure;
			Adapter = new OleDbDataAdapter(Command);
			Adapter.Fill(ds, "CategoryTbl");
			return ds;

		}
		catch (Exception Err) { throw Err; }
		finally { Connection.Close(); }
	}
}