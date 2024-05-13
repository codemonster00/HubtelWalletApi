using AutoMapper;
using FluentAssertions;
using HubTelWalletApi.Controllers;
using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using UnitTestHubtelWalletApi.MockData;

namespace UnitTestHubtelWalletApi
{
    public class WalletControllerTest
    {
        private readonly Mock<ILogger<WalletController>> logger;
        private readonly Mock<IMapper> mapper;

        public WalletControllerTest()
        {
            logger = new Mock<ILogger<WalletController>>();
            mapper = new Mock<IMapper>();
        }


        [Fact]

        public async Task GetAllWallets_AuthorizedRequest_ReturnsOkWithWallets()
        {
            // Arrange
            var accessToken = "valid_access_token"; 
            var phone = "1234567"; 
            var walletlist = WalletMockData.GetWallets(); 
            var walletrepo = new Mock<IWalletRepository>();
            walletrepo.Setup(x => x.GetMobilePhoneClaimFromJwt(accessToken)).Returns(phone);
            walletrepo.Setup(x => x.GetAllAsync(phone)).ReturnsAsync(walletlist);
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            

            // Act
            var result = await sut.GetAllWallets();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(new GetAllWalletsResponse
            {
                Message = "Resource successfully fetched",
                StatusCode = HttpStatusCode.OK,
                Wallets = null
            });
        }

        [Fact]
        public async Task GetAllWallets_UnauthorizedRequest_ReturnsUnauthorized()
        {
            // Arrange
            var walletrepo = new Mock<IWalletRepository>();
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            var accessToken = ""; 
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";

            // Act
            var result = await sut.GetAllWallets();

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
            unauthorizedResult.Value.Should().BeEquivalentTo(new GetAllWalletsResponse
            {
                Message = "Invalid Request",
                StatusCode = HttpStatusCode.Unauthorized,
                Wallets = null
            });
        }

        [Fact]
        public async Task GetAllWallets_ExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            var walletrepo = new Mock<IWalletRepository>();
            var accessToken = "rtyrfghh";
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            walletrepo.Setup(x => x.GetMobilePhoneClaimFromJwt(accessToken)).Throws(new Exception("Simulated exception"));

            // Act
            var result = await sut.GetAllWallets();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            internalServerErrorResult.Value.Should().BeEquivalentTo(new GetAllWalletsResponse
            {
                Message = "Sorry something went wrong",
                StatusCode = HttpStatusCode.InternalServerError,
                Wallets = null
            });
        }


        [Fact]
        public async Task Get_ExistingWallet_ReturnsOkWithWallet()
        {
            // Arrange
            var accessToken = "valid_access_token"; 
            var phone = "1234567"; 
            var existingWallet = new Wallet {  };
            var walletrepo = new Mock<IWalletRepository>();
            walletrepo.Setup(x => x.GetMobilePhoneClaimFromJwt(accessToken)).Returns(phone);
            walletrepo.Setup(x => x.GetAsync(123, phone)).ReturnsAsync(existingWallet);
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";

            // Act
            var result = await sut.Get(123); // Replace with an existing item ID

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(new GetWalletResponse
            {
                Message = "Resource successfully fetched",
                StatusCode = HttpStatusCode.OK,
                Wallet = null
            });
        }

        [Fact]
        public async Task Get_ItemNotFound_ReturnsNotFound()
        {
            // Arrange
            var walletrepo = new Mock<IWalletRepository>();
            walletrepo.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((Wallet)null);
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            var accessToken = "valid_access_token";
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            // Act
            var result = await sut.Get(123); // Replace with a non-existing item ID

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var accessToken = "valid_access_token"; 
            var phone = "1234567"; 
            var walletrepo = new Mock<IWalletRepository>();
            walletrepo.Setup(x => x.GetMobilePhoneClaimFromJwt(accessToken)).Returns(phone);
            walletrepo.Setup(x => x.DeleteAsync(123, phone)).Verifiable(); 
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";

            // Act
            var result = await sut.Delete(123);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            walletrepo.Verify(x => x.DeleteAsync(123, phone), Times.Once); 
        }

        [Fact]
        public async Task Delete_UnauthorizedRequest_ReturnsUnauthorized()
        {

            // Arrange
            var walletrepo = new Mock<IWalletRepository>();
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            var phone = "1234567";
            var accessToken = "";
            walletrepo.Setup(x=>x.DeleteAsync(123,phone)).Verifiable();
            walletrepo.Setup(x => x.GetMobilePhoneClaimFromJwt(accessToken)).Returns(phone);
            // Act
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            var result = await sut.Delete(1); // Replace with an existing item ID
           
            // Assert

            result.Should().BeOfType<UnauthorizedResult>();
            walletrepo.Verify(x => x.DeleteAsync(123, phone), Times.Never);
        }

        [Fact]
        public async Task Delete_ItemNotFound_ReturnsNotFound()
        {
            // Arrange
            var walletrepo = new Mock<IWalletRepository>();
            walletrepo.Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<string>())).Throws(new NotFoundException("Resource not found with specified id")); // Set up the deletion
          //  var phone = "1234567";
            var accessToken = "valid_access_token";
            var sut = new WalletController(walletrepo.Object, logger.Object, mapper.Object);
            sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            sut.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {accessToken}";
            // Act
            var result = await sut.Delete(123); // Replace with a non-existing item ID

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notfoundResult = result as NotFoundObjectResult;
            notfoundResult.Value.Should().BeEquivalentTo(new DeleteWalletResponse
            {
                Message = "Resource not found with specified id",
                StatusCode = HttpStatusCode.NotFound
                
            });
        }

    }
}