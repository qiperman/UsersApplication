using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace UsersApplication.Models
{
    public class UsersApplicationManager: UserManager<MyUser>
    {
        public UsersApplicationManager(IUserStore<MyUser> store)  : base(store) 
        {

        }

        public static UsersApplicationManager Create(IdentityFactoryOptions<UsersApplicationManager> options, IOwinContext context) 
        {
            AppContext db = context.Get<AppContext>();
            UsersApplicationManager manager = new UsersApplicationManager(new UserStore<MyUser>(db));
            manager.EmailService = new EmailService();
            manager.UserValidator = new UserValidator<MyUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true 
            };
            return manager;
        }
    }
}

public class EmailService : IIdentityMessageService
{
    public Task SendAsync(IdentityMessage message)
    {
        // настройка логина, пароля отправителя
        var from = "iw1v4n@yandex.ru";
        var pass = "654321Qwerty";

        // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
        SmtpClient client = new SmtpClient("smtp.yandex.ru", 25);

        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Credentials = new System.Net.NetworkCredential(from, pass);
        client.EnableSsl = true;

        // создаем письмо: message.Destination - адрес получателя
        var mail = new MailMessage(from, message.Destination);
        mail.Subject = message.Subject;
        mail.Body = message.Body;
        mail.IsBodyHtml = true;

        return client.SendMailAsync(mail);
    }
}