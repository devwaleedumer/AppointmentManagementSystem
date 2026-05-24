using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Helpers;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.Services.Email;
using AppointmentManagementSystem.ViewModels.Accounts;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AppointmentManagementSystem.Controllers
{
    public class AccountsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly AppointmentManagementSystemDbContext _db;
        private readonly IAppointmentService _appointmentService;

        public AccountsController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppointmentManagementSystemDbContext db,
            IEmailService emailService,
            IAppointmentService appointmentService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _db = db;
            _appointmentService = appointmentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Accounts/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


        public IActionResult CreateDoctor()
        {
            return View();
        }


        public async Task<IActionResult> Doctors()
        {
            var doctor = await _appointmentService.GetAllDoctors();
            return View(doctor);
        }


  

        [Authorize( Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDoctor(DoctorAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Qualification = model.Qualification,
                    Specialization = model.Specialization,
                    EmailConfirmed = false
                };

                var password = PasswordGenerator.GeneratePassword();
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();

                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, ConstHelper.DoctorRole);

                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }


                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await transaction.CommitAsync();

                var verificationLink =
                                        Url.Action(
                                            "VerifyEmail",
                                            "Accounts",
                                            new { token, email = user.Email },
                                            Request.Scheme
                                        );

                         var body = $"""
                                        <h2>Email Verification</h2>
                                        <p>Please click the link below to verify your email:</p>
                                        <p> Default Password: {password} </p?
                                        <a href="{verificationLink}">
                                            Verify Email
                                        </a>
                                        {verificationLink}
                                        Please ignore this email if you did not create an account with us.
                                        """;

                BackgroundJob.Enqueue( () =>  _emailService.SendEmailAsync(
                    user.Email!,
                    user.Name,
                    "Verify Your Email", body));

                return RedirectToAction("Doctors", "Accounts");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError("", "Something went wrong while creating doctor account.");
                return View(model);
            }
        }

       
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Invalid link, please try again.";
                ViewBag.IsValid = false;
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                ViewBag.Message = "Invalid link, please try again.";
                ViewBag.IsValid = false;
                return View();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                ViewBag.Message = "Invalid or expired token.";
                ViewBag.IsValid = false;
                return View();
            }


            ViewBag.Message = "Email confirmed successfully.";
            ViewBag.IsValid = true;

            return View();
        }



        public IActionResult PatientLogin()
        {
            return View();
        }
        public IActionResult DoctorLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PatientLogin(PatientLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var foundUser = await _userManager.FindByEmailAsync(model.Email);

            if (foundUser is null) 
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            if (!(await _userManager.IsInRoleAsync(foundUser, ConstHelper.PatientRole)))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    BackgroundJob.Enqueue(
                        () => _emailService.SendEmailAsync(
                            model.Email,
                            user.Name,
                            "Account Locked",
                            "Your account has been locked due to multiple failed login attempts. If this wasn't you, please contact support immediately."
                        )
                    );
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> DoctorLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var foundUser = await _userManager.FindByEmailAsync(model.Email);


            if (foundUser is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            if (foundUser.LockoutEnabled)
            {
                ModelState.AddModelError(string.Empty, "Suspicious activity detected. Your account has been locked.");
                BackgroundJob.Enqueue(
                    () => _emailService.SendEmailAsync(
                        foundUser.Email!,
                        foundUser.Name,
                        "Account Locked",
                        "Your account has been locked due to suspicious activity. If this wasn't you, please contact support immediately."
                    )
                );
                return View(model);
            }

            if (!(await _userManager.IsInRoleAsync(foundUser, ConstHelper.DoctorRole)))
            {
                await _userManager.SetLockoutEnabledAsync(foundUser, true);
                ModelState.AddModelError(string.Empty, "Suspicious activity detected. Your account has been locked.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email , model.Password, model.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    BackgroundJob.Enqueue(
                        () => _emailService.SendEmailAsync(
                            model.Email,
                            user.Name,
                            "Suspicious Login Attempt",
                            "Your account has been locked due to multiple failed login attempts. If this wasn't you, please contact support immediately."
                        )
                    );
                    ViewBag.ErrorMessage = "Account Locked out due to 2 consective failed attemps";
                }
                else if (result.IsNotAllowed)
                {
                    ViewBag.ErrorMessage = "You are not allowed to login. Ask admin for details";
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid login attempt.";
                }
                return View();
            }

            return RedirectToAction(nameof(Index), controllerName: "Home");
        }



        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPatient(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, ConstHelper.PatientRole);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }
            BackgroundJob.Enqueue(
                () => _emailService.SendEmailAsync(
                    user.Email!,
                    user.Name,
                    "Welcome to Appointment Management System",
                    "Your patient account has been successfully created. You can now log in with your credentials."
                )
            );

            return RedirectToAction(nameof(Index), controllerName: "Home");
        }


        [HttpPost]
        public async Task<IActionResult> RegisterAdminAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, ConstHelper.AdminRole);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }
            BackgroundJob.Enqueue(
                () => _emailService.SendEmailAsync(
                    user.Email!,
                    user.Name,
                    "Admin Account Created",
                    "Your admin account has been successfully created. You can now log in with your credentials."
                )
            );
            return RedirectToAction(nameof(Index), controllerName: "Home");
        }
    }
}
