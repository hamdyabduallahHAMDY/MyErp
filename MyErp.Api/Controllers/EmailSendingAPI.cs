using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace MyErp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailSendingAPI : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required");

            string subject = "";
            string body = "";

            switch (dto.TemplateType)
            {
                case EmailTemplateType.FirstContact:
                    subject = "Welcome to our services";
                    body = $"Hello {dto.Name},<br/><br/>Thank you for contacting us. We will get back to you shortly.<br/><br/>Best Regards,<br/>Sales Team";
                    break;

                case EmailTemplateType.OfferPrice:
                    subject = "Our Offer Price";
                    body = $"Hello {dto.Name},<br/><br/>Please find our offer price attached or below.<br/><br/>Best Regards,<br/>Sales Team";
                    break;

                case EmailTemplateType.FollowUp:
                    subject = "Following up with you";
                    body = $"Hello {dto.Name},<br/><br/>Just checking if you had time to review our previous message.<br/><br/>Best Regards,<br/>Sales Team";
                    break;

                default:
                    return BadRequest("Invalid template type");
            }

            try
            {
    //            var smtp = new SmtpClient("smtp.hostinger.com")
    //            {
    //                Port = 465,
    //                EnableSsl = true,
    //                UseDefaultCredentials = false,
    //                Credentials = new NetworkCredential(
    //    "Abduallah.Hamdy@flexe-soft.com",
    //    "Hamdy@word1"
    //)
    //            };

    //            var message = new MailMessage
    //            {
    //                From = new MailAddress("Abduallah.Hamdy@flexe.com"),
    //                Subject = subject,
    //                Body = body,
    //                IsBodyHtml = true
    //            };

    //            message.To.Add(dto.Email);

    //            await smtp.SendMailAsync(message);
                var smtp = new SmtpClient("smtp.hostinger.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
        "Abduallah.Hamdy@flexe-soft.com",
        "Hamdy@word1"
    )
                };

                var message = new MailMessage
                {
                    From = new MailAddress("Abduallah.Hamdy@flexe-soft.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(dto.Email);

                await smtp.SendMailAsync(message);

                return Ok(new
                {
                    message = "Email sent successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }
    }

    public class SendEmailDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public EmailTemplateType TemplateType { get; set; }
    }

    public enum EmailTemplateType
    {
        FirstContact = 1,
        OfferPrice = 2,
        FollowUp = 3
    }
}