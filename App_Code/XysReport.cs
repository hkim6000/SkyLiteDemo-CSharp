using skylite;
using skylite.ToolKit;
using System;
using System.Collections.Generic;
using System.Data;

public class XysReport : WebBase
{
    private OptionValues rpts = new OptionValues();
    public XysReport()
    {
        rpts.AddItem(Translator.Format("select"), "00000");

        List<NameValueGroup> data = GetNameValueGrp();
        if (data != null && data.Count > 0)
        {
            for (int i = 0; i < data.Count; i++)
            {
                rpts.AddItem(data[i].name, data[i].value, data[i].group);
            }
        }
    }
    private List<NameValueGroup> GetNameValueGrp()
    {
        string emsg = string.Empty;
        string SSQL = " select MenuDesc name,MenuTag value, MenuMethod  as [group] " +
                               " from XysMenu where MenuArea = N'X' and  " +
                               "PageId = (select PageId from XysPage where PageName = N'XysReport') order by MenuTag ";
        List<NameValueGroup> data = DataTableListT<NameValueGroup>(SQLData.SQLDataTable(SSQL, ref emsg));
        return data;
    }

    public override string InitialView()
    {
        var mnulist = SetPageMenu(ViewPart.Mode == ViewMode.New ? new[] { "xysrole" } : new string[] { });
        var BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new[] { "save" } : new string[] { });

        var label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = Translator.Format("report");

        var filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px");
        filter.Wrap.SetStyle(HtmlStyles.width, "95%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        var filterDD = new Dropdown();
        filterDD.Wrap.SetStyle(HtmlStyles.margin, "2px");
        filterDD.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
        filterDD.SelBox.SetStyle(HtmlStyles.fontSize, "20px");
        filterDD.SelBox.SetStyle(HtmlStyles.borderColor, "#333");
        filterDD.SelBox.SetAttribute(HtmlAttributes.id, "rpt");
        filterDD.SelBox.SetAttribute(HtmlEvents.onchange, "CallReport(this)");
        filterDD.SelBox.InnerText = rpts.HtmlText;

        var FilterSection = new FilterSection();
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%");
        FilterSection.FilterWrap.SetStyle(HtmlStyles.padding, string.Empty);
        FilterSection.FilterHtml = filterDD.HtmlText;

        var Wrap = new Wrap();
        Wrap.SetAttribute(HtmlAttributes.id, "RptBox");
        Wrap.SetStyle(HtmlStyles.boxSizing, "border-box");
        Wrap.SetStyle(HtmlStyles.padding, "8px");
        Wrap.SetStyle(HtmlStyles.width, "100%");
        Wrap.SetStyle(HtmlStyles.border, "1px solid #aaa");
        Wrap.SetStyle(HtmlStyles.borderRadius, "6px");
        Wrap.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)");

