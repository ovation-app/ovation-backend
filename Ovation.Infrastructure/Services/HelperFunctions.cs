using MimeKit;
using Ovation.Application.Constants;
using System.Globalization;
using System.Text.RegularExpressions;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Ovation.Persistence.Services
{
    public static class HelperFunctions
    {        

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException _)
            {
                return false;
            }
            catch (ArgumentException _)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException _)
            {
                return false;
            }
        }

        public static async Task<bool> ValidateEmailDomainAsync(string email)
        {
            var domain = email.Split('@')[1];

            try
            {
                var client = new SmtpClient();
                await client.ConnectAsync(domain, 25);
                client.Disconnect(true);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ShrinkWalletAddress(string address, int startLength = 6, int endLength = 4)
        {
            if (string.IsNullOrEmpty(address))
                return string.Empty;

            if (address.Length <= startLength + endLength)
                return address; // Address is too short to shrink, return as is

            string start = address.Substring(0, startLength);
            string end = address.Substring(address.Length - endLength);

            return $"{start}***{end}";
        }

        public static async Task<int> SendOtpResetPassword(string recipient)
        {
            var from = Constant.NoReplyEmail;
            var connection = Constant.EmailServer;
            var password = Environment.GetEnvironmentVariable("EMAIL_KEY")!;
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = "Reset Password OTP - Ovation Technologies";
            var otp = GenerateOTP();
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{GetMailBodyTemplateForResetPassword(otp)}" };
            using var smtp = new SmtpClient();
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Connect(connection, 587);
            smtp.Authenticate(from, password);

            try
            {
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return otp;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static async Task<int> SendOtpEmailVerification(string recipient, string code)
        {
            try
            {
                var from = Constant.NoReplyEmail;
                var connection = Constant.EmailServer;
                var password = Environment.GetEnvironmentVariable("EMAIL_KEY")!;
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(from));
                email.To.Add(MailboxAddress.Parse(recipient));
                email.Subject = "Email Verification - Ovation Technologies";
                var otp = GenerateOTP();
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{GetMailBodyTemplateForEmailVerificatio(otp, code)}" };
                using var smtp = new SmtpClient();
                smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                smtp.Connect(connection, 587);
                smtp.Authenticate(from, password);

                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return otp;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static async Task SendWelcomeNewsletterEmail(string recipient)
        {
            var from = Constant.NoReplyEmail;
            var connection = Constant.EmailServer;
            var password = Environment.GetEnvironmentVariable("EMAIL_KEY")!;
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = "Welcome to Ovation";
            var otp = GenerateOTP();
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{GetMailBodyTemplateForNewsletter(recipient.Split('@').First())}" };
            using var smtp = new SmtpClient();
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Connect(connection, 465);
            smtp.Authenticate(from, password);

            try
            {
                await smtp.SendAsync(email);
                //smtp.Disconnect(true);
            }
            catch (Exception)
            { }
        }

        private static string GetMailBodyTemplateForResetPassword(int otp)
        {
            var body = "\r\n<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Ovation Technologies</title>\r\n    <style>\r\n        *{\r\n            font-family:Arial, Helvetica, sans-serif;\r\n        }\r\n\r\n        body{\r\n            /* width: 100%; */\r\n            height: auto;\r\n\r\n            background-color: #d2d5e0;\r\n            width: 100%;\r\n            height: 100%;\r\n            display: flex;\r\n            align-items: center;\r\n            justify-content: center;\r\n            flex-direction: column;\r\n        }\r\n\r\n        /* .container{\r\n            background-color: #d2d5e0;\r\n            width: 100%;\r\n            height: 100%;\r\n            display: flex;\r\n            align-items: center;\r\n            justify-content: center;\r\n            flex-direction: column;\r\n\r\n        } */\r\n\r\n        .content{\r\n            background-color: white;\r\n            width: 450px;\r\n            margin-top: 20px;\r\n            margin-bottom: 20px;\r\n            border-radius: 0.5rem;\r\n            padding: 20px;\r\n        }\r\n\r\n        .discription{\r\n            line-height: 1.5rem;\r\n            font-size: 15px;\r\n            color: rgb(61, 59, 59);\r\n            \r\n        }\r\n\r\n        .nav{\r\n            display: flex;\r\n            align-items: center;\r\n            justify-content: space-between;\r\n        }\r\n\r\n        .nav > a{\r\n            text-decoration: none;\r\n            color: #423f3f;\r\n            font-weight: bold;\r\n            border: 2px solid #423f3f;\r\n            padding: 15px;\r\n            border-radius: 0.5rem;\r\n        }\r\n\r\n        .top-description{\r\n            font-weight: 100;\r\n            word-spacing: 0.2rem;\r\n            color: rgb(61, 59, 59);\r\n        }\r\n\r\n        .otp{\r\n            width: 100%;\r\n            background-color: #d2d5e0;\r\n            padding-top: 30px;\r\n            padding-bottom: 30px;\r\n            text-align: center;\r\n            font-weight: bold;\r\n            font-size: 50px;\r\n            border-radius: 0.5rem;\r\n            letter-spacing: 1rem;\r\n        }\r\n\r\n\r\n        .logo{\r\n            width: 40px;\r\n        }\r\n\r\n        .why{\r\n            width: 400px;\r\n            text-align: center;\r\n            font-size: 12px;\r\n            color: rgb(71, 68, 68);\r\n            font-weight: 600;\r\n            margin-bottom: 20px;\r\n        }\r\n\r\n    </style>\r\n</head>\r\n<body>\r\n    <!-- <p class=\"container\"> -->\r\n        <div class=\"content\">\r\n            <h1 class=\"heading\">\r\n                Password Reset\r\n            </h1>\r\n            \r\n            <p class=\"discription top-description\">\r\n                To proceed, you need to complete this step before continuing your password reset on Ovation Technologies.\r\n                 Please enter this verification code to continue with your password reset process on Ovation Technologies:\r\n            </p>\r\n    \r\n            \r\n            <p class=\"otp\">\r\n                {otpvalue}\r\n            </p>\r\n            \r\n            <p class=\"discription\">\r\n                If you didn't initiate password reset with Ovation Technologies, please ignore this message. This OTP will be valid only for this request.\r\n            </p>\r\n            <span class=\"discription\">\r\n                Thanks,\r\n            </span>\r\n                <br>\r\n            <span class=\"discription\">\r\n                 &copy; 2024 Ovation Technologies.\r\n            </span>\r\n        </div>\r\n\r\n        <span class=\"why\">\r\n            You have received this email because you've requested to reset your account password on Ovation App, if you didn't initiate this request you can ignore it and don't share the OTP you've received\r\n        </span>\r\n        \r\n    <!-- </p> -->\r\n</body>\r\n</html>";
            //body.Replace("{request}", type);
            return body.Replace("{otpvalue}", otp.ToString());
        }

        private static string GetMailBodyTemplateForNewsletter(string username)
        {
            var body = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\r\n    <link\r\n        href=\"https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap\"\r\n        rel=\"stylesheet\">\r\n    <title>Welcome to Ovation</title>\r\n    <style>\r\n        body {\r\n            font-family: \"Inter\", sans-serif;\r\n            background-color: #1a1a1a;\r\n            color: #ffffff;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n\r\n        .container {\r\n            max-width: 560px;\r\n            margin: 0 auto;\r\n            background-color: #111111;\r\n            border-radius: 20px;\r\n            padding: 16px;\r\n        }\r\n\r\n        .container h1 {\r\n            font-size: 24px;\r\n            font-weight: 600;\r\n            margin-bottom: 23px;\r\n        }\r\n\r\n        .container p {\r\n            color: #CCCCCC;\r\n            font-size: 18px;\r\n            font-weight: 400;\r\n            margin-bottom: 16px;\r\n        }\r\n\r\n        .logo {\r\n            background-color: #2e6930;\r\n            color: white;\r\n            padding: 20px;\r\n            text-align: center;\r\n            font-size: 24px;\r\n            border-radius: 5px;\r\n            margin-bottom: 20px;\r\n        }\r\n\r\n        p {\r\n\r\n            line-height: 30px;\r\n            letter-spacing: 0.5%;\r\n        }\r\n\r\n        .button {\r\n            display: inline-block;\r\n            background-color: #CFF073;\r\n            color: #111115;\r\n            padding: 10px 14px;\r\n            text-decoration: none;\r\n            border-radius: 30px;\r\n            margin-right: 12px;\r\n            font-weight: 600;\r\n            font-size: 12px;\r\n\r\n            margin-bottom: 10px;\r\n        }\r\n\r\n        .footer {\r\n            margin-top: 20px;\r\n            text-align: left;\r\n            display: flex;\r\n            flex-direction: column;\r\n            gap: 6px;\r\n        }\r\n\r\n        .footer span {\r\n            font-size: 18px;\r\n\r\n            color: #CCCCCC;\r\n            font-weight: 400;\r\n        }\r\n    </style>\r\n</head>\r\n\r\n<body>\r\n    <div class=\"container\">\r\n        <h1>Hi {usernameValue}</h1>\r\n        <div>\r\n\r\n            <img src=\"https://res.cloudinary.com/dkv32rrmi/image/upload/v1724308301/hk3q7kjgybz261hgv6nl.png\"\r\n                alt=\"Header Image\" style=\"max-width: 100%;\">\r\n        </div>\r\n        <p>Thank you for subscribing to stay in the loop with Ovation. We're thrilled to have you join our growing\r\n            community as we prepare for our exciting launch in the near future.</p>\r\n        <p>As one of our early supporters, you'll be the first to know about updates, exclusive features, and special\r\n            rewards coming your way. We're building something amazing together, and we can't wait to share it with you!\r\n        </p>\r\n        <p>Also, if you're interested in partnering with us, we'd love to hear from you! Apply to be a partner and join\r\n            us in building something amazing for the future of Web3.</p>\r\n        <div>\r\n            <a href=\"https://x.com/Ovation_Network\" class=\"button\">Apply to be a partner</a>\r\n\r\n            <a href=\"https://www.linkedin.com/company/ovationnetwork/posts/?feedView=all\" class=\"button\">Follow us on\r\n                linkedin</a>\r\n        </div>\r\n        <div class=\"footer\">\r\n            <span>Best regards,</span>\r\n            <span>Team Ovation 💚</span>\r\n        </div>\r\n    </div>\r\n</body>\r\n\r\n</html>";

            return body.Replace("{usernameValue}", username);
        }

        private static string GetMailBodyTemplateForEmailVerificatio(int otp, string code)
        {
            var body = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap\" rel=\"stylesheet\">\r\n    <title>Today’s the Day: Ovation’s Public MVP Launch is Live!</title>\r\n    <style>\r\n        body {\r\n            font-family: \"Inter\", sans-serif;\r\n            background-color: #1a1a1a;\r\n            color: #ffffff;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n        li {\r\n            color: #ffffff;\r\n        }\r\n        .container {\r\n            max-width: 700px;\r\n            margin: 0 auto;\r\n            background-color: #111111;\r\n            border-radius: 20px;\r\n            padding: 16px;\r\n        }\r\n        h1, h2 {\r\n            color: #ffffff;\r\n            font-size: 24px;\r\n            font-weight: 600;\r\n            margin-bottom: 23px;\r\n        }\r\n\t.title {\r\n            font-weight: 600;\r\n            font-size: 20px;\r\n        }\r\n        p {\r\n            color: #CCCCCC;\r\n            font-size: 18px;\r\n            font-weight: 400;\r\n            margin-bottom: 16px;\r\n            line-height: 30px;\r\n            letter-spacing: 0.5%;\r\n        }\r\n        .logo {\r\n            background-color: #2e6930;\r\n            color: white;\r\n            padding: 20px;\r\n            text-align: center;\r\n            font-size: 24px;\r\n            border-radius: 5px;\r\n            margin-bottom: 20px;\r\n        }\r\n        .button {\r\n            display: inline-block;\r\n            background-color: #CFF073;\r\n            color: #111115;\r\n            padding: 18px 14px;\r\n            text-decoration: none;\r\n            border-radius: 30px;\r\n            margin-right: 12px;\r\n            font-weight: 600;\r\n            font-size: 15px;\r\n            width: 200px;\r\n            text-align: center;\r\n            margin-top: 10px;\r\n            margin-bottom: 10px;\r\n        }\r\n        .footer {\r\n            margin-top: 20px;\r\n            text-align: left;\r\n            display: flex;\r\n            flex-direction: column;\r\n            gap: 6px;\r\n        }\r\n        .footer span {\r\n            font-size: 18px;\r\n            color: #CCCCCC;\r\n            font-weight: 400;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <p class=\"title\">Hello Ovationist,</p>\r\n<div>\r\n            <img src=\"https://res.cloudinary.com/dkv32rrmi/image/upload/v1724308301/hk3q7kjgybz261hgv6nl.png\" alt=\"Ovation MVP Header Image\" style=\"max-width: 100%;\">\r\n        </div>\r\n    <p class=\"desc\">\r\n      Verify your email address on Ovation!\r\n    </p>\r\n    <a class=\"button\" href=\"https://ovation.network/verify-account?email={codevalue}&code={otpvalue}\">\r\n      Verify Email\r\n    </a>\r\n    <p>You can also copy link below into your browser window</p>\r\n    <a href=\"https://ovation.network/verify-account?email={codevalue}&code={otpvalue}\">\r\n      https://ovation.network/verify-account?email={codevalue}&code={otpvalue}\r\n    </a>\r\n        <div class=\"footer\">\r\n            <span><strong>Welcome aboard!</strong></span>\r\n        </div>\r\n        <div class=\"footer\">\r\n            <span>The Ovation Team</span>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>\r\n";
            body = body.Replace("{codevalue}", code);
            return body.Replace("{otpvalue}", otp.ToString());
        }

        private static int GenerateOTP()
        {
            var r = new Random();
            return r.Next(100000, 999999);
        }
    }
}
