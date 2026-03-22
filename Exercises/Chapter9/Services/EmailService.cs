using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Booking Car", "filmbooking4@gmail.com"));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = "<h3>Cảm ơn bạn đã thanh toán!</h3>" };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync("filmbooking4@gmail.com", "yjqkiiwepguerbjk");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

}