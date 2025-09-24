using skylite;
using skylite.ToolKit;
using System.Data;

public class XysSettings : WebBase
{
    public override string InitialView()
    {
        string ViewHtml = string.Empty;

        var Title = new Label();
        Title.Wrap.InnerText = Translator.Format("settings");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "28px");
        Title.Wrap.SetStyle(HtmlStyles.fontWeight, "bold");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "10px");

        var mnu = new Label();
        mnu.Wrap.InnerText = "• " + Translator.Format("setroles");
        mnu.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysRole));
        mnu.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
        mnu.IDTag = "T100";

        var mnu1 = new Label();
        mnu1.Wrap.InnerText = "• " + Translator.Format("setaccounts");
        mnu1.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysUser));
        mnu1.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
        mnu1.IDTag = "T110";

        var mnu2 = new Label();
        mnu2.Wrap.InnerText = "• " + Translator.Format("setpages");
        mnu2.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysPage));
        mnu2.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
        mnu2.IDTag = "T120";

        var mnu3 = new Label();
        mnu3.Wrap.InnerText = "• " + Translator.Format("setmenu");
        mnu3.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysMenu));
        mnu3.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
        mnu3.IDTag = "T130";

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.ClearStyles();
        elmBox.Wrap.SetStyle(HtmlStyles.marginLeft, "50px");
        elmBox.Wrap.SetStyle(HtmlStyles.marginTop, "60px");

        elmBox.AddItem(Title, 50);

        ViewHtml = elmBox.HtmlText;
        return ViewHtml;
    }
}