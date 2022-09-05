﻿using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using com.ambassador.support.lib.Services;
using com.ambassador.support.webapi.Controllers.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using com.ambassador.support.lib.Interfaces;
using com.ambassador.support.lib.ViewModel;
using System.IO;

namespace com.ambassador.support.Test.Controller
{
    public class CustomsReportControllerTest
    {

        //public CustomsReportControllerTest():base()
        //{
        //    _mockService = new MockRepository
        //}

        private CustomsReportController GetCustomsReportController(Mock<IExpenditureRawMaterialService> facadeMock,Mock<IReceiptRawMaterialService> facemock2)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            CustomsReportController controller = new CustomsReportController(facadeMock.Object, facemock2.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public async Task GetState_Expect_ExpendRaw()
        {
            // Arrange
            var mockFacade = new Mock<IExpenditureRawMaterialService>();
            mockFacade.Setup(x => x.GetReport(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<ExpenditureRawMaterialViewModel>(),1));

            var mockFacade2 = new Mock<IReceiptRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade, mockFacade2);
            var result = customsReportController.GetExpenditureRawMaterial("", DateTimeOffset.Now, DateTimeOffset.Now, 1, 1, "");

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result) );
        }

        [Fact]
        public async Task GetState_UnExpect_ExpendRaw()
        {
            // Arrange
            var mockFacade = new Mock<IExpenditureRawMaterialService>();
            //mockFacade.Setup(x => x.GetReport(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
            //    .Returns(Tuple.Create(new List<ExpenditureRawMaterialViewModel>(), 1));

            var mockFacade2 = new Mock<IReceiptRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade, mockFacade2);
            var result = customsReportController.GetExpenditureRawMaterial("", DateTimeOffset.Now, DateTimeOffset.Now, 1, 1, "");

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(result));
        }

        [Fact]
        public async Task GetState_Expect_ExpendRawXls()
        {
            // Arrange
            var mockFacade = new Mock<IExpenditureRawMaterialService>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
                .Returns(new MemoryStream());

            var mockFacade2 = new Mock<IReceiptRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade, mockFacade2);
            var result = customsReportController.GetXlsIN( DateTimeOffset.Now, DateTimeOffset.Now);

            // Assert
            //Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.GetType().GetProperty("ContentType").GetValue(result, null));
        }

        [Fact]
        public async Task GetState_UnExpect_ExpendRawXls()
        {
            // Arrange
            var mockFacade = new Mock<IExpenditureRawMaterialService>();
            //mockFacade.Setup(x => x.GenerateExcel(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
            //    .Returns(new MemoryStream());

            var mockFacade2 = new Mock<IReceiptRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade, mockFacade2);
            var result = customsReportController.GetXlsIN(DateTimeOffset.Now, DateTimeOffset.Now);

            // Assert
            //Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(result));
        }

        [Fact]
        public async Task GetState_Expect_ReceiptRaw()
        {
            // Arrange
            var mockFacade = new Mock<IReceiptRawMaterialService>();
            mockFacade.Setup(x => x.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Tuple.Create(new List<ReceiptRawMaterialViewModel>(), 1));

            var mockFacade2 = new Mock<IExpenditureRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade2, mockFacade);
            var result = customsReportController.GetReceiptRawMaterial(DateTime.Now, DateTime.Now, 1, 1, "");

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }

        [Fact]
        public async Task GetState_UnExpect_ReceiptRaw()
        {
            // Arrange
            var mockFacade = new Mock<IReceiptRawMaterialService>();
            //mockFacade.Setup(x => x.GetReport(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
            //    .Returns(Tuple.Create(new List<ExpenditureRawMaterialViewModel>(), 1));

            var mockFacade2 = new Mock<IExpenditureRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade2, mockFacade);
            var result = customsReportController.GetReceiptRawMaterial( DateTime.Now, DateTime.Now, 1, 1, "");

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(result));
        }

        [Fact]
        public async Task GetState_Expect_ReceiptRawXls()
        {
            // Arrange
            var mockFacade = new Mock<IReceiptRawMaterialService>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new MemoryStream());

            var mockFacade2 = new Mock<IExpenditureRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade2, mockFacade);
            var result = customsReportController.GetExcelRawMaterial(DateTime.Now, DateTime.Now);

            // Assert
            //Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.GetType().GetProperty("ContentType").GetValue(result, null));
        }

        [Fact]
        public async Task GetState_UnExpect_ReceiptRawXls()
        {
            // Arrange
            var mockFacade = new Mock<IReceiptRawMaterialService>();
            //mockFacade.Setup(x => x.GenerateExcel(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
            //    .Returns(new MemoryStream());

            var mockFacade2 = new Mock<IExpenditureRawMaterialService>();
            CustomsReportController customsReportController = GetCustomsReportController(mockFacade2, mockFacade);
            var result = customsReportController.GetExcelRawMaterial(DateTime.Now, DateTime.Now);

            // Assert
            //Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(result));
        }
    }
}