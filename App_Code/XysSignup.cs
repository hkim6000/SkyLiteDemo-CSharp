using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public class XysSignup : WebPage
{
    private AppKey AppKey = null;

    public XysSignup()
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
            SSQL += " if exists(select * from XysDict where Isocode =  @isocode) " +
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
            SSQL += " if exists(select * from XysDict where left(Isocode,2) =  @isocode) " +
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
        if (emsg == string.Empty && dt != null && dt.Rows.Count != 0)
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

        var Title = new Label(Translator.Format("signup"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        var text = new Texts(Translator.Format("name"), "name", TextTypes.text);
        text.Required = true;
        text.Text.SetStyle(HtmlStyles.width, "200px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
        text.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

        var text1 = new Texts(Translator.Format("email"), "email", TextTypes.text);
        text1.Required = true;
        text1.Text.SetStyle(HtmlStyles.width, "350px");
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "150");
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
        text1.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

        var text2 = new Texts(Translator.Format("pwd"), "pwd", TextTypes.password);
        text2.Required = true;
        text2.Text.SetStyle(HtmlStyles.width, "200px");
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "20");
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
        text2.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

        var text3 = new Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password);
        text3.Required = true;
        text3.Text.SetStyle(HtmlStyles.width, "200px");
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "20");
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
        text3.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

        var btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, "NavXysVerify()");

        var btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
        btn1.SetAttribute(HtmlAttributes.@class, "button");
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 34);
        elmBox.AddItem(text, 1);
        elmBox.AddItem(text1, 14);
        elmBox.AddItem(text2, 1);
        elmBox.AddItem(text3, 40);
        elmBox.AddItem(btn1);
        elmBox.AddItem(btn, 10);

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
    }

    public ApiResponse NavXysSignin()
    {
        var _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(References.Pages.XysSignin);
        return _ApiResponse;
    }

    public ApiResponse NavXysVerify()
    {
        string name = GetDataValue("name");
        string email = GetDataValue("email");
        string pwd = GetDataValue("pwd");
        string pwd1 = GetDataValue("pwd1");

        var _ApiResponse = new ApiResponse();

        bool Valid = true;
        string DialogMsg = string.Empty;

        if (name == string.Empty || email == string.Empty || pwd == string.Empty || pwd1 == string.Empty)
        {
            Valid = false;
        }

        if (Valid == false)
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
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
            _ApiResponse.ExecuteScript("ShowButtons();");
        }
        else
        {
            if (ExistsEmail(email) == true)
            {
                var dialogBox = new DialogBox(Translator.Format("msg_exist"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                _ApiResponse.ExecuteScript("ShowButtons();");
            }
            else
            {
                var data = new ViewData();
                data.Tid = NewID();
                data.Name = name;
                data.Email = email;
                data.Pass = Encryptor.EncryptData(pwd);
                data.OTP = RandNUM().ToString();
                data.IpAddr = ClientIPAddress;
                data.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                data.Expired = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss");

                string MailFile = HtmlFolder + References.Htmls.Email_SignUp + "_" + ClientLanguage + ".html";
                if (!File.Exists(MailFile))
                {
                    MailFile = References.Htmls.Email_SignUp;
                }
                else
                {
                    MailFile = References.Htmls.Email_SignUp + "_" + ClientLanguage;
                }

                string rlt = SaveViewData(data);
                if (rlt == string.Empty)
                {
                    string Subject = HtmlTranslator.Value("msg_email");
                    string bodyHtml = ReadHtmlFile(MailFile)
                                             .Replace("{username}", data.Name)
                                             .Replace("{useremail}", data.Email)
                                             .Replace("{userpin}", data.OTP);
                    string[] ToAddr = { data.Email };

                    string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                    if (rltmail == string.Empty)
                    {
                        string SerializedString = SerializeObjectEnc(data, typeof(ViewData));
                        _ApiResponse.SetCookie(References.Keys.SignUp_User, SerializedString);
                        _ApiResponse.Navigate(References.Pages.XysVerify);
                    }
                    else
                    {
                        var dialogBox = new DialogBox(rltmail);
                        dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                        _ApiResponse.ExecuteScript("ShowButtons();");
                    }
                }
                else
                {
                    var dialogBox = new DialogBox(rlt);
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                    _ApiResponse.ExecuteScript("ShowButtons();");
                }
            }
        }
        return _ApiResponse;
    }

    private string SaveViewData(ViewData data)
    {
        List<string> SQL = new List<string>()
        {
            " Insert into XysSignUp( Tid,Name,Email,Pass,OTP,IpAddr,Created,Expired) " +
            " values ( @Tid,@Name,@Email,@Pass,@OTP,@IpAddr,@Created,@Expired) "
        };

         List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>()
        {
            new SqlParameter { ParameterName = "@Tid", Value = data.Tid, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@Name", Value = data.Name, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@Email", Value = data.Email, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@Pass", Value = data.Pass, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@OTP", Value = data.OTP, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@IpAddr", Value = data.IpAddr, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@Created", Value = data.Created, SqlDbType = SqlDbType.DateTime },
            new SqlParameter { ParameterName = "@Expired", Value = data.Expired, SqlDbType = SqlDbType.DateTime }
        };
        return PutData(SqlWithParams(SQL, SqlParams));
    }

    private bool ExistsEmail(string UserEmail)
    {
        bool rtnvlu = false;
        var SQLText = new SQLText();
        SQLText.Sql = " if exists(select * from XysUserInfo  where UserEmail = @UserEmail) " +
                      " begin select 1 end else begin select 0 end ";
        SQLText.Params.Add(new SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = SqlDbType.NVarChar });

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SQLText.ToString(), ref emsg);
        if (emsg == string.Empty && dt != null && dt.Rows.Count != 0)
        {
            if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
            {
                rtnvlu = true;
            }
        }
        return rtnvlu;
    }

    private string PutData(List<string> SQL)
    {
        string emsg = string.Empty;
        SQLData.SQLDataPut(SQL, ref emsg);
        WriteXysLog(string.Join("|", SQL), ref emsg);
        return emsg;
    }

    private void WriteXysLog(string logTxt, ref string msg)
    {
        string userid = (AppKey != null) ? AppKey.UserId : string.Empty;
        List<string> SQL = new List<string>()
        {
            "insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) " +
            "values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())"
        };

         List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>()
        {
            new SqlParameter { ParameterName = "@LogId", Value = NewID(), SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@ClientIp", Value = ClientIPAddress, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@UserId", Value = userid, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@LogTxt", Value = EscQuote(logTxt.Trim()), SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@JobRlt", Value = EscQuote(msg.Trim()), SqlDbType = SqlDbType.NVarChar }
        };
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
        public string Name = string.Empty;
        public string Email = string.Empty;
        public string Pass = string.Empty;
        public string OTP = string.Empty;
        public string IpAddr = string.Empty;
        public string Created = string.Empty;
        public string Expired = string.Empty;
    }
}