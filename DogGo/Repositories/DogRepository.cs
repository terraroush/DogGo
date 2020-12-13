using System;
using System.Collections.Generic;
using DogGo.Models;
using DogGo.Repositories.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;
        
        public DogRepository(IConfiguration config)
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
        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                    ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();
                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")), 
                            Notes = ReaderUtils.GetNullableString(reader, "Notes"),
                            ImageUrl = ReaderUtils.GetNullableString(reader, "ImageUrl"),

                    };


                        dogs.Add(dog);
                    }
                    reader.Close();
                    return dogs;
                }
            }
        }
    }
}

                        //ReaderUtils utils = new ReaderUtils();

                        //dog.Notes = utils.GetNullableString(reader, "Notes");
                        //dog.ImageUrl = utils.GetNullableString(reader, "ImageUrl");


                        //if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                        //    {
                        //    dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        //    }
                        //if (!reader.IsDBNull(reader.GetOrdinal("ImageUrl")))
                        //{
                        //    dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                        //}