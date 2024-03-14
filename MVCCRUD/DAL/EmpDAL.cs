using MVCCRUD.Models;
using MVCCRUD.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVCCRUD.DAL
{
	public class EmpDAL : BaseService
	{
        #region CRUD
        /// <summary>
        /// Creates a new record for Employee
        /// </summary>
        /// <param name="empModel">EmpModel object as params</param>
        /// <returns>Returns id of the employee</returns>
        public int Create(EmpModel empModel)
        {
            int insertedId = 0;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string sqlQuery = "INSERT INTO EmpModel (name, age) VALUES (@Name, @Age);" +
                                  "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@Name", empModel.Name);
                    command.Parameters.AddWithValue("@Age", empModel.Age);

                    sqlConnection.Open();

                    // Execute the INSERT statement and retrieve the inserted identity value
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        insertedId = Convert.ToInt32(result);
                    }
                }
            }

            return insertedId;
        }



        /// <summary>
        /// Gets all employee details as list of employees
        /// </summary>
        /// <returns>Returns List of EmpModel</returns>
        public List<EmpModel> Read()
		{
			List<EmpModel> empList = new List<EmpModel>();
			SqlConnection sqlConnection = new SqlConnection(ConnectionString);

			SqlCommand command = new SqlCommand();
			command.Connection = sqlConnection;
			string sqlQuery = String.Format("select id,name,age from EmpModel;");
			command.CommandText = sqlQuery;
			sqlConnection.Open();

			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
			DataTable dt = new DataTable();
			sqlDataAdapter.Fill(dt);

			foreach (DataRow dr in dt.Rows)
			{
				empList.Add(new EmpModel
				{
					Id = GetDBInt(dr["id"]),
					Age = GetDBInt(dr["age"]),
					Name =GetDBString(dr["name"].ToString())
				});
			}

			return empList;
		}

		/// <summary>
		/// Updates the Employee for the given id
		/// </summary>
		/// <param name="empModel">Employee model object</param>
		/// <returns>Returns int if successfully updated</returns>
		public int Update(EmpModel empModel)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "UPDATE EmpModel SET name = @Name, age = @Age WHERE id = @Id;";
                    command.Parameters.AddWithValue("@Name", empModel.Name);
                    command.Parameters.AddWithValue("@Age", empModel.Age);
                    command.Parameters.AddWithValue("@Id", empModel.Id);
                    command.Connection = sqlConnection;

                    sqlConnection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }

        /// <summary>
        /// Deletes the employee for the given id
        /// </summary>
        /// <param name="empModel">EmpModel object</param>
        /// <returns></returns>
        public int Delete(EmpModel empModel)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "DELETE FROM EmpModel WHERE id = @Id;";
                    command.Parameters.AddWithValue("@Id", empModel.Id);
                    command.Connection = sqlConnection;

                    sqlConnection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
        }
        #endregion

        #region Get Employee details
        /// <summary>
        /// Gets employee details for the given id
        /// </summary>
        /// <param name="empModel">Employee Model</param>
        /// <returns>Returns Employee details</returns>
        public EmpModel GetEmployee(EmpModel empModel)
		{
			SqlConnection sqlConnection = new SqlConnection(ConnectionString);
			SqlCommand command = new SqlCommand();
			command.Connection = sqlConnection;
			command.CommandText = "select id,name,age from EmpModel where id=@id;";
			command.Parameters.AddWithValue("@id", empModel.Id);
			sqlConnection.Open();

			SqlDataReader reader = command.ExecuteReader();
			while (reader.Read())
			{
				empModel.Name = GetDBString(reader["name"]);
				empModel.Age = GetDBInt(reader["age"]);
			}

			return empModel;
		}
		#endregion

	}
}

