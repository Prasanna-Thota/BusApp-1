using BusApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace BusApplication.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost("sendEmail")]
        public IActionResult SendEmail([FromBody] EmailDetails email)
        {
            string fromMail = "manikantaneeli05@gmail.com";
            string fromPassword = "inja yvwd kbsp ypit";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.To.Add(email.ToEmail); 
            message.Subject = email.Subject;
            message.Body = email.Body;

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                smtpClient.EnableSsl = true;

                smtpClient.Send(message);
            }
            return Ok(); // Return HTTP 200 OK if email sent successfully
        }
    }
}
