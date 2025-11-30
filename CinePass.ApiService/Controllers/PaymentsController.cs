using CinePass.Core.Services;
using CinePass.Shared.DTOs.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinePass.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> Create([FromBody] PaymentRequest dto)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(dto);
            return Ok(payment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("momo/create")]
    public async Task<ActionResult> CreateMoMoPayment([FromQuery] int bookingId)
    {
        try
        {
            var paymentUrl = await _paymentService.GenerateMoMoPaymentUrlAsync(bookingId);
            return Ok(new { paymentUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("vnpay/create")]
    public async Task<ActionResult> CreateVnPayPayment([FromQuery] int bookingId)
    {
        try
        {
            var paymentUrl = await _paymentService.GenerateVnPayPaymentUrlAsync(bookingId);
            return Ok(new { paymentUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("callback")]
    public async Task<ActionResult<PaymentResponse>> Callback([FromBody] PaymentCallback callback)
    {
        try
        {
            var payment = await _paymentService.ProcessCallbackAsync(callback);
            return Ok(payment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}