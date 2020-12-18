using DogGo.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT wk.Id as WalkId, wk.[Date], wk.Duration, wk.WalkerId, wk.DogId, wlkr.Name AS WalkerName, o.Name AS OwnerName
                    FROM Walks wk
                    JOIN Walker wlkr ON wk.WalkerId = wlkr.id
                    JOIN Dog d ON wk.DogId = d.Id
                    JOIN Owner o ON d.OwnerId = o.Id
                   
                    WHERE wk.WalkerId = @id
                    Order BY wk.[Date] DESC
                ";

                    cmd.Parameters.AddWithValue("@id", walkerId);


                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();

                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            Walker = new Walker()
                            {
                                Name = reader.GetString(reader.GetOrdinal("WalkerName"))
                            },
                            Dog = new Dog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Owner = new Owner()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                }
                            }
                        };

                        walks.Add(walk);
                    }

                    reader.Close();

                    return walks;
                }
            }
        }
    }
}
