using CrawlerSelenium.CrawlerSelenium;

namespace CrawlerSelenium
{
    public class RequestManager
    {
        public static string GetHtmlContent(string url)
        {
            try
            {
                DriverContext context = new DriverContext();
                context.Start();
                //context.Driver=
                context.Driver.Navigate().GoToUrl(url);
                var html = context.Driver.PageSource;
                return html;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return "";

        }

    }
}
