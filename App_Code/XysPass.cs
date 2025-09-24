using skylite;
using skylite.ToolKit;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

public class XysPass : WebPage
{
    private AppKey AppKey = null;

    public XysPass()
    {
        string paramVlu = ParamValue(References.Keys.AppKey, true);
        AppKey = (AppKey)DeserializeObjectEnc(paramVlu, typeof(AppKey));

        if (AppKey == null)
        {
            HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin);
        }
        else
        {
            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
        }
        HtmlTranslator.Add(GetPageDict(this.GetType().ToString()));
    }

    protected internal List<Translator.Dictionary> GetPageDict(string pagename)
    {
        List<Translator.Dictionary> rlt = new List<Translator.Dictionary>();
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
        if (emsg == string.Empty && dt != null && dt.Rows.Count != 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Translator.Dictionary _Dict = new Translator.Dictionary();
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

        Label Title = new Label(Translator.Format("userpwd"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        Texts text = new Texts(Translator.Format("pass"), "pass", TextTypes.password);
        text.Required = true;
        text.Text.SetAttribute(HtmlAttributes.name, "pass"); // Set name attribute explicitly as per toolkit rules
        text.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("placeholder"));
        text.Text.SetStyle(HtmlStyles.width, "330px");

        Button btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, "NavXysHome()");
        btn.IDTag = "C101";

        Button btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
        btn1.SetStyle(HtmlStyles.marginLeft, "12px");
        btn1.SetAttribute(HtmlAttributes.@class, "button1");
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignIn()");
        btn1.IDTag = "C102";

        Label lbl2 = new Label(Translator.Format("forgotpwd"));
        lbl2.Wrap.SetStyle(HtmlStyles.textAlign, "right");
        lbl2.Wrap.SetStyle(HtmlStyles.fontSize, "12px");
        lbl2.Wrap.SetStyle(HtmlStyles.paddingRight, "40px");
        lbl2.Wrap.SetStyle(HtmlStyles.color, "#ff6600");
        lbl2.Wrap.SetStyle(HtmlStyles.cursor, "pointer");
        lbl2.Wrap.SetStyle(HtmlStyles.fontStyle, "italic");
        lbl2.Wrap.SetStyle(HtmlStyles.textDecoration, "underline");
        lbl2.Wrap.SetAttribute(HtmlEvents.onclick, "NavXysPassReset()");
        lbl2.IDTag = "T101";

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 40);
        elmBox.AddItem(text);
        elmBox.AddItem(lbl2, 30);
        elmBox.AddItem(btn);
        elmBox.AddItem(btn1, 10);

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
    }

    public ApiResponse NavXysPassReset()
    {
        ApiResponse apiResponse = new ApiResponse();
        apiResponse.Navigate(References.Pages.XysPassReset);
        return apiResponse;
    }

    public ApiResponse NavXysSignIn()
    {
        ApiResponse apiResponse = new ApiResponse();
        apiResponse.Navigate(References.Pages.XysSignin);
        return apiResponse;
    }

    public ApiResponse NavXysHome()
    {
        string pass = GetDataValue("pass");

        ApiResponse apiResponse = new ApiResponse();

        if (pass != string.Empty)
        {
            if (ExistUser(AppKey.UserEmail, pass))
            {
                apiResponse.Navigate(References.Pages.XysHome);
            }
            else
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("msg_wrongcred"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
            }
        }
        else
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("placeholder"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            apiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
        }
        return apiResponse;
    }

    private bool ExistUser(string UserEmail, string UserPwd)
    {
        bool rtnvlu = false;

        string SSQL = " select count(*) from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                             " where b.UserEmail = @UserEmail and a.UserPwd = @UserPwd ";

        List<System.Data.SqlClient.SqlParameter> SqlParams = new List<System.Data.SqlClient.SqlParameter>();
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = SqlDbType.NVarChar });
        SqlParams.Add(new System.Data.SqlClient.SqlParameter { ParameterName = "@UserPwd", Value = Encryptor.EncryptData(UserPwd), SqlDbType = SqlDbType.NVarChar });

        string query = SqlWithParams(SSQL, SqlParams);
        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(query, ref emsg);
        if (emsg == string.Empty && dt != null && dt.Rows.Count != 0)
        {
            if (int.Parse(dt.Rows[0][0].ToString()) != 0)
            {
                rtnvlu = true;
            }
        }
        return rtnvlu;
    }
}