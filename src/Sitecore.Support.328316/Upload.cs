namespace Sitecore.Support.Shell.Framework.Commands.Media
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;

    [Serializable]
    public class Upload : Sitecore.Shell.Framework.Commands.Media.Upload
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                NameValueCollection parameters = new NameValueCollection
                {
                    ["id"] = StringUtil.GetString(new string[] {
                        context.Parameters["id"],
                        item.ID.ToString()
                    }),
                    ["language"] = (HttpUtility.ParseQueryString(Sitecore.Context.Items["SC_REQUEST_MEASUREMENT_URL"].ToString()).Get("la")) != null ?
                    HttpUtility.ParseQueryString(Sitecore.Context.Items["SC_REQUEST_MEASUREMENT_URL"].ToString()).Get("la") : item.Language.ToString(),
                    ["version"] = item.Version.ToString(),
                    ["load"] = StringUtil.GetString(new string[] { context.Parameters["load"] }),
                    ["edit"] = StringUtil.GetString(new string[] { context.Parameters["edit"] }),
                    ["tofolder"] = StringUtil.GetString(new string[] { context.Parameters["tofolder"] }),
                    [Sitecore.Configuration.State.Client.UsesBrowserWindowsQueryParameterName] = StringUtil.GetString(new string[] {
                        context.Parameters[Sitecore.Configuration.State.Client.UsesBrowserWindowsQueryParameterName],
                        "0"
                    })
                };
                Context.ClientPage.Start(this, "Run", parameters);
            }
        }
    }
}
