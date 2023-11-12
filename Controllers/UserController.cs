using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web_API.Configuration;
using Web_API.Data;
using Web_API.Models;
using Web_API.Models.DTO.Request;
using Web_API.Models.DTO.Responce;
using Web_API.Repository.IRepository;
using Web_API.Service;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            ApplicationDbContext db,
            IUnitOfWork unitOfWork,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            TokenValidationParameters tokenValidationParams,
            IMapper mapper,
            ILogger<UserController> logger,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _tokenValidationParams = tokenValidationParams;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpGet("ShowUsers")]
        public async Task<IActionResult> ShowUsers()
        {
            var users = await _unitOfWork.User.GetAll();
            return Ok(users);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM RegisterVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponce()
                {
                    Errors = new List<string>(){
                        "Invalid Paylpoad"
                    },
                    Success = false
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(RegisterVM.Email);

            if (existingUser != null)
            {
                return BadRequest(new RegistrationResponce()
                {
                    Errors = new List<string>(){
                        "User Already Exists"
                    },
                    Success = false
                });
            }

            var newuser = new User() { Email = RegisterVM.Email, UserName = RegisterVM.UserName, Address = RegisterVM.Address };
            var isCreated = await _userManager.CreateAsync(newuser, RegisterVM.Password);

            if (isCreated.Succeeded)
            {
                // we need to add a user to a role
                var createRole = await _roleManager.CreateAsync(new IdentityRole("client"));
                var resultRoleAdition = await _userManager.AddToRoleAsync(newuser, "client");

                var jwtToken = await GenerateJwtToken(newuser);

                return Ok(jwtToken);
            }
            else
            {
                return BadRequest(new RegistrationResponce()
                {
                    Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                    Success = false
                });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponce()
                {
                    Errors = new List<string>(){
                        "Invalid Paylpoad"
                    },
                    Success = false
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (existingUser == null)
            {
                return NotFound(new RegistrationResponce()
                {
                    Errors = new List<string>(){
                        "Invalid Login Request"
                    },
                    Success = false
                });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginVM.Password);
            if (!isCorrect)
            {
                return NotFound(new RegistrationResponce()
                {
                    Errors = new List<string>(){
                        "Wrong password"
                    },
                    Success = false
                });
            }

            var jwtToken = await GenerateJwtToken(existingUser);
            return Ok(jwtToken);
        }

        [HttpPatch("UpdateAccount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "client")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccount userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the necessary information" });
            }
            var user = await _unitOfWork.User.Get(x => x.Email == userData.Email);
            if (user == null)
            {
                return BadRequest(new { status = "error", message = "User Dose not exists" });
            }

            user.Address = userData.Address;
            user.PhoneNumber = userData.PhoneNumber;
            user.UserName = userData.UserName;

            if (userData.Password != null)
            {
                // check if the old password is the same
                var isCorrect = await _userManager.CheckPasswordAsync(user, userData.OldPassword);
                if (!isCorrect)
                {
                    return BadRequest(new { status = "error", message = "Password is not Correct" });
                }

                // verify confirmation password
                if (userData.Password != userData.ConfirmPassword)
                {
                    return BadRequest(new { status = "error", message = "Password don't match" });
                }

                // make the changes in the database
                await _userManager.ChangePasswordAsync(user, userData.OldPassword, userData.Password);
            }

            _unitOfWork.User.Update(user);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
            return Ok("You may now reset your password.");
        }

        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponce()
                {
                    Errors = new List<string>() { "Invalid payload" },
                    Success = false
                });
            }

            var result = await VerifyAndGenerateToken(tokenRequest);

            if (result == null)
            {
                return BadRequest(new RegistrationResponce()
                {
                    Errors = new List<string>() { "Invalid Tokens" },
                    Success = false
                });
            }

            return Ok(result);

        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string uid, string token, string newPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Fill all the needed inputs" });
            }

            var user = await _unitOfWork.User.Get(x => x.Id == uid);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);


            _unitOfWork.Save();
            return Ok();
        }

        private async Task<AuthResult> GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = await GetAllValidClaims(user);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (rt == null)
                await _db.RefreshTokens.AddAsync(refreshToken);
            else
                _db.RefreshTokens.Update(refreshToken);
            await _db.SaveChangesAsync();

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // validation 1 - validate jwt token format
                _tokenValidationParams.ValidateLifetime = false;
                var tokenInVerfication = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);
                _tokenValidationParams.ValidateLifetime = true;
                // validation 2 - vaidate enciption algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // validation 3 - validate the expity date
                var utcExpiryDate = long.Parse(tokenInVerfication.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>(){
                            "Token has not yet expired"
                        }
                    };
                }

                // validation 4 - validate existence of the token
                var storedToken = _db.RefreshTokens.FirstOrDefault(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>(){
                            "Token does not exist"
                        }
                    };
                }

                // validation 5 - validate if it is used or not
                if (storedToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>(){
                            "Token has been used"
                        }
                    };
                }

                // validation 6 - validate if it is revoked
                if (storedToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>(){
                            "Token has been revoked"
                        }
                    };
                }

                // validation 7 - validate the id 
                var jti = tokenInVerfication.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>(){
                            "Token does not match"
                        }
                    };
                }

                // update current token
                storedToken.IsUsed = true;
                _db.RefreshTokens.Update(storedToken);
                await _db.SaveChangesAsync();

                // generate new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);

            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }

        }
        private async Task<List<Claim>> GetAllValidClaims(User user)
        {
            var claims = new List<Claim>
            {
                new("id", user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Getting the claims that we hava assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // get the user role and add it to the claim
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    // to add all the claims of the user in the token
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    // to add all the claims of the spesific role to the claims in the token
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;

        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var datetimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            datetimeVal = datetimeVal.AddSeconds(unixTimeStamp);
            return datetimeVal;
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }

        private async Task SendForgotPasswordEmail(User user, string token)
        {
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:ForgotPassword").Value;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.UserName),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendEmailForForgotPassword(options);
        }

    }
}