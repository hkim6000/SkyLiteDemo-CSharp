using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;

public class XysSignin : WebPage
{
    private AppKey AppKey = null;

    public XysSignin()
    {
        HtmlTranslator.Add(GetPageDict(this.GetType().ToString()));
    }

    protected internal List<Translator.Dictionary> GetPageDict(string pagename)
    {
        List<Translator.Dictionary> rlt = new List<Translator.Dictionary>();
        string SSQL =
                        " declare @pageid nvarchar(50),@isocode nvarchar(10)  " +
                        " set @pageid = N'" + pagename + "' " +
                        " set @isocode = N'" + ClientLanguage + "' ";

        switch (ClientLanguage.Contains("-"))
        {
            case true:
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
                break;
            case false:
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
                break;
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

        Label Title = new Label(Translator.Format("hi"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        Texts text = new Texts(Translator.Format("email"), "email", TextTypes.text);
        text.Required = true;
        text.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("placeholder"));
        text.Text.SetStyle(HtmlStyles.width, "330px");

        Button btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, ByPassCall("NavXysPass", "email=::"));

        Label lbl1 = new Label(Translator.Format("signup"));
        lbl1.Wrap.SetStyle(HtmlStyles.position, "absolute");
        lbl1.Wrap.SetStyle(HtmlStyles.top, "26px");
        lbl1.Wrap.SetStyle(HtmlStyles.right, "34px");
        lbl1.Wrap.SetStyle(HtmlStyles.fontSize, "14px");
        lbl1.Wrap.SetStyle(HtmlStyles.textDecoration, "underline");
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#ff6600");
        lbl1.Wrap.SetStyle(HtmlStyles.cursor, "pointer");
        lbl1.Wrap.SetAttribute(HtmlEvents.onclick, ByPassCall("NavXysSignup"));

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 40);
        elmBox.AddItem(text, 40);
        elmBox.AddItem(btn, 10);
        elmBox.AddItem(lbl1);

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
        HtmlDoc.InitialScripts.RemoveCookie(References.Keys.AppKey);
    }

    public ApiResponse NavXysPass()
    {
        string email = GetDataValue("email");

        ApiResponse _ApiResponse = new ApiResponse();

        if (email != string.Empty)
        {
            if (!ExistUser(email))
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("msg_email"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText);
            }
            else
            {
                string SerializedString = SerializeObjectEnc(AppKey, typeof(AppKey));
                _ApiResponse.SetCookie(References.Keys.AppKey, SerializedString);
                _ApiResponse.Navigate(References.Pages.XysPass);
            }
        }
        else
        {
            DialogBox dialogBox = new DialogBox(Translator.Format("placeholder"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText);
        }
        return _ApiResponse;
    }

    public ApiResponse NavXysSignup()
    {
        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(References.Pages.XysSignup);
        return _ApiResponse;
    }

    private bool ExistUser(string UserEmail)
    {
        bool rtnvlu = false;

        string SSQL =
            " select a.UserId,UserName,UserEmail,UserPhone,UserPic,RoleId,UserRef " +
            " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
            " where b.UserEmail = @UserEmail ";

        List<SqlParameter> SqlParams = new List<SqlParameter>();
        SqlParams.Add(new SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = SqlDbType.NVarChar });

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SqlWithParams(SSQL, SqlParams), ref emsg);
        if (emsg == string.Empty && dt != null && dt.Rows.Count != 0)
        {
            AppKey = new AppKey();
            AppKey.UserId = dt.Rows[0][0].ToString();
            AppKey.UserName = dt.Rows[0][1].ToString();
            AppKey.UserEmail = dt.Rows[0][2].ToString();
            AppKey.UserPhone = dt.Rows[0][3].ToString();
            AppKey.RoleId = dt.Rows[0][4].ToString(); // Note: Original VB had RoleId assigned twice, keeping that pattern
            AppKey.RoleId = dt.Rows[0][5].ToString();
            AppKey.UserRef = dt.Rows[0][6].ToString();
            AppKey.DateTime = DateTime.Now;

            rtnvlu = true;
        }
        return rtnvlu;
    }
}