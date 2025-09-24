using skylite;
using skylite.ToolKit;

public class XysUser : WebBase
{
    public override string InitialView()
    {
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

        TitleSection2 PageLayout = PageTitle();
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
        PageLayout.ContentWrap.InnerText = PartialPage(References.Pages.XysUserMV);

        return PageLayout.HtmlText;
    }

    public ApiResponse MenuClick()
    {
        string m = GetDataValue("m");
        string t = GetDataValue("t");

        ApiResponse apiResponse = new ApiResponse();
        apiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(m, t));
        apiResponse.ExecuteScript("$ScrollToTop()");
        return apiResponse;
    }

    public ApiResponse NaviClick()
    {
        string m = GetDataValue("m");
        ApiResponse apiResponse = new ApiResponse();
        apiResponse.Navigate(m);
        return apiResponse;
    }
}