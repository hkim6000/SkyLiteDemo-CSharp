using skylite;
using skylite.ToolKit;
using System.Collections.Generic;
using System.Data;
using System.Net;

public class XysRoleMV : WebBase
{
    public override ViewPart InitialModel()
    {
        string emsg = string.Empty; 
        
        ViewPart = new ViewPart();
        ViewPart.Methods = ViewMethods("XysRole");
        ViewPart.Mode = ViewMode.View;
        ViewPart.Data = SQLData.SQLDataTable(" Select RoleId,RoleName,RoleAlias,RoleOrder From XysRole order by RoleOrder,RoleName", ref emsg);
        return ViewPart;
    }

    public override string InitialView()
    {
        var mnulist = SetPageMenu(new string[] { });
        var BtnWrap = SetPageButtons(ViewPart.Mode == ViewMode.New ? new[] { "save" } : new string[] { });

        var label = new Label();
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
        label.Wrap.InnerText = ViewPart.Mode == ViewMode.New ? Translator.Format("role") : Translator.Format("role");

        var filter = new skylite.ToolKit.FilterSection();
        filter.ModalWrap = true;
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
        filter.Wrap.SetStyle(HtmlStyles.width, "90%");
        filter.Menu = mnulist;
        filter.FilterHtml = label.HtmlText;

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.width, "90%");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.marginTop, "8px");
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 30px 30px 30px");

        List<ViewModel> data = DataTableListT<ViewModel>(ViewPart.Data);
        if (ViewPart.Data != null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var elm = new HtmlTag(HtmlTags.img, HtmlTag.Types.Empty);
                elm.SetAttribute(HtmlAttributes.title, data[i].RoleName);
                elm.SetAttribute(HtmlAttributes.src, ImagePath + "role.jpg");
                elm.SetStyle(HtmlStyles.width, "60px");

                var elm1 = new HtmlTag();
                elm1.SetStyle(HtmlStyles.padding, "6px");
                elm1.InnerText = data[i].RoleName + "<br>("  + data[i].RoleAlias + ")";

                var itmPnl = new ItemPanel();
                itmPnl.Wrap.SetAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m={References.Pages.XysRoleEV}&t={data[i].RoleId}"));
                itmPnl.Wrap.SetAttribute(HtmlAttributes.id, data[i].RoleId);
                itmPnl.Wrap.SetAttribute(HtmlAttributes.@class, "itmPnl");
                itmPnl.Wrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)");
                itmPnl.Wrap.SetStyle(HtmlStyles.minWidth, "100px");
                itmPnl.AddElement(elm, HorizontalAligns.Center);
                itmPnl.AddElement(elm1, HorizontalAligns.Center);
                elmBox.AddItem(itmPnl);
            }
        }

        string ViewHtml = filter.HtmlText + elmBox.HtmlText;

        return ViewHtml;
    }

    private class ViewModel
    {
        public string RoleId = string.Empty;
        public string RoleName = string.Empty;
        public string RoleAlias = string.Empty;
        public string RoleOrder = string.Empty;
    }
}