using skylite;
using skylite.ToolKit;

public class XysAuth : WebBase 
{
    public override void OnBeforRender()
    {
        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
    }

    public override string InitialView()
    {
        var Title = new Label(Translator.Format("verify"));
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

        var text = new Texts(Translator.Format("pinno"), "pin", TextTypes.text);
        text.Text.SetAttribute(HtmlAttributes.name, "pin"); 
        text.Text.SetStyle(HtmlStyles.width, "330px");
        text.Text.SetAttribute(HtmlAttributes.maxlength, "5");

        var lbl1 = new Label(Translator.Format("sentemail"));
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

        var btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
        btn.SetAttribute(HtmlAttributes.@class, "button");
        btn.SetAttribute(HtmlEvents.onclick, "NavXysHome()"); 

        var btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
        btn1.SetStyle(HtmlStyles.marginLeft, "12px");
        btn1.SetAttribute(HtmlAttributes.@class, "button1");
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignIn()");

        var elmBox = new HtmlElementBox();
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
        elmBox.SetStyle(HtmlStyles.position, "relative");
        elmBox.SetStyle(HtmlStyles.margin, "auto");
        elmBox.SetStyle(HtmlStyles.width, "500px");

        elmBox.AddItem(Title, 14);
        elmBox.AddItem(lbl1, 30);
        elmBox.AddItem(text, 40);
        elmBox.AddItem(btn);
        elmBox.AddItem(btn1, 10);

        
        return elmBox.HtmlText;
    }

    public ApiResponse NavXysSignIn()
    {
        var _ApiResponse = new ApiResponse();
        _ApiResponse.Navigate(References.Pages.XysSignin);
        return _ApiResponse;
    }

    public ApiResponse NavXysHome()
    {
        string pin = GetDataValue("pin");
        var _ApiResponse = new ApiResponse();

        if (!string.IsNullOrEmpty(pin))
        {
            _ApiResponse.Navigate(References.Pages.XysHome);
        }
        else
        {
            var dialogBox = new DialogBox(Translator.Format("enterpin"));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            // Use PopUpWindow to display a modal dialog.
            _ApiResponse.PopUpWindow(dialogBox.HtmlText);
        }
        return _ApiResponse;
    }
}