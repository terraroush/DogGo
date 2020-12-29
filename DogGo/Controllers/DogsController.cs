using DogGo.Repositories;
using DogGo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DogGo.Controllers
{
    [Authorize]
    public class DogsController : Controller
    {
        private readonly IDogRepository _dogRepo;

        public DogsController(IDogRepository dogRepository)
        {
            _dogRepo = dogRepository;
        }
        // GET: DogsController
        
        public ActionResult Index()
        {
            int currentUserId = GetCurrentUserId();
            List <Dog> dogs = _dogRepo.GetDogsByOwnerId(currentUserId);

            return View(dogs);
        }

        // GET: DogsController/Details/5
        public ActionResult Details(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);

            int currentUserId = GetCurrentUserId();

            if (dog.OwnerId != currentUserId)
            {
                return NotFound();
            }

            if (dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        // GET: DogsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog dog)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                dog.OwnerId = currentUserId;

                _dogRepo.AddDog(dog);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }

        // GET: DogsController/Edit/5
        public ActionResult Edit(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);
            int currentUserId = GetCurrentUserId();


            if (dog == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST: DogsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                dog.OwnerId = currentUserId;
                _dogRepo.UpdateDog(dog);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View(dog);
            }
        }

        // GET: DogsController/Delete/5
        public ActionResult Delete(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);
            int currentUserId = GetCurrentUserId();

            if (dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        // POST: DogsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Dog dog)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                dog.OwnerId = currentUserId;
                _dogRepo.DeleteDog(id);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View(dog);
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
