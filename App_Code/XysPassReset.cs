using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public class XysPassReset : WebPage
{
    private AppKey AppKey = null;

    public XysPassReset()
    {
        HtmlTranslator.Add(GetPageDict(this.GetType().ToString()));
    }

    protected internal List<Translator.@Dictionary> GetPageDict(string pagename)
    {
        List<Translator.@Dictionary> rlt = new List<Translator.@Dictionary>();
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
                   "          and (Isocode = '*' or Isocode = 'en-US' ) " +
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
                   "          and (Isocode = '*' or Isocode = 'en-US' ) " +
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

    public override void OnInitialized()
    {
        HtmlDoc.AddJsFile("WebScript.js");
        HtmlDoc.AddCSSFile("WebStyle.css");
        HtmlDoc.SetTitle(Translator.Format("title"));

        var Title = new Label(Translator.Format("reset"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        var lbl1 = new Label(Translator.Format("enteremail"));
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

        var text = new Texts(Translator.Format("email"), "email", TextTypes.text);
        text.Text.SetAttribute(HtmlAttributes.name, "email");
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "330px");

        var btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, "NavXysSent()");

        var btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
        btn1.SetStyle(HtmlStyles.marginLeft, "12px");
        btn1.SetAttribute(HtmlAttributes.@class, "button1");
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 16);
        elmBox.AddItem(lbl1, 20);
        elmBox.AddItem(text, 40);
        elmBox.AddItem(btn);
        elmBox.AddItem(btn1, 10);

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
    }

    public ApiResponse NavXysSignin()
    {
        var apiResponse = new ApiResponse();
        apiResponse.Navigate(References.Pages.XysSignin);
        return apiResponse;
    }

    public ApiResponse NavXysSent()
    {
        string email = GetDataValue("email");
        var apiResponse = new ApiResponse();

        if (!string.IsNullOrEmpty(email))
        {
            ViewData data = GetViewData(email);
            if (data != null)
            {
                string SerializedString = SerializeObjectEnc(data, typeof(ViewData));
                string UserLink = VirtualPath + "xyspasschange?x=" + SerializedString;
                string MailFile = HtmlFolder + References.Htmls.Email_PassReset + "_" + ClientLanguage + ".html";
                if (!File.Exists(MailFile))
                {
                    MailFile = References.Htmls.Email_PassReset;
                }
                else
                {
                    MailFile = References.Htmls.Email_PassReset + "_" + ClientLanguage;
                }

                string Subject = HtmlTranslator.Value("msg_reset");
                string bodyHtml = ReadHtmlFile(MailFile)
                                         .Replace("{username}", data.Name)
                                         .Replace("{useremail}", data.Email)
                                         .Replace("{userlink}", UserLink);
                string[] ToAddr = { data.Email };

                string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                if (rltmail == string.Empty)
                {
                    string rlt = SaveViewData(data);
                    if (rlt == string.Empty)
                    {
                        apiResponse.Navigate(References.Pages.XysSent);
                    }
                    else
                    {
                        var dialogBox = new DialogBox(rlt);
                        dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                        apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                        apiResponse.ExecuteScript("ShowButtons();");
                    }
                }
                else
                {
                    var dialogBox = new DialogBox(rltmail);
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                    apiResponse.ExecuteScript("ShowButtons();");
                }
            }
            else
            {
                apiResponse.Navigate(References.Pages.XysSent);
            }
        }
        else
        {
            var dialogBox = new DialogBox(Translator.Format("msg_email"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            apiResponse.PopUpWindow(dialogBox.HtmlText);
        }
        return apiResponse;
    }

    private string SaveViewData(ViewData data)
    {
        List<string> SQL = new List<string>()
        {
            " Insert into XysUserReset( Tid,Email,UserId,Status,Created,Expired) " +
            " values ( N'" + data.Tid + "',N'" + data.Email + "',N'" + data.UserId + "',0,getdate(),dateadd(minute,10,getdate())) "
        };
        return PutData(SQL);
    }

    private ViewData GetViewData(string UserEmail)
    {
        ViewData _data = null;
        string SSql = " select a.UserId,UserEmail,UserName " +
                            " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                            " where b.UserEmail = @UserEmail ";

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = SqlDbType.NVarChar });

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SqlWithParams(SSql, SqlParams), ref emsg);
        if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
        {
            _data = new ViewData();
            _data.Tid = NewID();
            _data.UserId = dt.Rows[0][0].ToString();
            _data.Email = dt.Rows[0][1].ToString();
            _data.Name = dt.Rows[0][2].ToString();
        }
        return _data;
    }

    private string PutData(List<string> SQL)
    {
        string emsg = string.Empty;
        SQLData.SQLDataPut(SQL, ref emsg);
        WriteXysLog(string.Join("|", SQL), emsg);
        return emsg;
    }

    private void WriteXysLog(string logTxt, string msg)
    {
        string userid = (AppKey != null) ? AppKey.UserId : string.Empty;
        List<string> SQL = new List<string>()
        {
            "insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) " +
            "values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())"
        };

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@LogId", Value = NewID(), SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@ClientIp", Value = ClientIPAddress, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = userid, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@LogTxt", Value = EscQuote(logTxt.Trim()), SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new SqlParameter { ParameterName = "@JobRlt", Value = EscQuote(msg.Trim()), SqlDbType = SqlDbType.NVarChar });

        string emsg = string.Empty;
        var _SqlData = new SQLData();
        _SqlData.DataPut(SqlWithParams(SQL, SqlParams), ref emsg);
    }

    private string SendEmail(string Subject, string bodyHtml, string[] ToAddr)
    {
        var mail = new Mail();
        mail.Subject = Subject;
        mail.ToAddr = ToAddr;
        mail.Body = bodyHtml;
        string rlt = mail.SendMail();
        return rlt;
    }

    public class ViewData
    {
        public string Tid = string.Empty;
        public string UserId = string.Empty;
        public string Email = string.Empty;
        public string Name = string.Empty;
        public string Pwd = string.Empty;
    }
}