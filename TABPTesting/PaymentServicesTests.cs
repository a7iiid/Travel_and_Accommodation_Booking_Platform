
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

        [Fact]
        public async Task GetPaymentByIdAsync_ReturnPaymentDTO_WhenPaymentNotExist()
        {
            //Arrange
            var paymentId= Guid.NewGuid();
            _mockPaymentRepo.Setup(r => r.GetByIdAsync(paymentId))
                .ReturnsAsync((Payment)null);
            //Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                               () => _paymentServices.GetPaymentByIdAsync(paymentId));
        }

        //[Fact]
        //public async Task AddPaymentAsync_ReturnsApprovalUrl_WhenPaymentAdded()
        //{
        //    //Arrange
        //    var paymentDto = new PaymentDTO();
        //    var paymentEntity = new Payment();
        //    var approvalUrl = "https://www.example.com";

        //    _mockMapper.Setup(m => m.Map<Payment>(paymentDto))
        //        .Returns(paymentEntity);
        //    _mockPaymentRepo.Setup(r => r.InsertAsync(paymentEntity))
        //        .ReturnsAsync(approvalUrl);

        //    //Act
        //    var result = await _paymentServices.AddPaymentAsync(paymentDto);

        //    //Assert
        //    _mockMapper.Verify(m => m.Map<Payment>(paymentDto), Times.Once);
        //    _mockPaymentRepo.Verify(r => r.InsertAsync(paymentEntity), Times.Once);
        //    Assert.Equal(approvalUrl, result);
        //}

        //[Fact]
        //public async Task AddPaymentAsync_ReturnsApprovalUrl_WhenPaymentNotAdded()
        //{ 
        //    //Arrange
        //    var paymentDto = new PaymentDTO();
        //    var paymentEntity = new Payment();
        //    _mockMapper.Setup(m => m.Map<Payment>(paymentDto))
        //        .Returns(paymentEntity);
        //    _mockPaymentRepo.Setup(r => r.InsertAsync(paymentEntity))
        //        .ReturnsAsync((string?)null);
        //    //Act
        //    var result = await _paymentServices.AddPaymentAsync(paymentDto);
        //    //Assert
        //    _mockMapper.Verify(m => m.Map<Payment>(paymentDto), Times.Once);
        //    _mockPaymentRepo.Verify(r => r.InsertAsync(paymentEntity), Times.Once);
        //    Assert.Null(result);
        //}

        [Fact]
        public async Task UpdatePaymentAsync_UpdatesExistingPayment()
        {
            //Arrange
            var paymentId = Guid.NewGuid();
            var existingPayment = new Payment { Id = paymentId };
            var paymentDto = new PaymentDTO();

            _mockPaymentRepo.Setup(r => r.GetByIdAsync(paymentId))
                .ReturnsAsync(existingPayment);

            //Act
            await _paymentServices.UpdatePaymentAsync(paymentId, paymentDto);

            //Assert
            _mockPaymentRepo.Verify(r => r.UpdateAsync(existingPayment, paymentId), Times.Once);
        }

        [Fact]
        public async Task UpdatePaymentAsync_ThrowsKeyNotFoundException_UpdatesNotExistingPayment()
        {
            //Arrange
            var paymentId = Guid.NewGuid();
            _mockPaymentRepo.Setup(r => r.GetByIdAsync(paymentId))
                .ReturnsAsync((Payment)null);
            _mockMapper.Setup(m => m.Map<PaymentDTO>(It.IsAny<Payment>()))
                .Returns(new PaymentDTO());
            //Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                               () => _paymentServices.UpdatePaymentAsync(paymentId, new PaymentDTO()));

        }

        [Fact]
        public async Task DeletePaymentAsync_ReturnTrue_WhenSuccessful()
        {
            //Arrange 
            var paymentId = Guid.NewGuid();
            _mockPaymentRepo.Setup(r => r.DeleteAsync(paymentId))
                .ReturnsAsync(true);
            //Act
            var result = await _paymentServices.DeletePaymentAsync(paymentId);
            //Assert
            Assert.True(result);
        }

        }
}
