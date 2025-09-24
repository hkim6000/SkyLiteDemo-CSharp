using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;

public class XysPermission : WebBase
{
    public XysPermission()
    {
        ViewFields.AddRange(new[] { new NameFlag { name = "RoleName", flag = true } });
    }

    public override ViewPart InitialModel()
    {
        string SSQL = " Select RoleName From XysRole Where  RoleId = '" + PartialData + "'";
        string emsg = string.Empty;

        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods();
        ViewPart.Mode = ViewMode.Edit;
        ViewPart.Params = PartialData;
        ViewPart.Data = SQLData.SQLDataTable(SSQL, ref emsg);

        for (int i = 0; i < ViewFields.Count; i++)
        {
            ViewPart.Fields.Add(new NameValue { name = ViewFields[i].name, value = ViewPart.ColunmValue(ViewFields[i].name) });
        }

        return ViewPart;
    }

    public override string InitialView()
    {
        var mnulist = SetPageMenu(new string[] { });
        var BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new[] { "save" } : new string[] { });

        var label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = Translator.Format("rolepermission");

        var filter = new FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        var Switch = SwitchUI();
        Switch.SetStyle(HtmlStyles.marginLeft, "8px");

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");
        elmBox.Wrap.SetStyle(HtmlStyles.overflow, "auto");

