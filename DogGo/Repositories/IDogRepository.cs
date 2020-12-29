using System;
using System.Collections.Generic;
using DogGo.Models;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories
{
    public interface IDogRepository
    {
        List<Dog> GetAllDogs();
        Dog GetDogById(int id);
        void AddDog(Dog dog);
        List<Dog> GetDogsByOwnerId(int ownerId);
        void UpdateDog(Dog dog);
        void DeleteDog(int dogId);


    }
}
