using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Domain.Model;
using Application.Services;
using Presentation.Validetors.AuthentcationValdetors;
using Application.DTOs.UserDTOs;
using Application.DTOs;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
       
        private readonly UserService _userService;


        public AuthenticationController(UserService userService   )
        {
            
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
      
        public async Task<ActionResult<string>> SignIn([FromBody] AuthenticationRequestDTO authenticationRequestBody)
        {
            // Validate the request body 
            var validator = new AuthenticationRequestValidator();
            var validationResult = validator.Validate(authenticationRequestBody);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(new { Errors = errors });
            }

            // Validate user credentials
            var token = await _userService.AuthenticateUserAsync(authenticationRequestBody);
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
