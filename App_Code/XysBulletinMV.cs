using System; 
using System.Data; 
using skylite; 
using skylite.ToolKit;

public class XysBulletinMV : WebBase 
{
    private SQLGridSection.SQLGridInfo SQLGridInfo;

    public XysBulletinMV() 
    {
        int pageNo;
        int.TryParse(ParamValue("DataGrid_PageNo"), out pageNo);

        SQLGridInfo = new SQLGridSection.SQLGridInfo
        {
            Id = "DataGrid",
            Name = "DataGrid",
            CurrentPageNo = (pageNo == 0 ? 1 : pageNo),
            LinesPerPage = 10,
            ExcludeDownloadColumns = new int[] { 0 },  
            TDictionary = HtmlTranslator.TDictionary, 
            Query = new SQLGridSection.SQLQuery
            {
                Tables = "XysBulletin",
                OrderBy = new string[] { "SYSDTE desc" },  
                Columns = new string[] {
                    "BltnId", "BltnTitle", "BltnMemo", "CreatedBy",
                    "dbo.XF_FileDownListEnc(FileRefId,'#') FileRefId",
                    "dbo.XF_OffetTime(SYSDTE," + CSTimeOffset.ToString() + ") SYSDTE" 
                },
                ColumnAlias = new string[] {
                    Translator.Format("no"),
                    Translator.Format("title"),
                    Translator.Format("contents"),
                    Translator.Format("publisher"),
                    Translator.Format("attached"),
                    Translator.Format("modified")
                },
                Filters = (string.IsNullOrEmpty(ParamValue("DataGrid_Filter")) ? string.Empty : ParamValue("DataGrid_Filter"))
            }
        };
    }

    public override ViewPart InitialModel()
    {
        return new ViewPart { Methods = ViewMethods("XysBulletin") };
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] { });  
        Wrap BtnWrap = SetPageButtons(new string[] { });

        Texts filterText = new Texts(TextTypes.text);
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px");
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "4px");
        filterText.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
        filterText.Text.SetStyle(HtmlStyles.fontSize, "16px");
        filterText.Text.SetStyle(HtmlStyles.height, "24px");
        filterText.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("searchterm"));
        filterText.Text.SetAttribute(HtmlAttributes.id, "sbox");
        filterText.Text.SetAttribute(HtmlAttributes.value, ParamValue("sboxtxt"));

        Button filterBtn = new Button();
        filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')");
        filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat");
        filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px");
        filterBtn.SetStyle(HtmlStyles.borderRadius, "50%");
        filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd");
        filterBtn.SetStyle(HtmlStyles.padding, "6px");
        filterBtn.SetStyle(HtmlStyles.height, "30px");
        filterBtn.SetStyle(HtmlStyles.width, "30px");
        filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)");
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysBulletinMV/SearchClicked"));

        FilterSection filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = filterText.HtmlText + filterBtn.HtmlText;  

        SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
        SetGridStyle(SQLGrid);

        HtmlElementBox elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");
        elmBox.Wrap.SetStyle(HtmlStyles.overflow, "auto");

        elmBox.AddItem(SQLGrid);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;  

        return ViewHtml;
    }

    private void SetGridStyle(SQLGridSection SQLGrid)
    {
        SQLGrid.Wrap.SetStyle(HtmlStyles.margin, string.Empty);  
        SQLGrid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
        SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block");

        if (SQLGrid.GridData != null)
        {
            SQLGrid.Grid.TableColumns[1].SetColumnAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysBulletinEV + "&t={0}"));
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.textDecoration, "underline");
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.cursor, "pointer");
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.minWidth, "140px");

            SQLGrid.Grid.TableColumns[3].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
            SQLGrid.Grid.TableColumns[4].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
            SQLGrid.Grid.TableColumns[4].SetColumnStyle(HtmlStyles.fontSize, "12px");
            SQLGrid.Grid.TableColumns[5].SetColumnStyle(HtmlStyles.fontSize, "12px");
            SQLGrid.Grid.TableColumns[3].SetColumnStyle(HtmlStyles.fontSize, "12px");
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.fontSize, "12px");

            SQLGrid.Grid.TableColumns[4].SetColumnFormat("@E #|{4}");
            SQLGrid.Grid.TableColumns[5].SetColumnFormat("@D {5} |" + DateFormat);

            for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)  
            {
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "left");
                        break; 
                    default:  
                        SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "center");
                        break;
                }
            }
        }
    }

    public ApiResponse SearchClicked()
    {
        string sbox = ParamValue("sbox");

        SQLGridInfo.Query.Filters = @"BltnTitle+BltnMemo+CreatedBy+dbo.XF_UserName(SYSUSR) like N'%{sbox}%'";

        SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
        SetGridStyle(SQLGrid);

        ApiResponse apiResponse = new ApiResponse(); 
        apiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid);
        apiResponse.StoreLocalValue("sboxtxt", sbox);
        return apiResponse;
    }
}