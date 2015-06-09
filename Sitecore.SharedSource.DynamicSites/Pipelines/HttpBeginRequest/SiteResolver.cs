using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Sites;
using Sitecore.Web;
using System;
using System.IO;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.SharedSource.DynamicSites.Pipelines.HttpBeginRequest
{

    [UsedImplicitly]
    public class SiteResolver : HttpRequestProcessor
    {
        /// <summary>
        /// Runs the processor.
        /// 
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            SiteContext site = ResolveSiteContext(args);
            UpdatePaths(args, site);
            Context.Site = site;
        }

        /// <summary>
        /// Gets the file path.
        /// 
        /// </summary>
        /// <param name="args">The args.</param><param name="context">The context.</param>
        /// <returns/>
        protected string GetFilePath(HttpRequestArgs args, SiteContext context)
        {
            return GetPath(context.PhysicalFolder, args.Url.FilePath, context);
        }

        /// <summary>
        /// Gets the item path.
        /// 
        /// </summary>
        /// <param name="args">The args.</param><param name="context">The context.</param>
        /// <returns/>
        protected string GetItemPath(HttpRequestArgs args, SiteContext context)
        {
            return GetPath(context.StartPath, args.Url.ItemPath, context);
        }

        /// <summary>
        /// Gets the path.
        /// 
        /// </summary>
        /// <param name="basePath">The base path.</param><param name="path">The path.</param><param name="context">The context.</param>
        /// <returns/>
        protected string GetPath(string basePath, string path, SiteContext context)
        {
            string virtualFolder = context.VirtualFolder;
            if (virtualFolder.Length > 0 && virtualFolder != "/")
            {
                string str = StringUtil.EnsurePostfix('/', virtualFolder);
                if (StringUtil.EnsurePostfix('/', path).StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
                    path = StringUtil.Mid(path, str.Length);
            }
            if (basePath.Length > 0 && basePath != "/")
                path = FileUtil.MakePath(basePath, path, '/');
            if (path.Length > 0 && path[0] != 47)
                path = '/' + path;
            return path;
        }

        /// <summary>
        /// Resolves the site context.
        /// 
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns/>
        protected virtual SiteContext ResolveSiteContext(HttpRequestArgs args)
        {
            string queryString = WebUtil.GetQueryString("sc_site");
            if (queryString.Length > 0)
            {
                SiteContext siteContext = SiteContextFactory.GetSiteContext(queryString);
                Assert.IsNotNull(siteContext, "Site from query string was not found: " + queryString);
                return siteContext;
            }
            if (Settings.EnableSiteConfigFiles)
            {
                string str = FileUtil.MakePath(FileUtil.NormalizeWebPath(StringUtil.GetString(new string[1]
                    {
                      Path.GetDirectoryName(args.Url.FilePath)
                    })), "site.config");
                if (FileUtil.Exists(str))
                {
                    SiteContext siteContextFromFile = SiteContextFactory.GetSiteContextFromFile(str);
                    Assert.IsNotNull(siteContextFromFile, "Site from site.config was not found: " + str);
                    return siteContextFromFile;
                }
            }
            Uri requestUri = WebUtil.GetRequestUri();
            var sites = SiteContextFactory.Sites;
            SiteContext siteContext1 = SiteContextFactory.GetSiteContext(requestUri.Host, args.Url.FilePath, requestUri.Port);
            Assert.IsNotNull(siteContext1, "Site from host name and path was not found. Host: " + requestUri.Host + ", path: " + args.Url.FilePath);
            return siteContext1;
        }

        /// <summary>
        /// Updates the paths.
        /// 
        /// </summary>
        /// <param name="args">The args.</param><param name="site">The site.</param>
        protected virtual void UpdatePaths(HttpRequestArgs args, SiteContext site)
        {
            if (!string.IsNullOrEmpty(args.Context.Request.PathInfo))
            {
                string filePath = args.Url.FilePath;
                int length = filePath.LastIndexOf('.');
                int num = filePath.LastIndexOf('/');
                args.Url.ItemPath = length >= 0 ? (length >= num ? filePath.Substring(0, length) : filePath) : filePath;
            }
            args.StartPath = site.StartPath;
            args.Url.ItemPath = GetItemPath(args, site);
            site.Request.ItemPath = args.Url.ItemPath;
            args.Url.FilePath = GetFilePath(args, site);
            site.Request.FilePath = args.Url.FilePath;
        }
    }
}
