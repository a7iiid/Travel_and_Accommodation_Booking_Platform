﻿using Application.DTOs.PaymentDTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;

namespace Application.Services
{
    public class PaymentServices
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentServices(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves all payments.
        /// </summary>
        public async Task<PaginatedList<Payment>> GetAllPaymentsAsync(int pageNumber=1, int pageSize=5)
        {
            var payments = await _paymentRepository.GetAllAsync( pageNumber,  pageSize);
            return payments;
        }

        /// <summary>
        /// Retrieves a payment by its ID.
        /// </summary>
        public async Task<PaymentDTO> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

            return _mapper.Map<PaymentDTO>(payment);
        }

        /// <summary>
        /// Adds a new payment.
        /// </summary>
        public async Task<CreateOrderResult> AddPaymentAsync(PaymentDTO paymentDTO)
        {
            var paymentEntity = _mapper.Map<Payment>(paymentDTO);
           CreateOrderResult createOrderResult =await _paymentRepository.InsertAsync(paymentEntity);
            
            return createOrderResult;
        }

        /// <summary>
        /// Updates an existing payment.
        /// </summary>
        public async Task UpdatePaymentAsync(Guid id, PaymentDTO paymentDTO)
        {
            var existingPayment = await _paymentRepository.GetByIdAsync(id);
            if (existingPayment == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found.");

            _mapper.Map(paymentDTO, existingPayment);
            await _paymentRepository.UpdateAsync(existingPayment, id);
        }

        /// <summary>
        /// Deletes a payment by its ID.
        /// </summary>
        public async Task<bool> DeletePaymentAsync(Guid id)
        {
            return await _paymentRepository.DeleteAsync(id);
        }

        public async Task VerifyAndUpdatePaymentStatusAsync(string orderId,PaymentStatus paymentStatus)
        {
            try
            {
                 await _paymentRepository.UpdatePaymentStatusWebHookAsync(orderId,paymentStatus);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred while verifying and updating the payment status.", ex);
            }
        }
    }
}
