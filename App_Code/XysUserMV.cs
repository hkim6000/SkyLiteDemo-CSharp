using skylite;
using skylite.ToolKit;
using System;
using System.Data;

public class XysUserMV : WebBase
{
    private SQLGridSection.SQLGridInfo SQLGridInfo;

    public XysUserMV()
    {
        SQLGridInfo = new SQLGridSection.SQLGridInfo()
        {
            Id = "DataGrid",
            Name = "DataGrid",
            CurrentPageNo = Convert.ToInt32( ValC(ParamValue("DataGrid_PageNo")) == 0 ? 1 : ValC(ParamValue("DataGrid_PageNo"))),
            LinesPerPage = 50,
            ExcludeDownloadColumns = new int[] { 0 },
            TDictionary = this.HtmlTranslator.TDictionary,
            Query = new SQLGridSection.SQLQuery
            {
                Tables = "XysUser a inner join XysUserInfo b on a.UserId = b.UserId ",
                OrderBy = new string[] { "dbo.XF_RoleOrder(b.RoleId)", "b.UserName" },
                Columns = new string[] { "a.UserId", "b.UserName", "dbo.XF_RoleAlias(b.RoleId) RoleAlias", "b.UserPhone", "b.UserEmail", "a.UserOTP", "dbo.XF_LevelName(a.LevelCode) Level", "a.UserStatus" },
                ColumnAlias = new string[] {
                    "",
                    Translator.Format("username"),
                    Translator.Format("rolealias"),
                    Translator.Format("userphone"),
                    Translator.Format("useremail"),
                    Translator.Format("userotp"),
                    Translator.Format("userlevel"),
                    Translator.Format("userstatus")
                },
                Filters = string.IsNullOrEmpty(ParamValue("DataGrid_Filter")) ? string.Empty : ParamValue("DataGrid_Filter")
            }
        };
    }

    public override ViewPart InitialModel()
    {
        return new ViewPart { Methods = ViewMethods("XysUser") };
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
        filterText.Text.SetAttribute(HtmlAttributes.name, "sbox");

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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysUserMV/SearchClicked"));

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

            SQLGrid.Grid.TableColumns[1].SetColumnAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysUserEV + "&t={0}"));
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.textDecoration, "underline");
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.cursor, "pointer");
            SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

            SQLGrid.Grid.TableColumns[5].SetColumnFormat("@R {5} | 0. , 1.✓");
            SQLGrid.Grid.TableColumns[7].SetColumnFormat("@R {7} | 0.Normal, 8.Suspended, 9.Terminated");

            for (int i = 0; i <= SQLGrid.Grid.TableColumns.Count - 1; i++)
            {
                switch (i)
                {
                    case 0:
                    case 1:
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

        SQLGridInfo.Query.Filters = "b.UserName + dbo.XF_RoleAlias(b.RoleId)+ b.UserEmail+ b.UserPhone like N'%" + sbox + "%' ";
        SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
        SetGridStyle(SQLGrid);

        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid);
        _ApiResponse.StoreLocalValue("sboxtxt", sbox);
        return _ApiResponse;
    }
}