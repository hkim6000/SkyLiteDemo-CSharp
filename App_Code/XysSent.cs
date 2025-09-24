using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;

public class XysSent : WebPage
{
    public XysSent()
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

        var lbl1 = new Label(Translator.Format("sentemail"));
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

        var lbl2 = new Label(Translator.Format("waitemail"));
        lbl2.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
        lbl2.Wrap.SetStyle(HtmlStyles.color, "#444");

        var btn = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
        btn.SetStyle(HtmlStyles.marginLeft, "6px");
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");


        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 30);
        elmBox.AddItem(lbl1, 16);
        elmBox.AddItem(lbl2, 30);
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
}