using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;

public class XysHome : WebBase
{
    public override ViewPart InitialModel()
    {
        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        ViewPart.Mode = ViewMode.View;

        ViewPart.UIControl = new UIControl();
        ViewPart.UIControl.Set(new UIControl.Item[]
        {
            new UIControl.Item { Name = "BltnTitle", Label = "", Styles = "width:500px;border:none;border-bottom:1px solid #333;border-radius:0px;", IsReadOnly = true, LineSpacing = 1 },
            new UIControl.Item { Name = "BltnMemo", Label = "", Styles = "font-size:12px;width:500px; height:180px;border:none;", UIType = UITypes.TextArea, IsReadOnly = true, LineSpacing = 1 },
            new UIControl.Item { Name = "CreatedBy", Label = "", Styles = "width:260px;border:none;border-bottom:1px solid #333;border-radius:0px;", IsReadOnly = true, LineSpacing = 1 },
            new UIControl.Item { Name = "SYSDTE", Label = "", Styles = "border:none; font-size:12px;width:160px;", IsReadOnly = true, LineSpacing = 1 },
            new UIControl.Item { Name = "FileRefId", Label = "Attach File(s)", Styles = "margin-left:14px", UIType = UITypes.File, IsVisible = false }
        });

        return ViewPart;
    }

