using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EasyReading
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "BookChapters",
                url: "Book/{book_id}/Chapter/{action}/{id}",
                defaults: new { controller = "Chapter", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Alignment",
                url: "AlignChapters/{id1}/{id2}",
                defaults: new { controller = "Alignment", action = "AlignChapters" }
            );

            routes.MapRoute(
                name: "FurtherAlignment",
                url: "AlignSentences/{id1}/{id2}",
                defaults: new { controller = "Alignment", action = "AlignSentences" }
            );

            routes.MapRoute(
                name: "ChapterBinding",
                url: "Alignment/CreateChapter/{id1}/{id2}",
                defaults: new { controller = "Alignment", action = "CreateChapterBinding" }
            );

            routes.MapRoute(
                name: "SentenceBinding",
                url: "Alignment/CreateBookmark/{book1}/{book2}/{id1}/{id2}",
                defaults: new { controller = "Alignment", action = "CreateBookmarkBinding" }
            );

            routes.MapRoute(
                name: "GetBookmarks",
                url: "GetBookmarks/{book1}/{book2}",
                defaults: new { controller = "Alignment", action = "GetBookmarkBinding" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}