        Wrap.InnerText = Rpt_Empty();

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "95%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "10px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "10px");

        elmBox.AddItem(FilterSection, 10);
        elmBox.AddItem(Wrap, 10);

        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

        var PageLayout = PageTitle();
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
        PageLayout.ContentWrap.InnerText = filter.HtmlText + elmBox.HtmlText;

        return PageLayout.HtmlText;
    }
    public ApiResponse NaviClick()
    {
        string m = GetDataValue("m");
        var _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(m);
        return _ApiResponse;
    }
    private string Rpt_Empty()
    {
        var Wrap = new Wrap();
        Wrap.SetStyle(HtmlStyles.display, "table-cell");
        Wrap.SetStyle(HtmlStyles.verticalAlign, "bottom");
        Wrap.SetStyle(HtmlStyles.fontSize, "20px");
        Wrap.SetStyle(HtmlStyles.color, "darkolivegreen");
        Wrap.InnerText = Translator.Format("selrpt");
        return Wrap.HtmlText;
    }
    public ApiResponse CallReport()
    {
        string rpt = GetDataValue("rpt");

        var _ApiResponse = new ApiResponse();

        string rpthtml = string.Empty;
        switch (rpt)
        {
            case "00000":
                rpthtml = Rpt_Empty();
                break;
            case "10000":
                rpthtml = Rpt_10000();
                break;
            case "20000":
                rpthtml = Rpt_20000();
                break;
            case "30000":
                rpthtml = Rpt_30000();
                break;
        }

        _ApiResponse.SetElementContents("RptBox", rpthtml);
        return _ApiResponse;
    }

    private string Rpt_10000()
    {
        string rptHtml = string.Empty;

        string bdt = DateTime.Now.ToString("yyyy-MM-dd");

        var filterText = new Texts(TextTypes.date);
        filterText.Text.SetAttribute(HtmlAttributes.name, "fltdte");
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
        filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
        filterText.Text.SetStyle(HtmlStyles.fontSize, "20px");
        filterText.Text.SetStyle(HtmlStyles.height, "20px");
        filterText.Text.SetStyle(HtmlStyles.padding, "8px");
        filterText.Text.SetStyle(HtmlStyles.border, "none");
        filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
        filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte");
        filterText.Text.SetAttribute(HtmlAttributes.value, bdt);

        var filterBtn = new Button();
        filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')");
        filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat");
        filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px");
        filterBtn.SetStyle(HtmlStyles.borderRadius, "50%");
        filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd");
        filterBtn.SetStyle(HtmlStyles.padding, "6px");
        filterBtn.SetStyle(HtmlStyles.height, "30px");
        filterBtn.SetStyle(HtmlStyles.width, "30px");
        filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)");
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_10000", "fltdte=::"));

        var FilterSection = new FilterSection();
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "20px");
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%");
        FilterSection.Wrap.SetStyle(HtmlStyles.padding, string.Empty);
        FilterSection.FilterHtml = filterText.HtmlText + filterBtn.HtmlText;

        var Wrap = new Wrap();
        Wrap.SetAttribute(HtmlAttributes.id, "RptRlt");
        Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
        Wrap.SetStyle(HtmlStyles.paddingTop, "8px");
        Wrap.SetStyle(HtmlStyles.width, "100%");

        rptHtml = FilterSection.HtmlText + Wrap.HtmlText;

        return rptHtml;
    }
    public ApiResponse RptRlt_10000()
    {
        string fltdte = GetDataValue("fltdte");
        DateTime dt;

        var _ApiResponse = new ApiResponse();

        if (DateTime.TryParse(fltdte, out dt) == false)
        {
            _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"));
        }
        else
        {
            var SQLGridInfo = new SQLGridSection.SQLGridInfo
            {
                Id = "ClockInGrid",
                Name = Translator.Format("clockin"),
                CurrentPageNo = 1,
                LinesPerPage = 50,
                DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                TitleEnabled = true,
                TDictionary = this.HtmlTranslator.TDictionary,
                Query = new SQLGridSection.SQLQuery
                {
                    Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                    OrderBy = new string[] { "a.sysdte desc" },
                    Columns = new string[] { "c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit", "dateadd(hour, convert(int,LocTime), a.sysdte) Local" },
                    ColumnAlias = new string[] { Translator.Format("name"), Translator.Format("email"), Translator.Format("loc"), Translator.Format("dst"), Translator.Format("unit"), Translator.Format("time") },
                    Filters = " convert(varchar(10),dateadd(hour, convert(int,LocTime), a.sysdte),121) = '" + DateTime.Parse(fltdte).ToString("yyyy-MM-dd") + "' "
                }
            };
            SQLGridSection SQLGrid= new SQLGridSection(SQLGridInfo);
            SetGridStyle(SQLGrid);

            _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText);
        }

        return _ApiResponse;
    }
    private void SetGridStyle(SQLGridSection SQLGrid)
    {
        SQLGrid.Wrap.SetStyle(HtmlStyles.margin, string.Empty);
        SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block");

        if (SQLGrid.GridData != null)
        {
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
            SQLGrid.Grid.TableColumns[5].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
            SQLGrid.Grid.TableColumns[4].SetColumnFormat("@R {4} | K.Km, M.Mile");
            SQLGrid.Grid.TableColumns[5].SetColumnFormat("@D {5} | MM/dd/yyyy HH:mm:ss");

            for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                        SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "left");
                        break;
                    default:
                        SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "center");
                        break;
                }
            }
        }
    }

    private string Rpt_20000()
    {
        string rptHtml = string.Empty;

        string bdt = DateTime.Now.ToString("yyyy-MM");

        var filterText = new Texts(TextTypes.month);
        filterText.Text.SetAttribute(HtmlAttributes.name, "fltdte");
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
        filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
        filterText.Text.SetStyle(HtmlStyles.fontSize, "20px");
        filterText.Text.SetStyle(HtmlStyles.height, "20px");
        filterText.Text.SetStyle(HtmlStyles.padding, "8px");
        filterText.Text.SetStyle(HtmlStyles.border, "none");
        filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
        filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte");
        filterText.Text.SetAttribute(HtmlAttributes.value, bdt);

        var filterBtn = new Button();
        filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')");
        filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat");
        filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px");
        filterBtn.SetStyle(HtmlStyles.borderRadius, "50%");
        filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd");
        filterBtn.SetStyle(HtmlStyles.padding, "6px");
        filterBtn.SetStyle(HtmlStyles.height, "30px");
        filterBtn.SetStyle(HtmlStyles.width, "30px");
        filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)");
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_20000", "fltdte=::"));

        var FilterSection = new FilterSection();
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "20px");
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%");
        FilterSection.Wrap.SetStyle(HtmlStyles.padding, string.Empty);
        FilterSection.FilterHtml = filterText.HtmlText + filterBtn.HtmlText;

        var Wrap = new Wrap();
        Wrap.SetAttribute(HtmlAttributes.id, "RptRlt");
        Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
        Wrap.SetStyle(HtmlStyles.paddingTop, "8px");
        Wrap.SetStyle(HtmlStyles.width, "100%");

        rptHtml = FilterSection.HtmlText + Wrap.HtmlText;

        return rptHtml;
    }
    public ApiResponse RptRlt_20000()
    {
        string fltdte = GetDataValue("fltdte");
        DateTime dt;

        var _ApiResponse = new ApiResponse();
        
        if (!DateTime.TryParse(fltdte + "-01", out dt))
        {
            _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"));
        }
        else
        {
            var SQLGridInfo = new SQLGridSection.SQLGridInfo
            {
                Id = "ClockInGrid",
                Name = Translator.Format("clockin"),
                CurrentPageNo = 1,
                LinesPerPage = 50,
                DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                TitleEnabled = true,
                TDictionary = this.HtmlTranslator.TDictionary,
                Query = new SQLGridSection.SQLQuery
                {
                    Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                    OrderBy = new string[] { "a.sysdte desc" },
                    Columns = new string[] { "c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit", "dateadd(hour, convert(int,LocTime), a.sysdte) Local" },
                    ColumnAlias = new string[] { Translator.Format("name"), Translator.Format("email"), Translator.Format("loc"), Translator.Format("dst"), Translator.Format("unit"), Translator.Format("time") },
                    Filters = " convert(varchar(7),dateadd(hour, convert(int,LocTime), a.sysdte),121) = '" + DateTime.Parse(fltdte).ToString("yyyy-MM") + "' "
                }
            };
            SQLGridSection SQLGrid= new SQLGridSection(SQLGridInfo);
            SetGridStyle(SQLGrid);

            _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText);
        }

        return _ApiResponse;
    }

    private string Rpt_30000()
    {
        string rptHtml = string.Empty;

        string bdt = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
        string edt = DateTime.Now.ToString("yyyy-MM-dd");

        var filterText = new Texts(TextTypes.date);
        filterText.Text.SetAttribute(HtmlAttributes.name, "fltdte");
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
        filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
        filterText.Text.SetStyle(HtmlStyles.fontSize, "20px");
        filterText.Text.SetStyle(HtmlStyles.height, "20px");
        filterText.Text.SetStyle(HtmlStyles.padding, "8px");
        filterText.Text.SetStyle(HtmlStyles.border, "none");
        filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
        filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte");
        filterText.Text.SetAttribute(HtmlAttributes.value, bdt);

        var filterText1 = new Texts(TextTypes.date);
        filterText1.Text.SetAttribute(HtmlAttributes.name, "fltdte1");
        filterText1.Wrap.SetStyle(HtmlStyles.margin, "2px");
        filterText1.Wrap.SetStyle(HtmlStyles.marginRight, "10px");
        filterText1.Wrap.SetStyle(HtmlStyles.marginTop, "10px");
        filterText1.Text.SetStyle(HtmlStyles.fontSize, "20px");
        filterText1.Text.SetStyle(HtmlStyles.height, "20px");
        filterText1.Text.SetStyle(HtmlStyles.padding, "8px");
        filterText1.Text.SetStyle(HtmlStyles.border, "none");
        filterText1.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa");
        filterText1.Text.SetAttribute(HtmlAttributes.id, "fltdte1");
        filterText1.Text.SetAttribute(HtmlAttributes.value, edt);

        var filterBtn = new Button();
        filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')");
        filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat");
        filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px");
        filterBtn.SetStyle(HtmlStyles.borderRadius, "50%");
        filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd");
        filterBtn.SetStyle(HtmlStyles.padding, "6px");
        filterBtn.SetStyle(HtmlStyles.height, "30px");
        filterBtn.SetStyle(HtmlStyles.width, "30px");
        filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)");
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_30000", "fltdte=::&fltdte1=::"));

        var FilterSection = new FilterSection();
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "20px");
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%");
        FilterSection.Wrap.SetStyle(HtmlStyles.padding, string.Empty);
        FilterSection.FilterHtml = filterText.HtmlText + filterText1.HtmlText + filterBtn.HtmlText;

        var Wrap = new Wrap();
        Wrap.SetAttribute(HtmlAttributes.id, "RptRlt");
        Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
        Wrap.SetStyle(HtmlStyles.paddingTop, "8px");
        Wrap.SetStyle(HtmlStyles.width, "100%");

        rptHtml = FilterSection.HtmlText + Wrap.HtmlText;

        return rptHtml;
    }
    public ApiResponse RptRlt_30000()
    {
        string fltdte = GetDataValue("fltdte");
        string fltdte1 = GetDataValue("fltdte1");
        DateTime dt;

        var _ApiResponse = new ApiResponse();

        if (!DateTime.TryParse(fltdte, out dt) || !DateTime.TryParse(fltdte1, out dt))
        {
            _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"));
        }
        else
        {
            var SQLGridInfo = new SQLGridSection.SQLGridInfo
            {
                Id = "ClockInGrid",
                Name = Translator.Format("clockin"),
                CurrentPageNo = 1,
                LinesPerPage = 50,
                DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                TitleEnabled = true,
                TDictionary = this.HtmlTranslator.TDictionary,
                Query = new SQLGridSection.SQLQuery
                {
                    Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                    OrderBy = new string[] { "a.sysdte desc" },
                    Columns = new string[] { "c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit", "dateadd(hour, convert(int,LocTime), a.sysdte) Local" },
                    ColumnAlias = new string[] { Translator.Format("name"), Translator.Format("email"), Translator.Format("loc"), Translator.Format("dst"), Translator.Format("unit"), Translator.Format("time") },
                    Filters = " convert(varchar(10),dateadd(hour, convert(int,LocTime), a.sysdte),121) between " +
                               " '" + DateTime.Parse(fltdte).ToString("yyyy-MM-dd") + "' and '" + DateTime.Parse(fltdte1).ToString("yyyy-MM-dd") + "' "
                }
            };
            SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
            SetGridStyle(SQLGrid);

            _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText);
        }

        return _ApiResponse;
    }
}