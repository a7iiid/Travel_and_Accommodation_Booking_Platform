﻿
using Application.DTOs.PaymentDTOs;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Model;
using Moq;
using System.Data.SqlTypes;
namespace TABPTesting
{
    public class PaymentServicesTests
    {
        private readonly Mock<IPaymentRepository> _mockPaymentRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PaymentServices _paymentServices;
        public PaymentServicesTests()
        {
            _mockPaymentRepo = new Mock<IPaymentRepository>();
            _mockMapper = new Mock<IMapper>();

            _paymentServices = new PaymentServices(
                               _mockPaymentRepo.Object,
                                _mockMapper.Object );
        }
        [Fact]
        public async Task GetPaymentsAsync_ReturnsPaginatedList_WhenPaymentsExist()
        {
            // Arrange
            var payments = new List<Payment> { new Payment(), new Payment() };
            var paginatedPayments = new PaginatedList<Payment>(payments, new PageData(2, 10, 1));

            _mockPaymentRepo.Setup(r => r.GetAllAsync( 1, 10))
                .ReturnsAsync(paginatedPayments);

            // Act
            var result = await _paymentServices.GetAllPaymentsAsync(1, 10);

            // Assert
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetPaymentByIdAsync_ReturnPaymentDTO_WhenPaymentExist() 
        { 
            //Arrange
            var paymentId= Guid.NewGuid();
            var payment = new Payment { Id = paymentId };
            var paymentDto = new PaymentDTO();

            _mockPaymentRepo.Setup(r => r.GetByIdAsync(paymentId))
                .ReturnsAsync(payment);
            _mockMapper.Setup(m => m.Map<PaymentDTO>(payment))
                .Returns(paymentDto);
            //Act
           var result= await _paymentServices.GetPaymentByIdAsync(paymentId);

            //Assert

            _mockPaymentRepo.Verify(r => r.GetByIdAsync(paymentId), Times.Once);
            Assert.Equal(paymentDto, result);

        }

    }
}
