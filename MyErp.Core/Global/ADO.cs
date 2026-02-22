using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Text;
using Logger;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
namespace MyErp.Core.Global
{
    public class ADO
    {
      

            //public static string GenerateUpdateQuery(string tableName, Dictionary<string, object> updateValues, Dictionary<string, object> whereConditions)
            //{
            //    List<string> setClauses = new List<string>();
            //    List<string> whereClauses = new List<string>();
            //    /*List<SqlParameter> parameters = new List<SqlParameter>();
            //    List<SQLiteParameter> Liteparameters = new List<SQLiteParameter>();*/
            //    // Create the SET clauses and parameters
            //    foreach (var pair in updateValues)
            //    {
            //        string parameterName = $"@{pair.Key}";
            //        if (parameterName == "@SavingStatusDate")
            //        {
            //            setClauses.Add($"{pair.Key} = {pair.Value}");
            //        }
            //        else
            //        {
            //            setClauses.Add($"{pair.Key} = '{pair.Value}'");
            //        }
            //    }

            //    // Create the WHERE clauses and parameters
            //    foreach (var pair in whereConditions)
            //    {
            //        string parameterName = $"@{pair.Key}";
            //        whereClauses.Add($"{pair.Key} = '{pair.Value}' ");
            //    }

            //    string setClause = string.Join(", ", setClauses);
            //    string whereClause = string.Join(" AND ", whereClauses);

            //    string query = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

            //    return query;
            //}

            //public static int executeQuery(string qrys)
            //{
            //    try
            //    {
            //        using (MySqlConnection connection = new MySqlConnection(MyErp.Global.InternalClasses.CustomerConfiguration.ConnectionString/*StaticData.Configuration.ConnectionString*/))
            //        {
            //            connection.Open();
            //            string handleQry = string.Join("\n", qrys);
            //            Logs.Log("Start updating status of Bulk");
            //            using (MySqlCommand command = new MySqlCommand(handleQry, connection))
            //            {
            //                if (command.CommandText != "")
            //                {
            //                    var rowAffected = command.ExecuteNonQuery(); // Returns the number of affected rows
            //                    connection.Close();
            //                    Logs.Log($"Finsh updating status of Bulk => {rowAffected}");
            //                    return rowAffected;
            //                }
            //                else
            //                {
            //                    var queries = new StringBuilder();
            //                    foreach (var item in qrys)
            //                    {
            //                        queries.Append(item + "\n");
            //                    }
            //                    Logs.Log("command.CommandText=>" + $"\n {queries}");
            //                    return 0;
            //                }

            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        var queries = new StringBuilder();
            //        foreach (var item in qrys)
            //        {
            //            queries.Append(item + "\n");
            //        }
            //        Logs.Log(ex.Message + "\n" + queries.ToString());
            //        return 0;
            //    }
            //}

            public async static Task<List<T>> GetExecuteQueryMySql<T>(string qry )
            {
                using (var connection = new SqlConnection(MyErp.Core.Global.CustomerConfiguration.ConnectionString))
                {
                    try
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        var result = await connection.QueryAsync<T>(qry).ConfigureAwait(false);
                        return result.ToList();
                    }
                    catch (Exception ex)
                    {
                        Logs.Log(ex.Message, ex); // Custom logging                    
                        return new List<T>();
                    }
                }
            }

        }

    }

