﻿using HtmlAgilityPack;
using ScrapySharp.Extensions;
using UfcFightCard.Constants;
using static UfcFightCard.Helpers.NumberHelper;
using static UfcFightCard.Helpers.DateTimeHelper;

namespace UfcFightCard.Helpers
{
    public static class HtmlNodeHelpers
    {

        // dictionary containing all possible method calls via HtmlParseAction
        private static Dictionary<string, Func<HtmlNode, string>> htmlNodeHelperMethods = new Dictionary<string, Func<HtmlNode, string>>()
        {
            { "LeftImage", HtmlNodeHelpers.LeftImage },
            { "RightImage", HtmlNodeHelpers.RightImage },
            { "LeftName", HtmlNodeHelpers.LeftName },
            { "RightName", HtmlNodeHelpers.RightName },
            { "WeightClass", HtmlNodeHelpers.WeightClass },
            { "CountryLeft", HtmlNodeHelpers.CountryLeft },
            { "CountryRight", HtmlNodeHelpers.CountryRight },
            { "LeftCountryImage", HtmlNodeHelpers.LeftCountryImage },
            { "RightCountryImage", HtmlNodeHelpers.RightCountryImage },
            { "LeftRank", HtmlNodeHelpers.LeftRank },
            { "RightRank", HtmlNodeHelpers.RightRank },
        };

        public static string HtmlParseAction(this HtmlNode htmlNode, string actionName)
        {
            string actionReturnString;
            if (htmlNodeHelperMethods.TryGetValue(actionName, out Func<HtmlNode, string> helperMethodToCall))
            {
                try
                {
                    actionReturnString = helperMethodToCall(htmlNode);
                }
                catch
                {
                    actionReturnString = $"<!-- Content not found for action {actionName} -->";
                }
            }
            else
            {
                throw new ArgumentException($"Method '{actionName}' not found");
            }
            return actionReturnString;
        }

        public static string LeftImage(this HtmlNode htmlNode)
        {
            return htmlNode.CssSelect(Tags.AthleteImage).First().Attributes[Tags.Src].Value.Trim();
        }
        public static string RightImage(this HtmlNode htmlNode)
        {
            return htmlNode.CssSelect(Tags.AthleteImage).ToList()[1].Attributes[Tags.Src].Value.Trim();
        }
		public static string LeftCountryImage(this HtmlNode htmlNode)
		{
            return $"{htmlNode.CssSelect(Tags.AthleteCountryLeft).ToList()[0].CssSelect(Tags.Img).First().Attributes[Tags.Src].Value.Trim()}";
		}
		public static string RightCountryImage(this HtmlNode htmlNode)
		{
			return $"{htmlNode.CssSelect(Tags.AthleteCountryRight).ToList()[0].CssSelect(Tags.Img).First().Attributes[Tags.Src].Value.Trim()}";
		}
		public static string LeftName(this HtmlNode htmlNode)
        {
            try
            {
                var givenName = htmlNode.CssSelect(Tags.FullNameRed).ToList()[0].CssSelect(Tags.GivenName).ToList()[0].InnerHtml;
                var familyName = htmlNode.CssSelect(Tags.FullNameRed).ToList()[0].CssSelect(Tags.FamilyName).ToList()[0].InnerHtml;
                return $"{givenName.Trim()} {familyName.Trim()}";
            }
            catch
            {
                var name = htmlNode.CssSelect(Tags.FullNameRed).ToList()[0].InnerHtml;
                return $"{name.Trim()}";
            }
        }
        public static string RightName(this HtmlNode htmlNode)
        {
            try
            {
                var givenName = htmlNode.CssSelect(Tags.FullNameBlue).ToList()[0].CssSelect(Tags.GivenName).ToList()[0].InnerHtml;
                var familyName = htmlNode.CssSelect(Tags.FullNameBlue).ToList()[0].CssSelect(Tags.FamilyName).ToList()[0].InnerHtml;
                return $"{givenName.Trim()} {familyName.Trim()}";
            }
            catch
            {
                var name = htmlNode.CssSelect(Tags.FullNameBlue).ToList()[0].InnerHtml;
                return $"{name.Trim()}";
            }
        }
        public static string WeightClass(this HtmlNode htmlNode)
        {

            return htmlNode.CssSelect(Tags.WeightClass).First().InnerHtml.Trim();

        }
		public static string CountryLeft(this HtmlNode htmlNode)
		{
		    return htmlNode.CssSelect(Tags.CountryText).ToList()[0].InnerHtml.Trim();
		}
		public static string CountryRight(this HtmlNode htmlNode)
		{

			return htmlNode.CssSelect(Tags.CountryText).ToList()[1].InnerHtml.Trim();

		}
		public static List<HtmlNode> Fights(List<HtmlNode> htmlNode)
        {
            return htmlNode[0].Descendants(Tags.Li)
                .Where(node => node.GetAttributeValue(Tags.Class, "").Contains(Tags.Rows))
                .ToList();
        }
        public static List<HtmlNode> UnorderedList(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode.Descendants(Tags.Ul)
                .Where(node => node.GetAttributeValue(Tags.Class, "").Contains(Tags.Fights))
                .ToList();
        }
        public static List<HtmlNode> LatestCard(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode.Descendants(Tags.Div)
                .Where(node => node.GetAttributeValue(Tags.Class, "").Contains(Tags.CardLogo))
                .ToList();
        }
        public static DateTime CardTimeStamp(HtmlDocument htmlDoc)
        {
            var doc = htmlDoc.DocumentNode.Descendants(Tags.Div)
                .Where(node => node.GetAttributeValue(Tags.Class, "").Contains(Tags.CardInfo))
                .ToList();

            var timeStamp = GetMainCardTimeStamp(doc);
            return timeStamp;
        }
        public static DateTime GetMainCardTimeStamp(List<HtmlNode> htmlNode)
        {
            var timeStamp = htmlNode[0].SelectSingleNode(Tags.Div).GetAttributeValue(Tags.TimeStamp, "");
            var jsConvertedTimeStamp = JsTimeConverter(timeStamp);
            return jsConvertedTimeStamp; ;
        }
        public static string MainCardUrl(List<HtmlNode> htmlNode)
        {
            return htmlNode[0].SelectSingleNode(Tags.A).GetAttributeValue(Tags.Href, "").Trim();
        }
		public static string LeftRank(this HtmlNode htmlNode)
		{
            try
            {
				return htmlNode.CssSelect(Tags.Rank).First().CssSelect(Tags.Span).ToList().First().InnerHtml.Trim();
			}
            catch
            {
                return "#N/a";
            }
		}
		public static string RightRank(this HtmlNode htmlNode)
		{
            try
            {
				return htmlNode.CssSelect(Tags.Rank).ToList()[1].CssSelect(Tags.Span).ToList().First().InnerHtml.Trim();
			}
			catch
			{
				return "#N/a";
			}
		}
	}
}
