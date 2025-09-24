using skylite;
using skylite.ToolKit;
using System.Data;
using System.Linq;  

public class XysFileMV : WebBase
{
    private SQLGridSection.SQLGridInfo SQLGridInfo;
    public XysFileMV() {
        SQLGridInfo = new SQLGridSection.SQLGridInfo
        {
            Id = "FileGrid",
            Name = Translator.Format("Files"),
            CurrentPageNo = 1,
            LinesPerPage = 50,
            ExcludeDownloadColumns = new[] { 0 },
            DisplayCount = SQLGridSection.DisplayCounts.FilteredNAll,
            TitleEnabled = true,
            TDictionary = HtmlTranslator.TDictionary,
            Query = new SQLGridSection.SQLQuery
            {
                Tables = "XysFile",
                OrderBy = new[] { "FileRef", "FileName" },
                Columns = new[] {
                "FileId", "FileRef", "FileRefId", "FileName", "FilePath",
                "dbo.XF_OffetTime(SYSDTE," + CSTimeOffset.ToString() + ") SYSDTE",
                "dbo.XF_UserName(SYSUSR) SYSUSR", "'Delete' [Delete]"
            },
                ColumnAlias = new[] {
                Translator.Format("FileId"),
                Translator.Format("Reference Area"),
                Translator.Format("FileRefId"),
                Translator.Format("File Name"),
                Translator.Format("File GUID"),
                Translator.Format("Uploaded"),
                Translator.Format("By"),
                " "
            },
                Filters = ""
            }
        };
    }
     

    public override ViewPart InitialModel()
    {
        return new ViewPart { Methods = ViewMethods("XysFile") };
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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysFileMV/SearchClicked"));

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
        SQLGrid.Wrap.SetStyle(HtmlStyles.width, "Calc(100% - 14px)");
        SQLGrid.Wrap.SetStyle(HtmlStyles.boxSizing, "border-box");

        if (SQLGrid.GridData != null)
        {
            for (int i = 0; i < SQLGrid.GridData.Rows.Count; i++)
            {
                SQLGrid.GridData.Rows[i][4] = SQLGrid.GridData.Rows[i][4].ToString().Split('\\').Last().Split('.').FirstOrDefault();
            }

            SQLGrid.Grid.TableColumns[0].SetHeaderStyle(HtmlStyles.display, "none");
            SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.display, "none");
            SQLGrid.Grid.TableColumns[2].SetHeaderStyle(HtmlStyles.display, "none");
            SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.display, "none");

            SQLGrid.Grid.TableColumns[7].SetColumnAttribute(HtmlEvents.onclick, ByPassCall("XysFile/RemoveFile", "t={0}"));
            SQLGrid.Grid.TableColumns[7].SetColumnStyle(HtmlStyles.textDecoration, "underline");
            SQLGrid.Grid.TableColumns[7].SetColumnStyle(HtmlStyles.cursor, "pointer");
            SQLGrid.Grid.TableColumns[7].SetColumnStyle(HtmlStyles.color, "#ff6600");

            SQLGrid.Grid.TableColumns[5].SetColumnFormat("@D {5} |" + DateFormat);

            SQLGrid.Grid.TableColumns[3].SetColumnStyle(HtmlStyles.fontSize, "12px");
            SQLGrid.Grid.TableColumns[4].SetColumnStyle(HtmlStyles.fontSize, "12px");
            SQLGrid.Grid.TableColumns[5].SetColumnStyle(HtmlStyles.fontSize, "12px");
            SQLGrid.Grid.TableColumns[6].SetColumnStyle(HtmlStyles.fontSize, "12px");

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

        SQLGridInfo.Query.Filters = "FileId+FileRef+FileName like N'%" + sbox + "%' ";
        SQLGridSection SQLGrid = new SQLGridSection(SQLGridInfo);
        SetGridStyle(SQLGrid);

        ApiResponse _ApiResponse = new ApiResponse();
        _ApiResponse.ReplaceElement("FileGrid", SQLGrid.HtmlText);
        _ApiResponse.PopOff();
        return _ApiResponse;
    }
}