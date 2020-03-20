namespace Sitecore.Support.Shell.Applications.Media.UploadMedia
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using Sitecore.Exceptions;
    using Sitecore.Globalization;
    using Sitecore.Pipelines;
    using Sitecore.Pipelines.Upload;
    using Sitecore.Shell.Web.UI;
    using Sitecore.Web;
    using Sitecore.Web.UI.XmlControls;
    using System;
    using System.Web;
    using System.Web.UI;

    public partial class UploadMediaPage2 : SecurePage
    {
        protected override void OnInit(EventArgs e)
        {
            Control child = ControlFactory.GetControl("UploadMedia");
            if (child != null)
            {
                this.Controls.Add(child);
            }
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (base.MaxRequestLengthExceeded)
            {
                HttpContext.Current.Response.Write("<html><head><script type=\"text/JavaScript\" language=\"javascript\">window.top.scForm.getTopModalDialog().frames[0].scForm.postRequest(\"\", \"\", \"\", 'ShowFileTooBig()')</script></head><body>Done</body></html>");
            }
            else if (!base.IsEvent && (base.Request.Files.Count > 0))
            {
                try
                {
                    string pathOrId = string.Empty;
                    Language contentLanguage = Language.Parse(WebUtil.GetQueryString("la"));
                    string itemUri = Sitecore.Context.ClientPage.ClientRequest.Form["ItemUri"];
                    ItemUri uri = ItemUri.Parse(itemUri);
                    if (uri != null)
                    {
                        pathOrId = uri.GetPathOrId();
                        contentLanguage = uri.Language;
                    }
                    else
                    {
                        SecurityException exception = new SecurityException("Upload ItemUri invalid");
                        Log.Error("ItemUri not valid. ItemUri: " + itemUri, exception, this);
                        throw exception;
                    }
                    UploadArgs args = new UploadArgs
                    {
                        FileOnly = false,
                        Files = base.Request.Files,
                        Folder = pathOrId,
                        Overwrite = Settings.Upload.SimpleUploadOverwriting,
                        Unpack = false,
                        Versioned = Settings.Media.UploadAsVersionableByDefault,
                        Language = contentLanguage,
                        CloseDialogOnEnd = false,
                        Destination = Settings.Media.UploadAsFiles ? UploadDestination.File : UploadDestination.Database
                    };
                    PipelineFactory.GetPipeline("uiUpload").Start(args);

                    if (args.UploadedItems.Count > 0)
                    {
                        pathOrId = args.UploadedItems[0].ID.ToString();
                        Log.Audit(this, "Upload: {0}", new string[] { StringUtil.Join(args.UploadedItems, ", ", "Name") });
                    }
                    else
                    {
                        pathOrId = string.Empty;
                    }
                    if (string.IsNullOrEmpty(args.ErrorText))
                    {
                        HttpContext.Current.Response.Write("<html><head><script type=\"text/JavaScript\" language=\"javascript\">window.top.scForm.getTopModalDialog().frames[0].scForm.postRequest(\"\", \"\", \"\", 'EndUploading(\"" + pathOrId + "\")')</script></head><body>Done</body></html>");
                    }
                }
                catch (OutOfMemoryException)
                {
                    HttpContext.Current.Response.Write("<html><head><script type=\"text/JavaScript\" language=\"javascript\">window.top.scForm.getTopModalDialog().frames[0].scForm.postRequest(\"\", \"\", \"\", 'ShowFileTooBig(" + StringUtil.EscapeJavascriptString(base.Request.Files[0].FileName) + ")')</script></head><body>Done</body></html>");
                }
                catch (Exception exception2)
                {
                    if (exception2.InnerException is OutOfMemoryException)
                    {
                        HttpContext.Current.Response.Write("<html><head><script type=\"text/JavaScript\" language=\"javascript\">window.top.scForm.getTopModalDialog().frames[0].scForm.postRequest(\"\", \"\", \"\", 'ShowFileTooBig(" + StringUtil.EscapeJavascriptString(base.Request.Files[0].FileName) + ")')</script></head><body>Done</body></html>");
                    }
                    else
                    {
                        HttpContext.Current.Response.Write("<html><head><script type=\"text/JavaScript\" language=\"javascript\">window.top.scForm.getTopModalDialog().frames[0].scForm.postRequest(\"\", \"\", \"\", 'ShowError')</script></head><body>Done</body></html>");
                    }
                }
            }
        }
    }
}
