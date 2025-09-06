using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Ovation.Application.Constants;
using Ovation.Application.DTOs;
using Ovation.Application.Repositories;
using Ovation.Domain.Entities;
using Ovation.Persistence.Services;

namespace Ovation.Persistence.Repositories
{
    internal class NewsletterRepository(IServiceScopeFactory serviceScopeFactory, IHttpClientFactory factory) : BaseRepository<Newsletter>(serviceScopeFactory), INewsletterRepository
    {
        public async Task<int> getTotalSubscribers()
        {
            return await _context.Newsletters.CountAsync();
        }

        public async Task<ResponseData> PostSubscriberAsync(NewsletterDto newsletter)
        {
            try
            {
                if (!HelperFunctions.IsValidEmail(newsletter.SubscriberEmail))
                    return new ResponseData{ Message = "Invalid Email Address" };


                var entity = await _context.Newsletters.FirstOrDefaultAsync(n => n.SubscriberEmail == newsletter.SubscriberEmail);

                if (entity != null)
                    return new ResponseData { Message = "Subscriber's email already exist", StatusCode = 409 };

                await _context.Newsletters.AddAsync(new Newsletter { SubscriberEmail = newsletter.SubscriberEmail });

                await HelperFunctions.SendWelcomeNewsletterEmail(newsletter.SubscriberEmail.Trim());

                return await _unitOfWork.SaveChangesAsync() > 0 ? new ResponseData { Status = true } : new ResponseData();
            }
            catch (DbUpdateException ex)
            {
                return new ResponseData();
            }
        }

        private async Task<ResponseData> SendFirstLaunchEmail(int page)
        {
            var signUps = await _context.Newsletters
                .Where(_ => _.SubscriberEmail != null)
                .OrderBy(_ => _.CreatedDate)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(_ => _.SubscriberEmail)
                .ToListAsync();
            var sent = 0;

            if (signUps == null || signUps.Count <= 0) return new ResponseData { Status = true, Message = "Not Email was fetched!" };

            var from = Constant.NoReplyEmail;
            var connection = Constant.EmailServer;
            var password = Environment.GetEnvironmentVariable("EMAIL_KEY")!;

            using var smtp = new SmtpClient();
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Connect(connection, 465);
            smtp.Authenticate(from, password);


            try
            {
                foreach (var emaill in signUps)
                {
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(from));

                    email.Subject = "Hello Ovationist, Welcome to Ovation MVP";
                    email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{GetMailBodyTemplateForLaunchFirstEmail()}" };
                    email.To.Add(MailboxAddress.Parse(emaill));
                    await smtp.SendAsync(email);

                    sent++;
                }

                smtp.Disconnect(true);
                return new ResponseData { Message = $"Sent {sent} emails", Status = true };
            }
            catch (Exception _)
            {
                return new ResponseData { Message = $"Sent {sent} emails. \n{_.Message}" };
            }

        }

