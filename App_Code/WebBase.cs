using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

public static class References
{
    public const string ProjectTitle = "SkyLite";

    public static class Htmls
    {
        public const string Email_SignUp = "Email_SignUp";
        public const string Email_PassReset = "Email_PassReset";
        public const string Email_PassChanged = "Email_PassChanged";
        public const string Email_ResetDevice = "Email_ResetDevice";
    }
    public static class Keys
    {
        public const string SignUp_User = WebParams.GlobalPrefix + ProjectTitle + ".TempUser";
        public const string AppKey = WebParams.GlobalPrefix + ProjectTitle + ".AppKey";
        public const string PageKey = WebParams.GlobalPrefix + ProjectTitle + ".Page.{0}";
    }
    public static class Elements
    {
        public const string PageContents = "pagecontents";
        public const string ContentWrap = "contentwrap";
        public const string ElmBox = "elmBox";
    }
    public static class Pages
    {
        public const string XysSignupExpired = "XysSignupExpired";
        public const string XysUnAuthorized = "XysUnAuthorized";
        public const string XysAuth = "XysAuth";
        public const string XysVerify = "XysVerify";
        public const string XysSignup = "XysSignup";
        public const string XysSignin = "XysSignin";
        public const string XysSent = "XysSent";
        public const string XysPassReset = "XysPassReset";
        public const string XysPassChange = "XysPassChange";
        public const string XysPermission = "XysPermission";
        public const string XysPass = "XysPass";
        public const string XysRole = "XysRole";
        public const string XysRoleMV = "XysRoleMV";
        public const string XysRoleEV = "XysRoleEV";
        public const string XysUser = "XysUser";
        public const string XysUserMV = "XysUserMV";
        public const string XysUserEV = "XysUserEV";
        public const string XysHome = "XysHome";
        public const string XysSettings = "XysSettings";
        public const string XysPage = "XysPage";
        public const string XysPageEV = "XysPageEV";
        public const string XysPageMV = "XysPageMV";
        public const string XysLevel = "XysLevel";
        public const string XysLevelEV = "XysLevelEV";
        public const string XysLevelMV = "XysLevelMV";
        public const string XysFile = "XysFile";
        public const string XysFileMV = "XysFileMV";
        public const string XysMenu = "XysMenu";
        public const string XysMenuMV = "XysMenuMV";
        public const string XysMenuEV = "XysMenuEV";
        public const string XysProfile = "XysProfile";
        public const string XysProPass = "XysProPass";
        public const string XysCloseAcct = "XysCloseAcct";
        public const string XysReport = "XysReport";
        public const string XysBulletin = "XysBulletin";
        public const string XysBulletinEV = "XysBulletinEV";
        public const string XysBulletinMV = "XysBulletinMV";
        public const string XysDict = "XysDict";
        public const string XysDictMV = "XysDictMV";
        public const string XysDictEV = "XysDictEV";
        public const string XysLang = "XysLang";
        public const string XysLangMV = "XysLangMV";
        public const string XysLangEV = "XysLangEV";
        public const string XysOption = "XysOption";
        public const string XysOptionMV = "XysOptionMV";
        public const string XysOptionEV = "XysOptionEV";
        public const string Support = "Support";
        public const string SupportMV = "SupportMV";
    }
}

public class AppKey
{
    public string UserId = string.Empty;
    public string UserName = string.Empty;
    public string UserEmail = string.Empty;
    public string UserPhone = string.Empty;
    public string UserPic = string.Empty;
    public string RoleId = string.Empty;
    public string UserRef = string.Empty;
    public DateTime DateTime;
}

public class WebBase : WebPage
{
    public ViewPart ViewPart = null;
    public List<NameFlag> ViewFields = new List<NameFlag>();
    public AppKey AppKey = null;
    public string DateTimeFormat;
    public string DateFormat;

    public WebBase()
        : base()
    {
        DateTimeFormat = ClientCulture.DateTimeFormat.ShortDatePattern + " " + ClientCulture.DateTimeFormat.ShortTimePattern;
        DateFormat = ClientCulture.DateTimeFormat.ShortDatePattern;

        try
        {
            string paramVlu = ParamValue(References.Keys.AppKey, true);
            AppKey = (AppKey)DeserializeObjectEnc(paramVlu, typeof(AppKey));
        }
        catch (Exception)
        {
            AppKey = null;
        }

        HtmlTranslator.Add(GetPageDict(this.GetType().ToString()));
    }

