﻿using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using UfcFightCard;
using UfcFightCard.Constants;
using UfcFightCard.Misc;
using UfcFightCard.Models;
using System.Web;
using System.Reflection;

var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
               .Build();

var ufcCardDetails = UfcEventsScraper.GetLatestUfcCardDetails();

NullChecker.Null(ufcCardDetails.LatestCardUrl);
var fightCardContent = UfcEventsScraper.GetUfcMainCardContent(ufcCardDetails.LatestCardUrl);
var html = await RazorTemplateEngine.RenderAsync(Url.razor, fightCardContent);
var emaildetails = config.GetRequiredSection("Emaildetails").Get<Emaildetails>();
if (Conditionals.ShouldSendEmail(ufcCardDetails, emaildetails)) 
{ 
    SmtpInitialize.SendEmail(emaildetails, HttpUtility.HtmlDecode(html), ufcCardDetails.MainCardTimeStamp.GetValueOrDefault(DateTime.Now));
}