        private async Task<ResponseData> SendSecondLaunchEmail(int page)
        {
            var signUps = await _context.Newsletters
                .Where(_ => _.SubscriberEmail != null)
                .OrderBy(_ => _.CreatedDate)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(_ => _.SubscriberEmail)
                .ToListAsync();

            var sent = 0;
            if (signUps == null && signUps.Count <= 0) return new ResponseData { Status = true, Message = "Not Email was fetched!" };

            var from = Constant.NoReplyEmail;
            var connection = Constant.EmailServer;
            var password = Environment.GetEnvironmentVariable("EMAIL_KEY")!;

            using var smtp = new SmtpClient();
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Connect(connection, 465);
            smtp.Authenticate(from, password);

            try
            {
                foreach (var emaill in signUps)
                {
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(from));

                    email.Subject = "Hello Ovationist, Welcome to Ovation MVP";
                    email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{GetMailBodyTemplateForLaunchSecondEmail()}" };
                    email.To.Add(MailboxAddress.Parse(emaill));
                    await smtp.SendAsync(email);

                    sent++;
                }

                smtp.Disconnect(true);
                return new ResponseData { Message = $"Sent {sent} emails", Status = true };
            }
            catch (Exception _)
            {
                return new ResponseData { Message = $"Sent {sent} emails. \n{_.Message}" };
            }
        }

        private async Task<ResponseData> SendPublicLaunchEmail(int page)
        {
            var publicLaunch = DateTime.Parse("2024-10-11");

            var signUps = await _context.Newsletters
                .Where(_ => _.SubscriberEmail != null && _.CreatedDate < publicLaunch)
                .OrderBy(_ => _.CreatedDate)
                .Skip(perPage * (page - 1))
                .Take(perPage)
                .Select(_ => _.SubscriberEmail)
                .ToListAsync();

            //var signUps = new List<string> { "jackdbliss@gmail.com", "Malcolmhenzaga@gmail.com", "maanav.porwal@ovation.network", "grantweaver11@gmail.com" };
            var sent = 0;

            if (signUps == null || signUps.Count <= 0) return new ResponseData { Status = true, Message = "Not Email was fetched!" };

            var from = Constant.NoReplyEmail;
            var connection = Constant.EmailServer;
            var password = Environment.GetEnvironmentVariable("EMAIL_KEY")!;

            using var smtp = new SmtpClient();
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Connect(connection, 465);
            smtp.Authenticate(from, password);


            try
            {
                foreach (var emaill in signUps)
                {
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(from));

                    email.Subject = "Today’s the Day: Ovation’s Public MVP Launch is Live! 🚀";
                    email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{GetMailBodyTemplateForPublicLaunchEmail()}" };
                    email.To.Add(MailboxAddress.Parse(emaill));
                    await smtp.SendAsync(email);

                    sent++;
                }

                smtp.Disconnect(true);
                return new ResponseData { Message = $"Sent {sent} emails", Status = true };
            }
            catch (Exception _)
            {
                return new ResponseData { Message = $"Sent {sent} emails. \n{_.Message}" };
            }

        }

        private static string GetMailBodyTemplateForLaunchFirstEmail()
        {
            var body = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap\" rel=\"stylesheet\">\r\n    <title>Welcome to Ovation MVP</title>\r\n    <style>\r\n        body {\r\n            font-family: \"Inter\", sans-serif;\r\n            background-color: #1a1a1a;\r\n            color: #ffffff;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n        .container {\r\n            max-width: 700px;\r\n            margin: 0 auto;\r\n            background-color: #111111;\r\n            border-radius: 20px;\r\n            padding: 16px;\r\n        }\r\n        .container h1 {\r\n            font-size: 24px;\r\n            font-weight: 600;\r\n            margin-bottom: 23px;\r\n        }\r\n        .container p {\r\n            color: #CCCCCC;\r\n            font-size: 18px;\r\n            font-weight: 400;\r\n            margin-bottom: 16px;\r\n            line-height: 30px;\r\n            letter-spacing: 0.5%;\r\n        }\r\n        .logo {\r\n            background-color: #2e6930;\r\n            color: white;\r\n            padding: 20px;\r\n            text-align: center;\r\n            font-size: 24px;\r\n            border-radius: 5px;\r\n            margin-bottom: 20px;\r\n        }\r\n        .button {\r\n            display: inline-block;\r\n            background-color: #CFF073;\r\n            color: #111115;\r\n            padding: 10px 14px;\r\n            text-decoration: none;\r\n            border-radius: 30px;\r\n            margin-right: 12px;\r\n            font-weight: 600;\r\n            font-size: 12px;\r\nwidth:95%;\r\ntext-align:center;\r\nmargin-top:10px;\r\n            margin-bottom: 10px;\r\n        }\r\n        .footer {\r\n            margin-top: 20px;\r\n            text-align: left;\r\n            display: flex;\r\n            flex-direction: column;\r\n            gap: 6px;\r\n        }\r\n        .footer span {\r\n            font-size: 18px;\r\n            color: #CCCCCC;\r\n            font-weight: 400;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n               <div>\r\n            <img src=\"https://res.cloudinary.com/dkv32rrmi/image/upload/v1724308301/hk3q7kjgybz261hgv6nl.png\" alt=\"Header Image\" style=\"max-width: 100%;\">\r\n        </div>\r\n        <p>You made it! By receiving this email, you're officially one of the select few to get early access to our private MVP release of Ovation.Network.</p>\r\n        <p>This is your chance to be part of something truly special, and we're excited to have you with us on this journey.</p>\r\n        <p>Click the link below to get started, and have fun exploring. Let's make something amazing together!</p>\r\n        <div>\r\n            <a href=\"https://ovation.network/login\" class=\"button\">Get started</a>\r\n        </div>\r\n        <div class=\"footer\">\r\n            <span>Welcome aboard!</span>\r\n            <span> Team Ovation 💚</span>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

            return body;
        }

        private static string GetMailBodyTemplateForLaunchSecondEmail()
        {
            var body = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap\" rel=\"stylesheet\">\r\n    <title>Welcome to Ovation MVP</title>\r\n    <style>\r\n        body {\r\n            font-family: \"Inter\", sans-serif;\r\n            background-color: #1a1a1a;\r\n            color: #ffffff;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n         li{\r\n            color: #ffffff;\r\n        }\r\n       .container {\r\n            max-width: 700px;\r\n            margin: 0 auto;\r\n            background-color: #111111;\r\n            border-radius: 20px;\r\n            padding: 16px;\r\n        }\r\n        h1, h2 {\r\n            color: #ffffff;\r\n            font-size: 24px;\r\n            font-weight: 600;\r\n            margin-bottom: 23px;\r\n        }\r\n        p {\r\n            color: #CCCCCC;\r\n            font-size: 18px;\r\n            font-weight: 400;\r\n            margin-bottom: 16px;\r\n            line-height: 30px;\r\n            letter-spacing: 0.5%;\r\n        }\r\n        .logo {\r\n            background-color: #2e6930;\r\n            color: white;\r\n            padding: 20px;\r\n            text-align: center;\r\n            font-size: 24px;\r\n            border-radius: 5px;\r\n            margin-bottom: 20px;\r\n        }\r\n        .button {\r\n            display: inline-block;\r\n            background-color: #CFF073;\r\n            color: #111115;\r\n            padding: 18px 14px;\r\n            text-decoration: none;\r\n            border-radius: 30px;\r\n            margin-right: 12px;\r\n            font-weight: 600;\r\n            font-size: 12px;\r\nwidth:95%;\r\ntext-align:center;\r\nmargin-top:10px;\r\n            margin-bottom: 10px;\r\n        }\r\n        .footer {\r\n            margin-top: 20px;\r\n            text-align: left;\r\n            display: flex;\r\n            flex-direction: column;\r\n            gap: 6px;\r\n        }\r\n        .footer span {\r\n            font-size: 18px;\r\n            color: #CCCCCC;\r\n            font-weight: 400;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n                <div>\r\n            <img src=\"https://res.cloudinary.com/dkv32rrmi/image/upload/v1724308301/hk3q7kjgybz261hgv6nl.png\" alt=\"Ovation MVP Header Image\" style=\"max-width: 100%;\">\r\n        </div>\r\n        <h2>MVP</h2>\r\n        <p>We're excited to introduce you to our MVP (Minimum Viable Product), a streamlined version of our platform designed to give you an early look at the core features and functionalities we've been working on.</p>\r\n        \r\n        <h2>What's the Minimum Viable Product (MVP)?</h2>\r\n        <p>The <strong>Minimum Viable Product (MVP)</strong> is the first version of Ovation Social, designed to showcase our core capabilities. However, please note that <strong>not all the features we've talked about—like</strong> the functionalities you might expect from platforms such as Twitter (X), Discord, and OpenSea—are available in this release.</p>\r\n        \r\n        <p>This MVP represents our starting point. Your feedback will play a crucial role in helping us build and refine the features that will make Ovation.Network a platform that brings all those capabilities together in one app. Plus, your feedback will help us secure additional funding so we can deliver the full experience we're promising—and more.</p>\r\n        \r\n        <h2>Your Access Link</h2>\r\n        <p>Attached is the private access link for testers to create your Ovation.Network account. While this link isn't exclusive to you alone, we ask that you do not share it publicly just yet. In two weeks, the platform will be open for everyone, but during this phase, your early feedback is essential to help us identify bugs and improve the experience.</p>\r\n        \r\n        <p>If you plan to use Ovation.Network on your phone, we recommend saving this link for easy access:</p>\r\n        <ul>\r\n            <li><strong>On iPhone:</strong> Open the link, tap the \"Share\" button, and select \"Add to Home Screen.\"</li>\r\n            <li><strong>On Android:</strong> Tap the menu button (three dots) and select \"Add to Home Screen.\"</li>\r\n        </ul>\r\n        <p>This will make it simple to jump back in anytime.</p>\r\n        \r\n        <h2>Keep It Under Wraps</h2>\r\n        <p>During this private MVP release, we're asking that you don't post or share anything about Ovation.Network on social media or elsewhere until the public launch, which is just two weeks away. Your discretion is key to helping us fine-tune everything before the big reveal.</p>\r\n        \r\n        <h2>We Need Your Feedback</h2>\r\n        <p>What's most important to us right now is hearing from you. After you've had time to explore the platform, we'll be asking you to complete a feedback form. Your feedback will directly shape the next phase of development and help us tackle any issues before the public launch. It's also crucial for helping us prioritize features and secure the funding needed to bring Ovation.Network to its full potential. You can always take the feedback by clicking the button below or directly click on the feedback button on the web app.</p>\r\n        \r\n        <a href=\"https://ovation.network/login\" class=\"button\">Submit a feedback</a>\r\n        \r\n        <div class=\"footer\">\r\n            <span><strong>Welcome aboard!</strong></span>\r\n        </div>\r\n      \r\n        <div class=\"footer\">\r\n                  <span>The Ovation Team</span>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

            return body;
        }

        private static string GetMailBodyTemplateForPublicLaunchEmail()
        {
            var body = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\r\n    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap\" rel=\"stylesheet\">\r\n    <title>Today’s the Day: Ovation’s Public MVP Launch is Live!</title>\r\n    <style>\r\n        body {\r\n            font-family: \"Inter\", sans-serif;\r\n            background-color: #1a1a1a;\r\n            color: #ffffff;\r\n            margin: 0;\r\n            padding: 20px;\r\n        }\r\n\r\n        li {\r\n            color: #ffffff;\r\n        }\r\n\r\n        .container {\r\n            max-width: 700px;\r\n            margin: 0 auto;\r\n            background-color: #111111;\r\n            border-radius: 20px;\r\n            padding: 16px;\r\n        }\r\n\r\n        h1, h2 {\r\n            color: #ffffff;\r\n            font-size: 24px;\r\n            font-weight: 600;\r\n            margin-bottom: 23px;\r\n        }\r\n\r\n        .texts {\r\n            margin-top: 12px;\r\n        }\r\n\r\n        p {\r\n            color: #CCCCCC;\r\n            font-size: 18px;\r\n            font-weight: 400;\r\n            margin-bottom: 16px;\r\n            line-height: 30px;\r\n            letter-spacing: 0.5%;\r\n        }\r\n\r\n        .logo {\r\n            background-color: #2e6930;\r\n            color: white;\r\n            padding: 20px;\r\n            text-align: center;\r\n            font-size: 24px;\r\n            border-radius: 5px;\r\n            margin-bottom: 20px;\r\n        }\r\n\r\n        .button {\r\n            display: inline-block;\r\n            background-color: #CFF073;\r\n            color: #111115;\r\n            padding: 18px 14px;\r\n            text-decoration: none;\r\n            border-radius: 30px;\r\n            margin-right: 12px;\r\n            font-weight: 600;\r\n            font-size: 12px;\r\n            width: 95%;\r\n            text-align: center;\r\n            margin-top: 10px;\r\n            margin-bottom: 10px;\r\n        }\r\n\r\n        .footer {\r\n            margin-top: 20px;\r\n            text-align: left;\r\n            display: flex;\r\n            flex-direction: column;\r\n            gap: 6px;\r\n        }\r\n\r\n            .footer span {\r\n                font-size: 18px;\r\n                color: #CCCCCC;\r\n                font-weight: 400;\r\n            }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        \r\n        <h2>Hello Ovationist,</h2>\r\n\r\n        <div>\r\n            <img src=\"https://res.cloudinary.com/dkv32rrmi/image/upload/v1724308301/hk3q7kjgybz261hgv6nl.png\" alt=\"Ovation MVP Header Image\" style=\"max-width: 100%;\">\r\n        </div>\r\n        <div class=\"texts\">\r\n            <p>\r\n                Today is the day! Ovation’s public MVP launch is now available, and\r\n                we’re excited to share it with you. Since our private launch, we’ve\r\n                addressed several bug fixes and improved wallet address access to\r\n                enhance your experience.\r\n            </p>\r\n            <p>\r\n                We’re also working on big new features coming before the end of the\r\n                year, so stay tuned. In the meantime, don’t forget to share your\r\n                Ovation Profile link on social media and invite others to join the\r\n                platform.\r\n            </p>\r\n            <p>\r\n                Click below to get started, and let’s make something amazing together!\r\n            </p>\r\n        </div>\r\n\r\n\r\n\r\n        <a href=\"https://ovation.network/login\" class=\"button\">Get Started</a>\r\n\r\n        <div class=\"footer\">\r\n            <span><strong>Welcome aboard!</strong></span>\r\n        </div>\r\n\r\n        <div class=\"footer\">\r\n            <span>The Ovation Team</span>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

            return body;
        }

        private async Task AddUsersAffilationCode()
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var users = await _context.Users.Select(x => new { UserId = new Guid(x.UserId) }).ToListAsync();

                foreach (var user in users)
                {
                    var affilation = new UserAffilation
                    {
                        UserId = user.UserId.ToByteArray(),
                        Code = user.UserId.ToString().Split('-').Last()
                    };

                    await _context.UserAffilations.AddAsync(affilation);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception _)
            {
                await transaction.RollbackAsync();
            }
        }

        //private async Task<ResponseData> UpdateUsersGoogleIdAsync()
        //{
        //    var res = await _context.Users
        //        .AsSingleQuery()
        //        .IgnoreAutoIncludes()
        //        .ToListAsync();

        //    var updatedCount = 0;
        //    var transactions = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        if(res != null && res.Count > 0)
        //        {
        //            foreach (var item in res)
        //            {
        //                item.GoogleId = item.Email;
        //            }

        //            updatedCount = await _context.SaveChangesAsync();
        //            await transactions.CommitAsync();
        //        }

        //        return new ResponseData { Message = $"Updated {updatedCount} users out of {res.Count}", Status = true };
        //    }
        //    catch (Exception _)
        //    {
        //        await transactions.RollbackAsync();
        //        return new ResponseData { Message = $"Updated {updatedCount} users out of {res.Count}", Status = false };
        //    }
        //}

        //private async Task UpdateNftVerify()
        //{
        //    var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var wallets = await _context.UserWallets
        //        .Where(_ => _.WalletId != null)
        //        .IgnoreAutoIncludes()
        //        .AsSingleQuery()
        //        .ToListAsync();

        //        foreach (var item in wallets)
        //        {
        //            var nfts = await _context.UserNfts
        //                .Where(_ => _.UserWalletId == item.Id)
        //                .IgnoreAutoIncludes()
        //                .AsSingleQuery()
        //                .ToListAsync();

        //            foreach (var item1 in nfts)
        //            {
        //                item1.Verified = 1;
        //            }

        //            await _context.SaveChangesAsync();

        //            //var sql = "UPDATE user_nfts SET Verified = @newValue WHERE UserWalletId = @id";
        //            //var newValueParameter = new SqlParameter("@newValue", 1);
        //            //var newValueParameter2 = new SqlParameter("@id", item.Id);

        //            //await _context.Database.ExecuteSqlRawAsync(sql, newValueParameter, newValueParameter2);
        //            //await transaction.CommitAsync();
        //        }

        //        await transaction.CommitAsync();
        //    }
        //    catch (Exception _)
        //    {
        //        await transaction.RollbackAsync();
        //    }

        //}

        //internal async Task AdjustUserProfileViews()
        //{
        //    var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var stats = await _context.UserStats.ToListAsync();

        //        if (stats != null && stats.Count > 0)
        //        {
        //            foreach (var item in stats)
        //            {
        //                var views = await _context.UserProfileViews.Where(_ => _.UserId == item.UserId).CountAsync();

        //                item.Views = views;
        //            }

        //            await _context.SaveChangesAsync();

        //            await transaction.CommitAsync();
        //        }
        //    }
        //    catch (Exception _)
        //    {
        //        await transaction.RollbackAsync();
        //    }

        //}
    }
}
