using skylite;
using skylite.ToolKit;
using System.Data;

public class XysLevel : WebBase
{
    public override string InitialView()
    {
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

        TitleSection2 PageLayout = PageTitle();
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
        PageLayout.ContentWrap.InnerText = PartialPage(References.Pages.XysLevelMV);

        return PageLayout.HtmlText;
    }

    public ApiResponse MenuClick()
    {
        string m = GetDataValue("m");
        string t = GetDataValue("t");

        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(m, t));
        _ApiResponse.ExecuteScript("$ScrollToTop()");
        return _ApiResponse;
    }

    public ApiResponse NaviClick()
    {
        string m = GetDataValue("m");
        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(m);
        return _ApiResponse;
    }
}