    public override void OnResponse(ref ApiResponse _ApiResponse)
    {
        base.OnResponse(ref _ApiResponse);
    }

    public override ApiResponse OnRequest(string Method = "")
    {
        string SerializedViewPart = ParamValue(References.Keys.PageKey.Replace("{0}", this.GetType().ToString()));
        if (SerializedViewPart != string.Empty)
        {
            ViewPart = (ViewPart)DeserializeObjectEnc(SerializedViewPart, typeof(ViewPart));
        }

        if (ViewPart != null)
        {
            for (int i = 0; i < ViewFields.Count; i++)
            {
                if (ViewFields[i].flag == false)
                {
                    NameValue fld = ViewPart.Field(ViewFields[i].name);
                    if (fld != null)
                    {
                        fld.value = ParamValue(ViewFields[i].name);
                    }
                }
            }
        }

        ApiResponse _ApiResponse = null;
        if (IsMethodName(Method) == false)
        {
            _ApiResponse = new ApiResponse();
            var dialogBox = new DialogBox(Translator.Format("accessdenied"));
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        return _ApiResponse;
    }

    public override void OnInitialized()
    {
        ViewPart = InitialModel();

        string SerializedViewPart = SerializeObjectEnc(ViewPart, typeof(ViewPart));
        HtmlDoc.InitialScripts.StoreLocalValue(References.Keys.PageKey.Replace("{0}", this.GetType().ToString()), SerializedViewPart);
        if (AppKey != null)
        {
            HtmlDoc.InitialScripts.SetCookie(References.Keys.AppKey, SerializeObjectEnc(AppKey, typeof(AppKey)));
        }

        HtmlDoc.AddJsFile("WebScript.js");
        HtmlDoc.AddCSSFile("WebStyle.css");
        HtmlDoc.SetTitle(Translator.Format("title"));

        bool _ViewAccess = ViewAccess();
        HtmlDoc.HtmlBodyAddOn = _ViewAccess ? InitialView() : PartialPage(References.Pages.XysUnAuthorized);
    }

    protected internal bool IsMethodName(string MethodName)
    {
        bool rtnvlu = false;
        if (ViewPart == null || ViewPart.Methods == null) return true;
        if (ViewPart.Methods.Find(x => x.Method.ToLower().EndsWith(MethodName.ToLower())) == null) return true;

        for (int i = 0; i < ViewPart.Methods.Count; i++)
        {
            string compMethod = !ViewPart.Methods[i].Method.Contains("/") ? MethodName : this.GetType().ToString() + "/" + MethodName;
            if (ViewPart.Methods[i].Method.ToLower().Trim() == compMethod.ToLower().Trim())
            {
                rtnvlu = true;
            }
        }
        return rtnvlu;
    }

    private bool ViewAccess()
    {
        if (AppKey == null) return false;
        string SSQL = " declare @roleid nvarchar(50),@pagename nvarchar(100) " +
                      " set @roleid = N'" + AppKey.RoleId + "' " +
                      " set @pagename = N'" + this.GetType().ToString() + "' " +
                      " select count(*) from XysPage where PageUse=1 and dbo.XF_RolePage(@roleid,PageId) = 1 and PageName = @pagename ";

        string tcnt = SQLData.SQLFieldValue(SSQL);
        return ValC(tcnt) == 0 ? false : true;
    }

    protected internal List<ViewMethod> ViewMethods(string PageType = "")
    {
        PageType = (PageType == string.Empty) ? this.GetType().ToString() : PageType;
        if (AppKey == null) return null;
        string SSQL = " declare @roleid nvarchar(50),@pagename nvarchar(100) " +
                      " set @roleid = N'" + AppKey.RoleId + "' " +
                      " set @pagename = N'" + PageType + "' " +
                      " select MenuArea Area,MenuTag Tag,MenuMethod Method,MenuParams Params,MenuCtl Ctl,MenuType CtlType,dbo.XF_RoleMenu(@roleid,MenuId) Allowed" +
                      " from XysMenu where MenuUse=1 and PageId = dbo.XF_PageIdByName(@pagename) " +
                      "  order by area,MenuOrder ";

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SSQL, ref emsg);
        List<ViewMethod> data = DataTableListT<ViewMethod>(SQLData.SQLDataTable(SSQL, ref emsg));
        return data;
    }

