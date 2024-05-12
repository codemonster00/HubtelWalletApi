using HubTelWalletApi.Context;
using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace HubTelWalletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletRepository walletRepository,ILogger<WalletController> logger)
        {
           _walletRepository = walletRepository;
            _logger = logger;
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(int id )
        {
            try
            {
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (string.IsNullOrEmpty(accessToken))
                {
                   
                    return Unauthorized(new GetWalletResponse { Message="This request is unauthorized",StatusCode= HttpStatusCode.Unauthorized,Wallet=null});

                }
                var phone = _walletRepository.GetMobilePhoneClaimFromJwt(accessToken);
                var wallet = await _walletRepository.GetAsync(id, phone);
                if (wallet == null)
                {
                    return NotFound(new GetAllWalletsResponse { Message = "Specified resource was not found", StatusCode = HttpStatusCode.NotFound});
                }
                return Ok(new GetWalletResponse { Message = "Resource successfully fetched", StatusCode = HttpStatusCode.OK, Wallet = wallet });
                // add unauthorised response here
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Something went wrong getting the specified resource ");

                return StatusCode(StatusCodes.Status404NotFound,new GetWalletResponse{Message= ex.Message,StatusCode=HttpStatusCode.NotFound,Wallet=null});
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong getting the specified resource ");

                return StatusCode(StatusCodes.Status500InternalServerError, new GetWalletResponse { Message = "Sorry something went wrong", StatusCode = HttpStatusCode.InternalServerError, Wallet = null });

            }

        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllWallets()
        {
            try
            {
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var phone = _walletRepository.GetMobilePhoneClaimFromJwt(accessToken);
                    var allwallets = await _walletRepository.GetAllAsync(phone);

                    return Ok( new GetAllWalletsResponse { Message="Resource successfully fetched",StatusCode=HttpStatusCode.OK,Wallets=allwallets});
                }
               
            }
           catch(Exception ex) {

                _logger.LogError(ex, "Something went wrong getting the specified resource ");

                return StatusCode(StatusCodes.Status500InternalServerError, new GetAllWalletsResponse { Message = "Sorry something went wrong", StatusCode = HttpStatusCode.InternalServerError, Wallets = null });

            }          
        
          return Unauthorized(new GetAllWalletsResponse{Message="Invalid Request", StatusCode= HttpStatusCode.Unauthorized,Wallets=null});
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Wallet wallet)
        {
           try
            {
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1] ;
                
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                wallet.Owner =await _walletRepository.GetAppUserFromPhone(_walletRepository.GetMobilePhoneClaimFromJwt(accessToken));
                await _walletRepository.WalletExist(wallet);

                await _walletRepository.ExceedCreateWalletLimit(wallet);

                var saved = await _walletRepository.AddAsync(wallet);

                if (!saved)
                {
                    return BadRequest();
                }
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new AddWalletResponse { Message = ex.Message, StatusCode = HttpStatusCode.BadRequest });
            }

            

          

        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized();
                }
                var phone = _walletRepository.GetMobilePhoneClaimFromJwt(accessToken);
                 await _walletRepository.DeleteAsync(id,phone);
            }
            catch (Exception)
            {

                throw;
            }
          
            return NoContent();
        }
    }
}
