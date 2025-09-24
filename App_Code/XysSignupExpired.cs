using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;

public class XysSignupExpired : WebPage
{
    public XysSignupExpired()
    {
        HtmlTranslator.Add(GetPageDict(GetType().ToString()));
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

        Label Title = new Label(Translator.Format("expired"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        Label lbl1 = new Label(Translator.Format("pinexpired"));
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

        Button btn = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
        btn.SetStyle(HtmlStyles.marginLeft, "6px");
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, "NavXysSignup()");

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 30);
        elmBox.AddItem(lbl1, 16);
        elmBox.AddItem(btn, 10);

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
    }

    public ApiResponse NavXysSignup()
    {
        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(References.Pages.XysSignup);
        return _ApiResponse;
    }
}