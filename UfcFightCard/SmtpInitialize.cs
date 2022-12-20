﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UfcFightCard.Models;
using UfcFightCard.Misc;

namespace UfcFightCard
{
    public static class SmtpInitialize
    {
        public static void SendEmail(Emaildetails emaildetails, string html, DateTime cardTimeStamp)
        {
            NullChecker.Null(emaildetails.Email);
            NullChecker.Null(emaildetails.ToEmail);
            var Client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = emaildetails.Email,
                    Password = emaildetails.Password
                }
            };

            var FromEmail = new MailAddress(emaildetails.Email, emaildetails.Name);
            var ToEmail = new MailAddress(emaildetails.ToEmail, emaildetails.ToName);
            var Message = new MailMessage
            {
                From = FromEmail,
                Subject = $"Ufc card {cardTimeStamp.ToString("D")}",
                Body = html,
                IsBodyHtml = true
            };
            Message.To.Add(ToEmail);

            try
            {
                Client.Send(Message);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
