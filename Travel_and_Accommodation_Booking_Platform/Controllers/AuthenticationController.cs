using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Domain.Model;
using Presentation.model;
using Application.Services;
using Presentation.Validetors.AuthentcationValdetors;
using Application.DTOs.UserDTOs;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly UserService _userService;


        public AuthenticationController(IConfiguration configuration, ITokenGenerator tokenGenerator,UserService userService   )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
            _userService=userService??throw new ArgumentNullException(nameof(userService));
            
        }

        /// <summary>
        /// Endpoint for user sign-in. Validates user credentials and
        /// generates a JWT token upon successful authentication.
        /// </summary>
        /// <param name="authenticationRequestBody">The request body containing email and password.</param>
        /// <returns>
        /// If successful, returns the generated JWT token;
        /// otherwise, returns validation errors or unauthorized status.
        /// </returns>
        [HttpPost("sign-in")]
      
        public async Task<ActionResult<string>> SignIn([FromBody] AuthenticationRequestBody authenticationRequestBody)
        {
            // Validate the request body 
            var validator = new AuthenticationRequestBodyValidator();
            var validationResult = validator.Validate(authenticationRequestBody);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }

            // Validate user credentials
            var user = await _tokenGenerator.ValidateUserCredentials(
                authenticationRequestBody.Email,
                authenticationRequestBody.Password);

            if (user is null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });

            }

            // Configure JWT settings
            var jwtConfig = new JWTConfig
            {
                SecretKey = Environment.GetEnvironmentVariable("SecretKey"),
                Issuer = Environment.GetEnvironmentVariable("Issuer"),
                Audience = Environment.GetEnvironmentVariable("Audience"),
            };

            // Generate the JWT token
            var token = await _tokenGenerator.GenerateToken(
                user.Email,
                authenticationRequestBody.Password,
                jwtConfig);

            return Ok(new { Token = token });
        }
        [HttpPost("register")]
        
        public async Task<ActionResult<string>> Register(UserRegisterDTO user)
        {
            try
            {
                var validator = new UserRegisterDTOValidator();
                var validationResult = validator.Validate(user);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    return BadRequest(new { Errors = errors });
                }
              


                await _userService.RegisterUserAsync(user);


                return Ok("Register User Successfully.");
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
