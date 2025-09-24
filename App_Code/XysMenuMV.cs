using skylite;
using skylite.ToolKit;
using System.Data;
using System;

public class XysMenuMV : WebBase
{
    private SQLGridSection.SQLGridInfo SQLGridInfo;
    public XysMenuMV()
    {
        int pageNo;
        int.TryParse(ParamValue("DataGrid_PageNo"), out pageNo);

        SQLGridInfo = new SQLGridSection.SQLGridInfo
        {
            Id = "DataGrid",
            Name = "DataGrid",
            CurrentPageNo = (pageNo == 0 ? 1 : pageNo),
            LinesPerPage = 30,
            ExcludeDownloadColumns = new int[] { 0 },
            TDictionary = this.HtmlTranslator.TDictionary,
            Query = new SQLGridSection.SQLQuery
            {
                Tables = "XysMenu a inner join XysPage b on a.PageId = b.PageId",
                OrderBy = new string[] { "b.PageOrder", "a.MenuArea", "a.MenuOrder" },
                Columns = new string[] { "a.MenuId", "b.PageDesc + ' (<i>' + b.PageName + '</i>)' as PageName", "a.MenuDesc", "a.MenuArea", "a.MenuMethod", "a.MenuOrder", "a.MenuUse" },
                ColumnAlias = new string[] { "",
                                            Translator.Format("page"),
                                            Translator.Format("mdesc"),
                                            Translator.Format("area"),
                                            Translator.Format("method"),
                                            Translator.Format("order"),
                                            Translator.Format("use") },
                Filters = string.IsNullOrEmpty(ParamValue("DataGrid_Filter")) ? string.Empty : ParamValue("DataGrid_Filter")
            }
        };
    } 

    public override ViewPart InitialModel()
    {
        return new ViewPart { Methods = ViewMethods("XysMenu") };
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] { });
        Wrap BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new string[] { "save" } : new string[] { });

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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysMenuMV/SearchClicked"));

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
        elmBox.SetStyle(HtmlStyles.overflow, "auto");
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px");

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
            SQLGrid.Grid.TableColumns[0].SetHeaderStyle(HtmlStyles.display, "none");
            SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.display, "none");

            SQLGrid.Grid.TableColumns[2].SetColumnAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysMenuEV + "&t={0}"));
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.textDecoration, "underline");
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.cursor, "pointer");

            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

            SQLGrid.Grid.TableColumns[3].SetColumnFormat("@R {3} | X.Method, M.Menu, B.Button");
            SQLGrid.Grid.TableColumns[6].SetColumnFormat("@R {6} | 0. , 1.✓");

            for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
            {
                switch (i)
                {
                    case 1:
                    case 2:
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

        SQLGridInfo.Query.Filters = "b.PageName + b.PageDesc + MenuDesc + MenuMethod like N'%" + sbox + "%' ";
        SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
        SetGridStyle(SQLGrid);

        ApiResponse apiResponse = new ApiResponse();
        apiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid);
        apiResponse.StoreLocalValue("sboxtxt", sbox);
        return apiResponse;
    }
}