using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Domain.Model;
using Presentation.Validetors;
using Presentation.model;
using Application.DTOs;
using Infrastructure.Repository;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Domain.Entities;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenGenerator _tokenGenerator;
        ApplicationDbContext _context;  
            ILogger<UserRepository> _logger;
         private readonly IMapper _mapper;

        public AuthenticationController(
            IConfiguration configuration,
            ITokenGenerator tokenGenerator,
            ApplicationDbContext context,  
            ILogger<UserRepository> logger,
        IMapper mapper
        )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
             _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                SecretKey = _configuration["JWT:SecretKey"],
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
            };

            // Generate the JWT token
            var token = await _tokenGenerator.GenerateToken(
                user.Email,
                authenticationRequestBody.Password,
                jwtConfig);

            return Ok(new { Token = token });
        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                User userEntity = _mapper.Map<User>(user);

                UserRepository userRepository = new UserRepository(_context, _logger);
                await userRepository.InsertAsync(userEntity);


                return Ok("Register User Successfully.");
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