        elmBox.AddItem(Switch, 1);

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;
        return ViewHtml;
    }

    private Wrap SwitchUI()
    {
        string roleid = ViewPart.Params;
        string rolename = SQLData.SQLFieldValue("SELECT dbo.XF_RoleName(N'" + roleid + "')");

        var _Wrap = new Wrap();
        _Wrap.SetStyle(HtmlStyles.margin, "auto");
        _Wrap.SetStyle(HtmlStyles.marginTop, "30px");
        _Wrap.SetStyle(HtmlStyles.marginBottom, "30px");

        string rlt = string.Empty;
        string sSql = " declare @RoleId NVARCHAR(50); set @RoleId = N'" + roleid + "'; " +
                             " select t1, case when t2='' and t3 <> '' then 'Common' else t2 end as t2,t3,t4,tck,tint from dbo.XF_RolePermission(@RoleId) order by tord;";
        DataTable dt = SQLData.SQLDataTable(sSql, ref rlt);
        if (string.IsNullOrEmpty(rlt))
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var HtmlTranslator = new Translator();

                var _tablex = new Grid();
                _tablex.Table.SetAttribute(HtmlAttributes.@class, "table");
                _tablex.Table.SetStyle(HtmlStyles.fontSize, "14px");
                _tablex.Table.SetStyle(HtmlStyles.width, string.Empty);
                _tablex.DataSource(dt);

                for (int i = 0; i < _tablex.Headers.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                        case 5:
                            _tablex.Headers[i].SetStyle(HtmlStyles.display, "none");
                            _tablex.SetColumnStyle(i, HtmlStyles.display, "none");
                            break;
                        case 1:
                            _tablex.Headers[i].InnerText = Translator.Format("area");
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "center");
                            _tablex.SetColumnStyle(i, HtmlStyles.fontWeight, "bold");
                            break;
                        case 2:
                            _tablex.Headers[i].InnerText = Translator.Format("page");
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "left");
                            _tablex.SetColumnStyle(i, HtmlStyles.fontWeight, "bold");
                            break;
                        case 3:
                            _tablex.Headers[i].InnerText = Translator.Format("function");
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "left");
                            break;
                        case 4:
                            _tablex.Headers[i].InnerText = Translator.Format("status");
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "center");
                            break;
                    }
                    _tablex.SetColumnStyle(i, HtmlStyles.whiteSpace, "nowrap");
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string rowid = dt.Rows[i][0].ToString();
                    string rowstatus = dt.Rows[i][4].ToString();
                    string rowtype = dt.Rows[i][5].ToString();

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j == 4)
                        {
                            string jsEvent = "SetRoleRange('XysPermission/SetRoleRange','" + roleid + "','" + rowid + "','" + rowtype + "',this.checked)";

                            var Switch = new Switch();
                            Switch.Id = rowid;
                            Switch.Name = "switch";
                            Switch.Attributes = "onclick:" + jsEvent;
                            Switch.Size = 50;
                            if (rowstatus == "1")
                            {
                                Switch.Checked = true;
                            }

                            _tablex.Rows[i].Columns[j].InnerText = Switch.HtmlText;
                        }
                    }
                }
                _Wrap.InnerText += _tablex.HtmlText;
            }
        }

        return _Wrap;
    }

    public ApiResponse SetRoleRange()
    {
        string roleid = GetDataValue("d");
        string key = GetDataValue("c");
        string keytyp = GetDataValue("t");
        string keyvlu = GetDataValue("s");
        
        List<string> SQL = new List<string>();
        SQL.Add(" DECLARE @RoleId NVARCHAR(50),@PageId NVARCHAR(50),@Key NVARCHAR(50),@KeyTyp INT,@KeyVlu INT,@SYSDTE datetime,@SYSUSR nvarchar(100) " +
                 " SET @RoleId = N'" + roleid + "' " +
                 " SET @PageId = N'' " +
                 " SET @Key = N'" + key + "' " +
                 " SET @KeyTyp = " + ValC(keytyp) + " " +
                 " SET @KeyVlu = " + ValC(keyvlu) + " " +
                 " SET @SYSDTE = getdate() " +
                 " SET @SYSUSR = N'" + AppKey.UserId + "' " +
                 " IF @KeyTyp = 0 " +
                 " BEGIN " +
                 "      DELETE FROM XysRolePage WHERE RoleId=@RoleId AND PageId =@Key " +
                 "      if @KeyVlu = 1 " +
                 "      begin " +
                 "          INSERT INTO XysRolePage(RoleId,PageId,SYSDTE,SYSUSR) values(@RoleId,@Key,@SYSDTE,@SYSUSR) " +
                 "      end " +
                 "      else " +
                 "      begin " +
                 "          DELETE FROM XysRoleMenu WHERE RoleId=@RoleId AND MenuId in (select MenuId from XysMenu where PageId =@Key)  " +
                 "      end " +
                 " END " +
                 " ELSE " +
                 " BEGIN " +
                 "      SET @PageId = isnull((select PageId from XysMenu where MenuId = @Key),'') " +
                 "      DELETE FROM XysRoleMenu WHERE RoleId=@RoleId AND MenuId =@Key " +
                 "      if @KeyVlu = 1 " +
                 "      begin " +
                 "          INSERT INTO XysRoleMenu(RoleId,MenuId,SYSDTE,SYSUSR) values(@RoleId,@Key,@SYSDTE,@SYSUSR) " +
                 "          if not exists(select * from XysRolePage where RoleId=@RoleId and PageId = (select PageId from XysMenu where MenuId = @Key))  " +
                 "          begin " +
                 "              if @PageId <> '' " +
                 "              begin " +
                 "                  INSERT INTO XysRolePage(RoleId,PageId,SYSDTE,SYSUSR) values(@RoleId,@PageId,@SYSDTE,@SYSUSR) " +
                 "              end " +
                 "          end " +
                 "      end " +
                 "      else " +
                 "      begin " +
                 "          if @PageId <> '' " +
                 "          begin " +
                 "              if not exists(select * from XysRoleMenu where RoleId=@RoleId and MenuId in (select MenuId from XysMenu where PageId = @PageId))  " +
                 "              begin " +
                 "                  delete from XysRolePage where RoleId=@RoleId and PageId=@PageId " +
                 "              end " +
                 "          end " +
                 "      end " +
                 " END ");

        string rlt = PutData(SQL);

        var _ApiResponse = new ApiResponse();
        if (!string.IsNullOrEmpty(rlt))
        {
            var dialogBox = new DialogBox(rlt);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
        }

        string SSQL = string.Empty;
        if ( ValC(keytyp) == 0)
        {
            SSQL =
                " DECLARE @RoleId NVARCHAR(50) " +
                " SET @RoleId = N'" + roleid + "' " +
                " select t1,tck from dbo.XF_RolePermission(@RoleId) where t1 in " +
                " ( select PageId from XysPage where PageId = N'" + key + "' " +
                "   union all " +
                "   select MenuId from XysMenu where PageId = N'" + key + "' ) ";
        }
        else
        {
            SSQL =
                " DECLARE @RoleId NVARCHAR(50) " +
                " SET @RoleId = N'" + roleid + "' " +
                " select t1,tck from dbo.XF_RolePermission(@RoleId) where t1 in " +
                " ( select PageId from XysPage where PageId = (select PageId from XysMenu where MenuId = N'" + key + "') " +
                "   union all " +
                "   select MenuId from XysMenu where MenuId = N'" + key + "' ) ";
        }
        DataTable dt = SQLData.SQLDataTable(SSQL, ref rlt);
        if (dt != null && dt.Rows.Count > 0)
        {
            string script = " var elem = document.getElementsByName('switch'); ";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tid = dt.Rows[i][0].ToString();
                string tbool = (dt.Rows[i][1].ToString() == "1") ? "true" : "false";
                script +=
                    " for (var i = 0, j = elem.length; i < j; i++) { " +
                    "    if (elem[i].id == '" + tid + "'){ " +
                    "       elem[i].checked = " + tbool + "; " +
                    "    } " +
                    " } ";
            }

            _ApiResponse.ExecuteScript(script);
        }

        return _ApiResponse;
    }

    public ApiResponse ItemSelected()
    {
        string t = GetDataValue("t");
        var _ApiResponse = new ApiResponse();
        HtmlDocument Html = PartialDocument(References.Pages.XysRoleEV, t);
        _ApiResponse.SetElementContents(References.Elements.PageContents, Html);
        return _ApiResponse;
    }
}