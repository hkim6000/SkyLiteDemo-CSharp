using System;
using System.Data;
using skylite;
using skylite.ToolKit;

public class XysOptionMV : WebBase
{
    private SQLGridSection.SQLGridInfo SQLGridInfo;

    public XysOptionMV()
    {
        SQLGridInfo = new SQLGridSection.SQLGridInfo
        {
            Id = "DataGrid",
            Name = "DataGrid",
            CurrentPageNo = Convert.ToInt32(ParamValue("DataGrid_PageNo")) == 0 ? 1 : Convert.ToInt32(ParamValue("DataGrid_PageNo")),
            LinesPerPage = 50,
            ExcludeDownloadColumns = new int[] { 0 },
            TDictionary = this.HtmlTranslator.TDictionary,
            Query = new SQLGridSection.SQLQuery
            {
                Tables = "XysOption",
                OrderBy = new string[] { "CODE", "SNO" },
                Columns = new string[] { "CODE", "SNO", "SD01", "SD02", "SD03", "SD04", "SD05", "SD06", "SD07" },
                ColumnAlias = new string[] {
                    Translator.Format("code"),
                    Translator.Format("no."),
                    Translator.Format("sd1"),
                    Translator.Format("sd2"),
                    Translator.Format("sd3"),
                    Translator.Format("sd4"),
                    Translator.Format("sd5"),
                    Translator.Format("sd6"),
                    Translator.Format("sd7")
                },
                Filters = string.IsNullOrEmpty(ParamValue("DataGrid_Filter")) ? "CODE='OPTION' and CODE+SD01+SD02+SD03+SD04+SD05+SD06+SD07 like '%%' " : ParamValue("DataGrid_Filter")
            }
        };
    }

    public override ViewPart InitialModel()
    {
        return new ViewPart { Methods = ViewMethods("XysOption") };
    }

    public override string InitialView()
    {
        MenuList mnulist = SetPageMenu(new string[] { });
        Wrap BtnWrap = SetPageButtons(new string[] { });

        Texts filterText = new Texts("", TextTypes.text);
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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysOptionMV/SearchClicked"));

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
            SQLGrid.Grid.TableColumns[0].SetHeaderStyle(HtmlStyles.display, "none");
            SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.display, "none");

            SQLGrid.Grid.TableColumns[1].SetColumnAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysOptionEV + "&t={0}{1}"));
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.textDecoration, "underline");
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.cursor, "pointer");
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

            SQLGrid.Grid.TableColumns[4].SetColumnFormat("@R {4} | 0. , 1.✓");


            for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
            {
                SQLGrid.Grid.TableColumns[i].SetHeaderStyle(HtmlStyles.whiteSpace, "nowrap");
                switch (i)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
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

        SQLGridInfo.Query.Filters = "CODE='OPTION' and CODE+SD01+SD02+SD03+SD04+SD05+SD06+SD07  like '%" + sbox + "%' ";
        SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
        SetGridStyle(SQLGrid);

        ApiResponse apiResponse = new ApiResponse();
        apiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid);
        apiResponse.StoreLocalValue("sboxtxt", sbox);
        return apiResponse;
    }
}