    public override void OnBeforRender()
    {
        if (AppKey == null)
        {
            HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin);
        }
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[]{});

        Label fLabel = new Label();
        fLabel.Wrap.InnerText = Translator.Format("hi") + AppKey.UserName;
        fLabel.Wrap.SetStyle(HtmlStyles.fontSize, "22px");
        fLabel.Wrap.SetStyle(HtmlStyles.fontWeight, "bold");
        fLabel.Wrap.SetStyle(HtmlStyles.margin, "14px 0px 0px 14px");

        FilterSection filter = new FilterSection();
        filter.FilterHtml = fLabel.HtmlText;
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.margin, "8px 2% 8px 2%");
        filter.Menu = mnulist;

        ImageBox Image = new ImageBox();
        Image.Image.SetAttribute(HtmlAttributes.src, ImagePath + "img_home.gif");

        MultiMenuSection mmnu = new MultiMenuSection();
        mmnu.Wrap.SetStyle(HtmlStyles.margin, "16px 2% 8px 2%");
        mmnu.Wrap.SetStyle(HtmlStyles.marginBottom, "80px");

        string emsg = string.Empty;
        DataTable BltnDt = SQLData.SQLDataTable(
                  " SELECT top 15 BltnId, " +
                  "        BltnTitle, " +
                  "        ' (' + replace(substring(convert(varchar(10),SYSDTE,121),6,5),'-','/') + ')' BltnDte " +
                  " FROM XysBulletin ORDER BY SYSDTE DESC ", ref emsg);

        if (BltnDt != null && BltnDt.Rows.Count > 0)
        {
            MenuList mnus = new MenuList();
            mnus.Title.InnerText = Translator.Format("bulletin");
            mnus.Wrap.SetStyles(" border:1px solid #ff6600;");
            for (int i = 0; i < BltnDt.Rows.Count; i++)
            {
                string bltnId = BltnDt.Rows[i][0].ToString();
                string bltnTitle = BltnDt.Rows[i][1].ToString();
                string bltnDte = BltnDt.Rows[i][2].ToString();

                bltnTitle = (bltnTitle.Length > 24) ? bltnTitle.Substring(0, 24) + " ...." : bltnTitle;

                HtmlTag mnuitem = new HtmlTag();
                mnuitem.SetStyles("font-size:14px;");
                mnuitem.InnerText = bltnTitle + bltnDte;
                mnuitem.SetAttribute(HtmlEvents.onclick, ByPassCall("ShowBulletin", "t=" + bltnId));
                mnus.Add(mnuitem);
            }
            mmnu.Menus.Add(mnus);
        }

        List<string> PageGroups = GetPageGroups();
        if (PageGroups.Count > 0)
        {
            List<MenuablePage> MenuablePages = GetMeuablePages(PageGroups);
            if (MenuablePages != null)
            {
                for (int i = 0; i < PageGroups.Count; i++)
                {
                    string _pagegroup = PageGroups[i];
                    List<MenuablePage> _MenuablePages = MenuablePages.FindAll(x => x.PageGroup == _pagegroup);

                    if (_MenuablePages.Count > 0)
                    {
                        MenuList mnus = new MenuList();
                        mnus.Title.InnerText = _pagegroup;

                        for (int j = 0; j < _MenuablePages.Count; j++)
                        {
                            string mnuAttr = HtmlAttributes.title + ":" + _MenuablePages[j].PageDesc + ";" +
                                             HtmlEvents.onclick + ":" + ByPassCall("NaviClick", "m=" + _MenuablePages[j].PageName);
                            mnus.Add(_MenuablePages[j].PageDesc, string.Empty, mnuAttr);
                        }
                        mmnu.Menus.Add(mnus);
                    }
                }
            }
        }

        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

        TitleSection2 PageLayout = PageTitle();
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
        PageLayout.ContentWrap.InnerText = Image.HtmlText + filter.HtmlText + mmnu.HtmlText;

        return PageLayout.HtmlText;
    }

    private List<MenuablePage> GetMeuablePages(List<string> PageGroups)
    {
        List<MenuablePage> rlt = null;
        string JoinPageGroups = "(N'" + string.Join("',N'", PageGroups) + "')";
        string SSQL = " select PageId,PageName,PageDesc,PageGroup from XysPage " +
                             " where PageMenu = 1 and PageUse = 1 and PageGroup in " + JoinPageGroups + " " +
                             " order by PageOrder ";

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SSQL, ref emsg);
        if (dt != null && dt.Rows.Count != 0)
        {
            rlt = DataTableListT<MenuablePage>(dt);
        }
        return rlt;
    }

    private List<string> GetPageGroups()
    {
        List<string> rlt = new List<string>();
        string SSQL = " declare @roleid nvarchar(50) " +
                             " set @roleid = N'" + AppKey.RoleId + "' " +
                             " select PageGroup from XysPage " +
                             " where PageUse = 1 and PageGroup <> '' and PageId in (select PageId from XysRolePage where RoleId = @roleid ) " +
                             " order by PageOrder ";

        string emsg = string.Empty;
        DataTable dt = SQLData.SQLDataTable(SSQL, ref emsg);
        if (dt != null && dt.Rows.Count != 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string pagegroup = dt.Rows[i][0].ToString();
                string grp = rlt.Find(x => x == pagegroup);
                if (grp == null)
                {
                    rlt.Add(pagegroup);
                }
            }
        }
        return rlt;
    }

    public ApiResponse NaviClick()
    {
        string m = GetDataValue("m");
        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(m);
        return _ApiResponse;
    }

    public ApiResponse Profile()
    {
        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(References.Pages.XysProfile);
        return _ApiResponse;
    }

    public ApiResponse SignOut()
    {
        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.RemoveCookie(References.Keys.AppKey);
        _ApiResponse.ClearStorage();
        _ApiResponse.Navigate(References.Pages.XysSignin);
        return _ApiResponse;
    }

    public ApiResponse ShowBulletin()
    {
        string t = GetDataValue("t");

        ViewPart.UIControl.Data = GetBltn(t);
        ViewPart.UIControl.UIMode = UIControl.Modes.View;
        ViewPart.UIControl.Wrap.SetStyle(HtmlStyles.padding, "10px");
        string BltnContents = ViewPart.UIControl.HtmlText;

        DialogBox dialogBox = new DialogBox(BltnContents);
        dialogBox.ContentsWrap.SetStyles("width:600px;");

        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.PopUpWindow(dialogBox.HtmlText);
        return _ApiResponse;
    }

    public ApiResponse FileDownLoad()
    {
        return FileDownLoadEnc();
    }

    public DataTable GetBltn(string BltnId)
    {
        string emsg = string.Empty;
        string SSQL = string.Empty;
        SSQL = " declare @BltnId decimal(18) " +
                " set @BltnId = " + decimal.Parse(BltnId).ToString() + "  " +
                " select BltnId, BltnTitle, BltnMemo, CreatedBy,'@E #|' + dbo.XF_FileDownListEnc(FileRefId,'#') FileRefId, SYSDTE, SYSUSR " +
                " from XysBulletin " +
                " where  BltnId = @BltnId    ";
        DataTable dt = SQLData.SQLDataTable(SSQL, ref emsg);
        return dt;
    }

    private class MenuablePage
    {
        public string PageId = string.Empty;
        public string PageName = string.Empty;
        public string PageDesc = string.Empty;
        public string PageGroup = string.Empty;
    }
}