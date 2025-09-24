using skylite;
using skylite.ToolKit;
using System.Data;
using System.Collections.Generic;

public class XysUnAuthorized : WebPage
{
    public XysUnAuthorized()
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
        DataTable dt = SQLData.SQLDataTable(SSQL,ref emsg);
        if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
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

        Label Title = new Label(Translator.Format("unauthorize"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "40px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "16px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "20px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        Button btn = new Button(Translator.Format("home"), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetStyle(HtmlStyles.marginLeft, "20px");
        btn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysUnAuthorized/NaviClick", "m=" + References.Pages.XysHome));

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "108px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

        elmBox.AddItem(Title, 30);
        elmBox.AddItem(btn, 1);

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
    }

    public ApiResponse NaviClick()
    {
        string m = GetDataValue("m");
        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(m);
        return _ApiResponse;
    }
}