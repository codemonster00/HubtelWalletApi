using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
namespace HubTelWalletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly IOtpSender _otpSender;

        public AuthController(IUserRepository userRepository, ILogger<AuthController> logger, IOtpSender otpSender)
        {
            _userRepository = userRepository;
            _logger = logger;
            _otpSender = otpSender;
        }


        [HttpPost("signup")]
       
        public   IActionResult SignUp([FromBody] AppUser user)
        {

            try
            {
                
                if (!ModelState.IsValid || string.IsNullOrEmpty(user.Phone))
                {
                    return BadRequest(new SignUpResponse { Message = "Invalid Request", StatusCode = HttpStatusCode.BadRequest ,Otp=null,RequestId=null});
                }
                if (!_userRepository.UserExist(user.Phone))
                {
                    _userRepository.SaveUser(user);
                }

                var data = _otpSender.SendOtp(user.Phone);
                return Ok(new SignUpResponse
                {
                    Message = "Success,Otp Successfully Sent",
                     RequestId = data.RequestId ,
                    StatusCode = HttpStatusCode.OK,
                    Otp = data.Otp,
                    Phone = data.Phone

                });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while processing SignUp request.");

                // Return a generic error message
                return StatusCode(StatusCodes.Status500InternalServerError, new SignUpResponse { Message = "Sorry something went wrong", StatusCode = HttpStatusCode.InternalServerError });
            }
        }



        [HttpPost("verify")]
        public IActionResult VerfiyOtp([FromBody] OtpData data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Otp))
                {
                    return BadRequest(new VerifyOtpResponse { Message = "Invalid Request", StatusCode = HttpStatusCode.BadRequest });
                }
                if (!_otpSender.Verify(data))
                {
                    return BadRequest(new VerifyOtpResponse { Message = "Invalid otp code", StatusCode = HttpStatusCode.BadRequest });
                }

                var jwtToken = GenerateJwtToken(data.Phone);
                return Ok(new VerifyOtpResponse { StatusCode = HttpStatusCode.OK, Message = "Successfully verified", Token = jwtToken });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while processing SignUp request.");

                return StatusCode((int)HttpStatusCode.InternalServerError, new VerifyOtpResponse { Message = "Sorry something went wrong", StatusCode = HttpStatusCode.InternalServerError });
            }
        }

        private static string  GenerateJwtToken(string userphone)
        {
            // Retrieve JWT settings from configuration
            var jwtSettings = Program.GetJwtSettings();

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new(ClaimTypes.MobilePhone, userphone) // Add any additional claims here
                }),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresInMinutes), // Token expiration time
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