    public virtual ViewPart InitialModel()
    {
        var _ViewPart = new ViewPart();
        _ViewPart.Methods = ViewMethods();
        return _ViewPart;
    }

    public virtual string InitialView()
    {
        return string.Empty;
    }

    public override void OnAfterRender()
    {
        base.OnAfterRender();
    }

    protected internal List<Translator.Dictionary> GetPageDict(string pagename)
    {
        var rlt = new List<Translator.Dictionary>();
        string SSQL =
            " declare @pageid nvarchar(50),@isocode nvarchar(10)  " +
            " set @pageid = N'" + pagename + "' " +
            " set @isocode = N'" + ClientLanguage + "' ";

        if (ClientLanguage.Contains("-"))
        {
            SSQL += " if exists(select * from XysDict where Isocode = @isocode) " +
                    " begin " +
                    "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                    "  Where (Target = @pageid or Target = '*') " +
                    "          and (Isocode = '*' or Isocode = @isocode ) " +
                    "  order by KeyWord  " +
                    " end " +
                    " else " +
                    " begin " +
                    "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                    "  Where (Target = @pageid or Target = '*') " +
                    "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                    "  order by KeyWord  " +
                    " end ";
        }
        else
        {
            SSQL += " if exists(select * from XysDict where left(Isocode,2) = @isocode) " +
                    " begin " +
                    "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                    "  Where (Target = @pageid or Target = '*') " +
                    "          and (Isocode = '*' or Isocode = @isocode ) " +
                    "  order by KeyWord  " +
                    " end " +
                    " else " +
                    " begin " +
                    "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                    "  Where (Target = @pageid or Target = '*') " +
                    "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                    "  order by KeyWord  " +
                    " end ";
        }

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SSQL, ref emsg);
        if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var _Dict = new Translator.Dictionary();
                _Dict.IsoCode = dt.Rows[i][1].ToString();
                _Dict.DicKey = dt.Rows[i][2].ToString();
                _Dict.DicWord = dt.Rows[i][3].ToString();
                rlt.Add(_Dict);
            }
        }
        return rlt;
    }

    public string PutData(List<string> SQL)
    {
        string emsg = string.Empty;
        SQLData.SQLDataPut(SQL, ref emsg);
        WriteXysLog(string.Join("|", SQL), ref emsg);
        return emsg;
    }

    public void WriteXysLog(string logTxt, ref string msg)
    {
        string userid = (AppKey != null) ? AppKey.UserId : string.Empty;

        List<string> SQL = new List<string>()
        {
            "insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) " +
            "values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())"
        };

        var SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@LogId", Value = NewID(), SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@ClientIp", Value = ClientIPAddress, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = userid, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@LogTxt", Value = EscQuote(logTxt.Trim()), SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@JobRlt", Value = EscQuote(msg.Trim()), SqlDbType = SqlDbType.NVarChar });

        string emsg = string.Empty;
        var _SqlData = new SQLData();
        _SqlData.DataPut(SqlWithParams(SQL, SqlParams), ref emsg);
    }

    public string SendEmail(string Subject, string bodyHtml, string[] ToAddr)
    {
        var mail = new Mail();
        mail.Subject = Subject;
        mail.ToAddr = ToAddr;
        mail.Body = bodyHtml;
        string rlt = mail.SendMail();
        return rlt;
    }

    protected internal TitleSection2 PageTitle(bool showTimer = true)
    {
        var tmr = new Timer { InnerText = "00:00" };
        tmr.SetAttribute(HtmlAttributes.id, "tmr");
        tmr.SetStyle(HtmlStyles.top, "38px");
        tmr.SetStyle(HtmlStyles.right, "90px");

        string imgfile = PhysicalFolder + "photos\\" + AppKey.UserId + ".jpg";
        if (!File.Exists(imgfile))
        {
            imgfile = ImagePath + "img_fakeuser.jpg";
        }
        else
        {
            imgfile = GetPhotoData(imgfile);
        }

        var tSector = new TitleSection2();
        tSector.Title.Caption.InnerText = WebAppName;
        tSector.Title.Page.InnerText = Translator.Format("title");
        if (showTimer) tSector.ContentExtra.InnerText = tmr.HtmlText;
        tSector.Title.LogoImage.SetAttribute(HtmlEvents.onclick, "$navi('" + References.Pages.XysHome + "')");
        tSector.UserIcon.Icon.SetAttribute(HtmlAttributes.src, imgfile);
        tSector.UserIcon.Menu.AddItem(Translator.Format("profile"), string.Empty, "onclick:Profile();");
        tSector.UserIcon.Menu.AddItem(Translator.Format("signout"), string.Empty, "onclick:SignOut();");
        tSector.FooterSection.AddMenu(Translator.Format("support"), string.Empty, "onclick:" + ByPassCall("NaviClick", "m=support"));
        return tSector;
    }

    protected MenuList SetPageMenu(string[] selectedTags)
    {
        var menulist = new MenuList();
        menulist.Wrap.SetStyle(HtmlStyles.@float, "right");

        List<ViewMethod> Methods = ViewPart.Methods.FindAll(x => x.Area == "M" && x.Allowed == 1).OrderBy(x => x.Sort).ToList();
        if (Methods != null)
        {
            for (int i = 0; i < Methods.Count; i++)
            {
                if (selectedTags.Length == 0)
                {
                    string mnuctl = RollbackQuote(Methods[i].Ctl).Replace("{params}", string.Empty);
                    string mnutyp = Methods[i].CtlType;
                    dynamic mnuobj = DeserializeObject(mnuctl, AssemblyType(mnutyp));
                    for (int j = 0; j < mnuobj.Attributes.Count; j++)
                    {
                        mnuobj.Attributes[j].value = mnuobj.Attributes[j].value.Replace("%partialdata%", PartialData);
                    }
                    menulist.Add(mnuobj);
                }
                else
                {
                    string Tags = string.Join(",", selectedTags);
                    if (Tags.ToLower().Contains(Methods[i].Tag.ToLower()))
                    {
                        string mnuctl = RollbackQuote(Methods[i].Ctl).Replace("{params}", string.Empty);
                        string mnutyp = Methods[i].CtlType;
                        dynamic mnuobj = DeserializeObject(mnuctl, AssemblyType(mnutyp));
                        for (int j = 0; j < mnuobj.Attributes.Count; j++)
                        {
                            mnuobj.Attributes[j].value = mnuobj.Attributes[j].value.Replace("%partialdata%", PartialData);
                        }
                        menulist.Add(mnuobj);
                    }
                }
            }
        }
        return menulist;
    }

    protected Wrap SetPageButtons(string[] selectedTags)
    {
        var BtnWrap = new Wrap();
        BtnWrap.SetStyle(HtmlStyles.display, "flex");
        BtnWrap.SetStyle(HtmlStyles.justifyContent, "flex-start");

        string BtnHtml = string.Empty;
        List<ViewMethod> Methods = ViewPart.Methods.FindAll(x => x.Area == "B" && x.Allowed == 1).OrderBy(x => x.Sort).ToList();
        if (Methods != null)
        {
            for (int i = 0; i < Methods.Count; i++)
            {
                if (selectedTags.Length == 0)
                {
                    string mnuctl = RollbackQuote(Methods[i].Ctl).Replace("{params}", string.Empty);
                    string mnutyp = Methods[i].CtlType;
                    dynamic mnuobj = DeserializeObject(mnuctl, AssemblyType(mnutyp));
                    BtnHtml += mnuobj.HtmlText;
                }
                else
                {
                    string Tags = string.Join(",", selectedTags);
                    if (Tags.ToLower().Contains(Methods[i].Tag.ToLower()))
                    {
                        string mnuctl = RollbackQuote(Methods[i].Ctl).Replace("{params}", string.Empty);
                        string mnutyp = Methods[i].CtlType;
                        dynamic mnuobj = DeserializeObject(mnuctl, AssemblyType(mnutyp));
                        BtnHtml += mnuobj.HtmlText;
                    }
                }
            }
        }
        BtnWrap.InnerText = BtnHtml;
        return BtnWrap;
    }

    #region Attached File Handler

    protected string UploadFile(string fileKey, string refId)
    {
        string rlt = string.Empty;
        if (HttpContext.Current.Request.Files.Count == 0) return rlt;
        if (ViewPart.Mode == ViewMode.Edit) DeleteFiles(refId);

        string emsg = string.Empty;
        List<string> SQL = new List<string>();

        HttpFileCollection hfc = HttpContext.Current.Request.Files;
        SQL.Add(" delete from XysFile where FileRefId = N'" + refId + "' ");
        for (int i = 0; i < hfc.Count; i++)
        {
            if (hfc.Keys[i] == fileKey)
            {
                string flId = NewID(1);
                string flRef = this.GetType().ToString();
                string flRefid = refId;
                string fltype = hfc[i].ContentType;
                string flname = hfc[i].FileName.Split('\\').Last();
                string flname2 = NewID(0, false) + (hfc[i].FileName.Split('.').Last() == string.Empty ? string.Empty : "." + hfc[i].FileName.Split('.').Last());
                string flFolder = DataFolder + flname2;
                string flPath = DataPath + flname2;
                hfc[i].SaveAs(flFolder);

                string ssql = " Insert XysFile(FileId,FileRef,FileRefId,FileType,FileName,FileLink,FilePath,SYSDTE,SYSUSR) " +
                                " values(N'" + flId + "',N'" + flRef + "',N'" + flRefid + "',N'" + fltype + "',N'" + flname + "', " +
                                "        N'" + flPath + "',N'" + flFolder + "'," +
                                "          Getdate(),N'" + AppKey.UserId + "')";
                SQL.Add(ssql);
            }
        }

        return PutData(SQL);
    }

    private void DeleteFiles(string refId)
    {
        string ssql = " select FilePath from XysFile where FileRefId = N'" + refId + "' ";
        string emsg = string.Empty;

        DataTable dt = SQLData.SQLDataTable(ssql, ref emsg);
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string filepath = dt.Rows[i][0].ToString();
                if (File.Exists(filepath))
                {
                    try
                    {
                        File.Delete(filepath);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }

    protected ApiResponse DeleteFile()
    {
        string fileid = GetDataValue("t");
        var _ApiResponse = new ApiResponse();
        if (fileid != string.Empty)
        {
            var dialogBox = new DialogBox(Translator.Format("wannadeletefile"));
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
            dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("RemoveFileProcess", "t=" + fileid));
            dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }
        return _ApiResponse;
    }

    protected ApiResponse DeleteFileProcess()
    {
        string fileid = GetDataValue("t");
        var _ApiResponse = new ApiResponse();
        string rlt = PutDeleteFile(fileid);
        if (rlt == string.Empty)
        {
            var dialogBox = new DialogBox(Translator.Format("successdeletefile"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button1;onclick:" + ByPassCall("Refresh"));
            _ApiResponse.PopUpWindow(dialogBox.HtmlText);
        }
        else
        {
            var dialogBox = new DialogBox(rlt);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText);
        }
        return _ApiResponse;
    }

    private string PutDeleteFile(string FileId)
    {
        string rlt = string.Empty;
        string filepath = SQLData.SQLFieldValue(" declare @FileId nvarchar(50) " +
                                                " set @FileId = N'" + FileId + "' " +
                                                " select FilePath from XysFile Where  FileId = @FileId ");
        if (filepath != string.Empty && File.Exists(filepath))
        {
            List<string> SQL = new List<string>();
            SQL.Add("delete from XysFile where FileId = N'" + FileId + "'");
            File.Delete(filepath);
            rlt = PutData(SQL);
        }
        else
        {
            if (!filepath.ToLower().Contains("sqlerror") && !File.Exists(filepath))
            {
                List<string> SQL = new List<string>();
                SQL.Add("delete from XysFile where FileId = N'" + FileId + "'");
                PutData(SQL);
            }
            rlt = Translator.Format("filenotfound");
        }
        return rlt;
    }

    protected ApiResponse FileDownLoadEnc()
    {
        string encFileId = GetDataValue("FileId");
        string decFileId = Encryptor.DecryptData(encFileId);
        var _ApiResponse = new ApiResponse();
        if (decFileId != string.Empty && decFileId.Split('|').Length >= 2)
        {
            string p1 = decFileId.Split('|')[0];
            string p2 = decFileId.Split('|')[1];

            if ((DateTime.Now - Convert.ToDateTime(p1)).TotalMinutes > 30)
            {
                var dialogBox = new DialogBox(Translator.Format("filenotfound"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText);
            }
            else
            {
                string emsg = string.Empty; 
                DataTable dt = SQLData.SQLDataTable("select FileName,FilePath from XysFile where FileId = N'" + p2 + "' ", ref emsg);
                if (dt != null)
                {
                    string filename = dt.Rows[0][0].ToString();
                    string filepath = dt.Rows[0][1].ToString();
                    string FileLink = DownLoadFileLink(filename, filepath);
                    _ApiResponse.DownloadFile(FileLink);
                }
                else
                {
                    var dialogBox = new DialogBox(Translator.Format("filenotfound"));
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText);
                }
            }
        }
        return _ApiResponse;
    }

    public ApiResponse UpdatePhoto()
    {
        string imgid = GetDataValue("f");
        string inputid = GetDataValue("s");
        string userid = DecryptString(GetDataValue("p"));
        var _ApiResponse = new ApiResponse();
        HttpFileCollection hfc = HttpContext.Current.Request.Files;

        if (hfc.Count == 0 || hfc[0].FileName.Split('.').Last() != "jpg")
        {
            var dialogBox = new DialogBox(Translator.Format("nouploadfilenojpg"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.SetElementValue(inputid, string.Empty);
            _ApiResponse.PopUpWindow(dialogBox.HtmlText);
        }
        else
        {
            string FileName = PhysicalFolder + "photos\\" + userid + ".jpg";
            try
            {
                hfc[0].SaveAs(FileName);
                string imgdata = GetPhotoData(FileName);
                _ApiResponse.SetElementAttribute(imgid, HtmlAttributes.src, imgdata);
            }
            catch (Exception ex)
            {
                var dialogBox = new DialogBox(ex.Message);
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.SetElementValue(inputid, string.Empty);
                _ApiResponse.PopUpWindow(dialogBox.HtmlText);
            }
        }
        return _ApiResponse;
    }

    public string GetPhotoData(string FileName)
    {
        string _imageData = string.Empty;
        var webClient = new System.Net.WebClient();
        try
        {
            using (Stream stream = webClient.OpenRead(FileName))
            {
                var bitmap = new Bitmap(stream);
                _imageData = ImageHandler.ImageBase64(bitmap, 300, 380);
            }
        }
        catch (Exception)
        {
            using (Stream stream = webClient.OpenRead(ImagePath + "noimage.jpg"))
            {
                var bitmap = new Bitmap(stream);
                _imageData = ImageHandler.ImageBase64(bitmap, 300, 380);
            }
        }
        return _imageData;
    }
    #endregion
}

public enum ViewMode
{
    View = 0,
    @New = 1,
    Edit = 2
}

public class ViewPart
{
    public List<NameValue> Fields = new List<NameValue>();
    public List<ViewMethod> Methods = null;
    public DataTable Data = null;
    public string Params = string.Empty;
    public ViewMode Mode = ViewMode.View;
    public UIControl UIControl = null;

    public string ColunmValue(string ColumnName)
    {
        string rtnVlu = string.Empty;
        if (Data != null && Data.Columns[ColumnName] != null && Data.Rows.Count > 0)
        {
            rtnVlu = Data.Rows[0][ColumnName].ToString();
        }
        return rtnVlu;
    }

    public NameValue Field(string Name)
    {
        return Fields.Find(x => x.name == Name);
    }
}

public class ViewMethod
{
    public string Area = string.Empty;
    public string Tag = string.Empty;
    public string Method = string.Empty;
    public string Params = string.Empty;
    public string Ctl = string.Empty;
    public string CtlType = string.Empty;
    public int Sort = 0;
    public int Allowed = 0;
}