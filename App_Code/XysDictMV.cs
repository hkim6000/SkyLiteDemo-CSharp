using skylite;
using skylite.ToolKit;
using System;
using System.Data;

public class XysDictMV : WebBase
{
    private SQLGridSection.SQLGridInfo SQLGridInfo;
    public XysDictMV(){
        SQLGridInfo = new SQLGridSection.SQLGridInfo
        {
            Id = "DataGrid",
            Name = "DataGrid",
            CurrentPageNo = (Convert.ToInt32(ParamValue("DataGrid_PageNo")) == 0 ? 1 : Convert.ToInt32(ParamValue("DataGrid_PageNo"))),
            LinesPerPage = 100,
            ExcludeDownloadColumns = new int[] { 0 },
            TDictionary = HtmlTranslator.TDictionary,
            Query = new SQLGridSection.SQLQuery
            {
                Tables = "XysDict",
                OrderBy = new string[] { "IsoCode", "Target", "KeyWord" },
                Columns = new string[] { "Target", "IsoCode", "KeyWord", "Translated" },
                ColumnAlias = new string[] { Translator.Format("target"),
                                             Translator.Format("isocode"),
                                             Translator.Format("keyword"),
                                             Translator.Format("translated") },
                Filters = (ParamValue("DataGrid_Filter") == string.Empty ? string.Empty : ParamValue("DataGrid_Filter"))
            }
        };
    } 

    public override ViewPart InitialModel()
    {
        return new ViewPart { Methods = ViewMethods("XysDict") };
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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysDictMV/SearchClicked"));

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
            SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

            SQLGrid.Grid.TableColumns[2].SetColumnAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysDictEV + "&t={0}{1}{2}"));
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.textDecoration, "underline");
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.cursor, "pointer");
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

            for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
            {
                switch (i)
                {
                    case 0:
                    case 2:
                    case 3:
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

        SQLGridInfo.Query.Filters = "Target + IsoCode + KeyWord + Translated  like N'%" + sbox + "%' ";
        SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
        SetGridStyle(SQLGrid);

        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid);
        _ApiResponse.StoreLocalValue("sboxtxt", sbox);
        return _ApiResponse;
    }
}