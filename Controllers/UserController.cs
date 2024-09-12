using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    [EnableCors("AllowLocalhost")]
    [ApiController]
    [Route("api/[controller]")]
    // [assembly: InternalsVisibleToAttribute("Friend2")]
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
            IEmailService emailService
            )
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
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ShowUsers()
        {
            var users = _mapper.Map<List<ClientVM>>(await _userManager.GetUsersInRoleAsync("client"));
            if (!users.Any())
            {
                return NoContent();
            }
            return Ok(users);
        }

        [HttpGet("ShowCrew")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ShowCrew()
        {
            // var dashAtrtribut = _mapper.Map<List<DashboardUsersVM>>(await _unitOfWork.DashboardUser.GetAll(x => x.User));

            var dashAtrtribut = await _unitOfWork.DashboardUser.GetAll(x => x.User);
            var crewInfo = new List<DashboardUsersVM>();
            foreach (var item in dashAtrtribut)
            {
                var role = (await _userManager.GetRolesAsync(item.User))[0];
                crewInfo.Add(new()
                {
                    Id = item.Id,
                    UserName = item.User.UserName,
                    Email = item.User.Email,
                    PhoneNumber = item.User.PhoneNumber,
                    ZipCode = item.ZipCode,
                    City = item.City,
                    Adress = item.Adress,
                    Country = item.User.Country,
                    Role = role,
                });
            }

            if (!crewInfo.Any())
            {
                return NoContent();
            }

            return Ok(crewInfo);
        }

        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(statusCode: StatusCodes.Status409Conflict)]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM RegisterVM)
        {
            return await CreateUser(RegisterVM, "client");
        }

        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(statusCode: StatusCodes.Status409Conflict)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "owner")]
        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterVM RegisterVM)
        {
            return await CreateUser(RegisterVM, "admin", false);
        }

        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(statusCode: StatusCodes.Status409Conflict)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "owner")]
        [HttpPost("CreateOwner")]
        public async Task<IActionResult> CreateOwner([FromBody] RegisterVM RegisterVM)
        {
            return await CreateUser(RegisterVM, "owner", false);
        }

        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(statusCode: StatusCodes.Status409Conflict)]
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
                return NotFound(new { state = "Error", message = "user not exists, please check the email" });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginVM.Password);
            if (!isCorrect)
            {
                return NotFound(new { state = "Error", message = "Wrong password, please enter a valid one" });
            }

            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string dashBoardDomain = _configuration.GetSection("Application:DashBoardDomain").Value;

            var role = await _userManager.GetRolesAsync(existingUser);
            var hasOrigin = this.Request.Headers.TryGetValue("Origin", out var origin);
            var domain = appDomain;

            if (dashBoardDomain.Contains(origin))
            {
                if (role[0] == "client")
                {
                    return BadRequest(new { message = "your are not allowed to access this website" });
                }
                var jwtToken = await GenerateJwtToken(role[0], existingUser, false);
                return Ok(jwtToken);
            }
            else
            {
                var jwtToken = await GenerateJwtToken(role[0], existingUser, true);
                return Ok(jwtToken);
            }
        }

        [HttpPost("Logout")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            var rtDb = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);

            _db.RefreshTokens.Remove(rtDb);

            Response.Cookies.Delete("jwtToken", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Secure = true
            });
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Secure = true
            });

            _unitOfWork.Save();

            return Ok();
        }

        [HttpPatch("UpdateAccount")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccount userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please fill all the necessary information" });
            }
            var user = await _unitOfWork.User.Get(x => x.Email == userData.Email);
            if (user == null)
            {
                return NotFound(new { status = "error", message = "User Dose not exists" });
            }

            user.Country = userData.Address;
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
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Please Enter the email" });
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { status = "error", message = "User Dose not exists" });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
            return Ok(new { status = "success", message = "We sent an email to verify your identity, please verify it in 15 min." });
        }

        [HttpPost]
        [Route("refreshToken")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Invalid Payload" });
            }
            TokenRequest tokenRequest = new TokenRequest();
            var jwtToken = HttpContext.Request.Cookies["jwtToken"];
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            if (jwtToken == null || refreshToken == null)
            {
                return BadRequest(new { status = "error", message = "you need to include access and refresh" });
            }

            tokenRequest.Token = jwtToken;
            tokenRequest.RefreshToken = refreshToken;

            var result = await VerifyAndGenerateToken(tokenRequest);

            if (result == null)
            {
                return Unauthorized(new { status = "error", message = "Invalid Token" });
            }

            if (result.Success == false)
            {
                return BadRequest(result);
            }

            return Ok(result);

        }

        [HttpPost]
        [Route("ResetPassword")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto rpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Fill all the required inputs" });
            }

            var user = await _userManager.FindByIdAsync(rpDto.UserId);
            if (user == null)
            {
                return NotFound(new { status = "error", message = "User Not Found" });
            }
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, rpDto.Token, rpDto.NewPassword);

            if (resetPasswordResult.Succeeded)
            {
                _unitOfWork.Save();
                return Ok(new { status = "success", message = "Password Is Updated" });
            }
            else
            {
                return BadRequest(new { status = "error", message = "use the Token one time only" });
            }
        }

        private async Task<AuthResult> GenerateJwtToken(string role, User user, bool sendAccessToken = true)
        {
            if (user.Email == "FAKE")
            {
                return null;
            }
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = await GetAllValidClaims(user);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
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
            {
                _db.Entry(rt).State = EntityState.Detached;
                refreshToken.Id = rt.Id;
                _db.RefreshTokens.Update(refreshToken);
            }
            await _db.SaveChangesAsync();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiryDate,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Secure = true
            };

            var cookieOptionsForDashboard = new CookieOptions
            {
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Secure = true
            };


            if (sendAccessToken)
            {
                Response.Cookies.Append("jwtToken", jwtToken, cookieOptions);
            }
            else
            {
                Response.Cookies.Append("jwtToken", jwtToken, cookieOptionsForDashboard);
            }
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                // RefreshToken = refreshToken.Token,
                Email = user.Email,
                Role = role,
                UserName = user.UserName
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
                var storedToken = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                var role = await _userManager.GetRolesAsync(dbUser);

                if (expiryDate > DateTime.UtcNow)
                {
                    var jsonToken = jwtTokenHandler.ReadToken(tokenRequest.Token) as JwtSecurityToken;
                    var email = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                    return new AuthResult()
                    {
                        Success = true,
                        Errors = new List<string>(){
                            "Token has not yet expired"
                        },
                        Token = tokenRequest.Token,
                        Email = email,
                        UserName = dbUser.UserName,
                        Role = role[0]

                    };
                }

                // validation 4 - validate existence of the token
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
                _db.SaveChanges();

                // generate new token
                if (role[0] != "client")
                {
                    return await GenerateJwtToken(role[0], dbUser, false);
                }
                return await GenerateJwtToken(role[0], dbUser);

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
            string dashBoardDomain = _configuration.GetSection("Application:DashBoardDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:ForgotPassword").Value;
            var hasOrigin = this.Request.Headers.TryGetValue("Origin", out var origin);
            var domain = appDomain;
            if (hasOrigin)
            {
                if (dashBoardDomain.Contains(origin))
                {
                    domain = dashBoardDomain;
                }
            }
            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.UserName),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(domain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendEmailForForgotPassword(options);
        }


        private async Task<IActionResult> CreateUser(RegisterVM RegisterVM, string role, bool sendAllTokens = true)
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
                return Conflict(new { status = "Error", message = "User Already Exists" });
            }

            var newuser = new User() { Email = RegisterVM.Email, UserName = RegisterVM.UserName, Country = RegisterVM.Country };
            IdentityResult isCreated;
            if (role == "client")
            {
                isCreated = await _userManager.CreateAsync(newuser, RegisterVM.Password);
            }
            else
            {
                var DashbordUserAttributes = new DashbordUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = newuser.Id,
                    ZipCode = RegisterVM.ZipCode,
                    City = RegisterVM.City,
                    Adress = RegisterVM.Adress
                };
                var dashAtrtribut = await _unitOfWork.DashboardUser.Add(DashbordUserAttributes);
                isCreated = await _userManager.CreateAsync(newuser, RegisterVM.Password);
            }

            if (isCreated.Succeeded)
            {
                // we need to add a user to a role
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                var resultRoleAdition = await _userManager.AddToRoleAsync(newuser, role);

                if (sendAllTokens)
                {
                    var jwtToken = await GenerateJwtToken(role, newuser);

                    return StatusCode(201, jwtToken);
                }
                return Ok(new { status = "success", message = "new " + role + " Is Created" });
            }
            else
            {
                return StatusCode(500, new { status = "Error", message = "Not able to crate your account, Please try again later", errors = isCreated.Errors });
            }
        }

    }
}