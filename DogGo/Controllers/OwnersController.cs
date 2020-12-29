using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    [Authorize]
    public class OwnersController : Controller
    {
        private IOwnerRepository _ownerRepo;
        private IDogRepository _dogRepo;
        private IWalkerRepository _walkerRepo;
        private INeighborhoodRepository _neighborhoodRepo;


        // ASP.NET will give us an instance of whichever Repository. This is called "Dependency Injection"
        public OwnersController(IOwnerRepository ownerRepository, IDogRepository dogRepository, IWalkerRepository walkerRepository, INeighborhoodRepository neighborhoodRepository)
        {
            _ownerRepo = ownerRepository;
            _dogRepo = dogRepository;
            _walkerRepo = walkerRepository;
            _neighborhoodRepo = neighborhoodRepository;
        }

        // GET: OwnersController
        public ActionResult Index()
        {
            List<Owner> owners = _ownerRepo.GetAllOwners();

            return View(owners);
        }

        // GET: OwnersController/Details/5
        public ActionResult Details(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(owner.Id);
            List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);

            ProfileViewModel vm = new ProfileViewModel()
            {
                Owner = owner,
                Dogs = dogs,
                Walkers = walkers
            };
            if (owner == null)
                {
                    return NotFound();
                }
            return View(vm);
        }

        // GET: OwnersController/Create
        public ActionResult Create()
        {
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();

            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = new Owner(),
                Neighborhoods = neighborhoods
            };

            return View(vm);
        }

        // POST: OwnersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OwnerFormViewModel vm)
        {
            try
            {
                _ownerRepo.AddOwner(vm.Owner);
                return RedirectToAction("Details", new { id = vm.Owner.Id });
            }
            catch (Exception ex)
            {
                //vm.ErrorMessage = "Woops! Something went wrong while saving this owner";
                vm.Neighborhoods = _neighborhoodRepo.GetAll();

                return View(vm);
            }
        }

        // GET: OwnersController/Edit/5
        public ActionResult Edit(int id)
        {
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();
            Owner owner = _ownerRepo.GetOwnerById(id);


            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = owner,
                Neighborhoods = neighborhoods
            };

            if (owner == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // POST: OwnersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, OwnerFormViewModel vm)
        {
            try
            {
                _ownerRepo.UpdateOwner(vm.Owner);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //vm.ErrorMessage = "Woops! Something went wrong while saving this owner";
                vm.Neighborhoods = _neighborhoodRepo.GetAll();

                return View(vm);
            }
        }

        // GET: OwnersController/Delete/5
        public ActionResult Delete(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);

            return View(owner);
        }

        // POST: OwnersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Owner owner)
        {
            try
            {
                _ownerRepo.DeleteOwner(id);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View(owner);
            }
        }

        public ActionResult Login()
        {
            //if user is already logged in, do not let them view this view
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            Owner owner = _ownerRepo.GetOwnerByEmail(viewModel.Email);

            if (owner == null)
            {
                //Has ugly user exp
                return Unauthorized();
            }

            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()),
        new Claim(ClaimTypes.Email, owner.Email),
        new Claim(ClaimTypes.Role, "DogOwner"),
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Dogs");
        }
    }
}
