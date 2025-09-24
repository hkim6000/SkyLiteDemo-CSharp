using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

public class XysPassChange : WebPage
{
    private ViewData data;
    private AppKey AppKey = null;

    public XysPassChange()
    {
        try
        {
            HtmlTranslator.Add(GetPageDict(this.GetType().ToString()));
            string QryVlu = QueryValue("x");
            data = (ViewData)DeserializeObjectEnc(QryVlu, typeof(ViewData));
        }
        catch (Exception)
        {
            data = null;
        }
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
        DataTable dt = SQLData.SQLDataTable(SSQL,ref emsg);
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

    public override void OnBeforRender()
    {
        if (data == null)
        {
            HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin);
        }
        else
        {
            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
        }
    }

    public override void OnInitialized()
    {
        HtmlDoc.AddJsFile("WebScript.js");
        HtmlDoc.AddCSSFile("WebStyle.css");
        HtmlDoc.SetTitle(Translator.Format("title"));

        var Title = new Label(Translator.Format("changepwd"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        var lbl1 = new Label(Translator.Format("enterpwd"));
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

        var text = new Texts(Translator.Format("pwd"), "pwd", TextTypes.password);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "180px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "15");

        var text1 = new Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password);
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "180px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "15");

        var btn = new Button(Translator.Format("submit"), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 16);
        elmBox.AddItem(lbl1, 24);
        elmBox.AddItem(text, 4);
        elmBox.AddItem(text1, 40);
        elmBox.AddItem(btn, 10);

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
    }

    public ApiResponse NavXysSignin()
    {
        string pwd = GetDataValue("pwd");
        string pwd1 = GetDataValue("pwd1");

        var apiResponse = new ApiResponse();

        bool Valid = true;
        string DialogMsg = string.Empty;

        if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(pwd1))
        {
            Valid = false;
        }

        if (!Valid)
        {
            DialogMsg = Translator.Format("msg_required");
        }
        else
        {
            if (pwd != pwd1)
            {
                DialogMsg = Translator.Format("msg_pwdconfirm");
            }
            else
            {
                string rlt = ValidatePassword(pwd);
            }
        }

        if (DialogMsg != string.Empty)
        {
            var dialogBox = new DialogBox(DialogMsg);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
            apiResponse.ExecuteScript("ShowButtons();");
        }
        else
        {
            if (!ExistsReset(data.Tid))
            {
                var dialogBox = new DialogBox(Translator.Format("msg_exist"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                dialogBox.AddButton(Translator.Format("passreset"), string.Empty, "class:button;onclick:$NavigateTo('" + References.Pages.XysPassReset + "');");
                apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                apiResponse.ExecuteScript("ShowButtons();");
            }
            else
            {
                data.Pwd = pwd;

                string MailFile = HtmlFolder + References.Htmls.Email_PassChanged + "_" + ClientLanguage + ".html";
                if (!File.Exists(MailFile))
                {
                    MailFile = References.Htmls.Email_PassChanged;
                }
                else
                {
                    MailFile = References.Htmls.Email_PassChanged + "_" + ClientLanguage;
                }

                string rlt = SaveViewData(data);
                if (rlt == string.Empty)
                {
                    string Subject = HtmlTranslator.Value("msg_email");
                    string bodyHtml = ReadHtmlFile(MailFile)
                                             .Replace("{username}", data.Name)
                                             .Replace("{useremail}", data.Email);
                    string[] ToAddr = { data.Email };

                    string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                    if (rltmail == string.Empty)
                    {
                        apiResponse.Navigate(References.Pages.XysSignin);
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
                    var dialogBox = new DialogBox(rlt);
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                    apiResponse.ExecuteScript("ShowButtons();");
                }
            }
        }
        return apiResponse;
    }

    private string SaveViewData(ViewData _data)
    {
        List<string> SQL = new List<string>()
        {
            " update XysUserReset set Status = 9, Expired=getdate() where Tid = N'" + _data.Tid + "' ",
            " update XysUser set " +
            " UserPwd = N'" + Encryptor.EncryptData(_data.Pwd) + "', SYSDTE = GETDATE(), SYSUSR = N'" + _data.UserId + "' " +
            " where UserId = N'" + _data.UserId + "' "
        };
        return PutData(SQL);
    }

    private bool ExistsReset(string Tid)
    {
        bool rtnvlu = false;

        var SQLText = new SQLText();
        SQLText.Sql = " select * from XysUserReset where Status = 0 and getdate() between Created and Expired and Tid = @Tid ";
        SQLText.Params.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@Tid", Value = Tid, SqlDbType = SqlDbType.NVarChar });

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SQLText.ToString(), ref emsg);
        if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
        {
            rtnvlu = true;
        }
        return rtnvlu;
    }

    private string PutData(List<string> SQL)
    {
        string emsg = string.Empty;
        SQLData.SQLDataPut(SQL,ref emsg);
        WriteXysLog(string.Join("|", SQL), emsg);
        return emsg;
    }

    private void WriteXysLog(string logTxt, string msg)
    {
        string userid = (AppKey != null) ? AppKey.UserId : string.Empty;

        List<string> SQL = new List<string>();
        SQL.Add("insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())");

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@LogId", Value = NewID(), SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@ClientIp", Value = ClientIPAddress, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserId", Value = userid, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@LogTxt", Value = EscQuote(logTxt.Trim()), SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@JobRlt", Value = EscQuote(msg.Trim()), SqlDbType = SqlDbType.NVarChar });

        string emsg = string.Empty;
        SQLData _SqlData = new SQLData();
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