﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using e_commerce_store.data;
using e_commerce_store.Models;
using e_commerce_store.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace e_commerce_store.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,ApplicationDbContext context){
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel){
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.Identifier) ?? 
            await _userManager.FindByNameAsync(loginViewModel.Identifier);

            if (user != null)
            {
                //User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password.Trim());
                
                if (passwordCheck)
                {
                    //Password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password.Trim(), true, true);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                //Password is incorrect
                ModelState.AddModelError("Password", "Wrong password. Please try again");
                return View(loginViewModel);
            }
            //User not found
             ModelState.AddModelError("Identifier", "Wrong email/userName. Please try again");
            return View(loginViewModel);

        }


        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                ModelState.AddModelError("EmailAddress", "This email address is already in use");
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                Name = registerViewModel.Name.Trim(),
                Email = registerViewModel.EmailAddress.Trim(),
                UserName = registerViewModel.UserName.Trim(),
                PhoneNumber = registerViewModel.PhoneNumber == null ? null : registerViewModel.PhoneNumber.Trim()
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded){
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                var result = await _signInManager.PasswordSignInAsync(newUser, registerViewModel.Password.Trim(), true, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else{
                foreach (var error in newUserResponse.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(registerViewModel);
            }
            
            return RedirectToAction("Register", "Account");
        }

        [HttpPost]
        [Route("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}