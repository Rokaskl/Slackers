using System;
using System.Data.SqlClient;

public class DBConnection
{
	public DBConnection(string args)
	{
	}

    private bool Connect()
    {
        //get connectionstring
        SqlConnection connection = new SqlConnection(/*connection string*/);

    }
}
