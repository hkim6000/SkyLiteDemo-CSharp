using skylite;
using skylite.ToolKit;
using System.Data;

public class SupportMV : WebBase
{
    public override ViewPart InitialModel()
    {
        return new ViewPart { Methods = ViewMethods("Support") };
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] { });
        Wrap BtnWrap = SetPageButtons(new string[] { });

        var label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = Translator.Format("support");

        var filter = new skylite.ToolKit.FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;

        return ViewHtml;
    }
}