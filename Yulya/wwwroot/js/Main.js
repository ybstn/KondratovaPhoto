var MenuXDistance ='30vw';
var MenuXDistanceWide ='40vw';
var XDistance='';

function ServiceGroupMouseUP(sender) 
{
location.replace("/");
}

function OpenWhatsApp(sender) 
{
location.assign(sender);
}
function ShowSendFormButtonClick(sender) {
    
    var CloseSendFormDiv = document.getElementsByClassName("CloseSendFormDiv");
    CloseSendFormDiv[0].style.display ="block";
    var SendForm = document.getElementsByClassName("SendForm");
    SendForm[0].style.display ="block";
    SendForm[0].style["pointer-events"] ="initial";
    var AllDiv = document.getElementsByClassName("AllBlackout");
    AllDiv[0].style["filter"] ="blur(20px)";
    return false;
}



function CloseSendFormButtonClick()
{
    var SendForm = document.getElementsByClassName("SendForm");
    SendForm[0].style.display ="none";
    SendForm[0].style["pointer-events"] ="none";
    var CloseSendFormDiv = document.getElementsByClassName("CloseSendFormDiv");
    CloseSendFormDiv[0].style.display ="none";
    var AllDiv = document.getElementsByClassName("AllBlackout");
    AllDiv[0].style["filter"] ="none";

}
function ShowDiscountsPuttonFX()
{
}
function ShowDiscountsForm()
{
    var CloseDiscountsForm = document.getElementsByClassName("CloseDiscountsForm");
    CloseDiscountsForm[0].style.display ="block";
    var DiscountsForm = document.getElementsByClassName("DiscountsForm");
    DiscountsForm[0].style.display ="block";
    DiscountsForm[0].style["pointer-events"] ="initial";
    var AllDiv = document.getElementsByClassName("AllBlackout");
    AllDiv[0].style["filter"] ="blur(20px)";
    return false;
}

function CloseDiscountsForm()
{
    var DiscountsForm = document.getElementsByClassName("DiscountsForm");
    DiscountsForm[0].style.display ="none";
    DiscountsForm[0].style["pointer-events"] ="none";
    var CloseDiscountsForm = document.getElementsByClassName("CloseDiscountsForm");
    CloseDiscountsForm[0].style.display ="none";
    var AllDiv = document.getElementsByClassName("AllBlackout");
    AllDiv[0].style["filter"] ="none";
}

function BackButtMouseUp()
{
    history.back();
}
function MenuButtMouseDown()
{
    var MenuButtons = document.getElementsByClassName("MenuButton");
 
}

function MenuButtMouseUp()
{
    XDistance = MenuXDistance;
    var BrowserWindowWidth =$( window).width();
    if (BrowserWindowWidth<=550)
    {
        XDistance = MenuXDistanceWide;
    }

    
    var RightMenus = document.getElementsByClassName("RightMenuContainer");
    var MenuRightBlackouts = document.getElementsByClassName("MenuRightBlackout");
    var BodyPanels = document.getElementsByClassName("MainPanel");
    var AllBlackout = document.getElementsByClassName("AllBlackout");
   
     MenuRightBlackouts[0].style.zIndex='199';
    // MenuRightBlackouts[0].style.right = XDistance;
    MenuRightBlackouts[0].style.width='100%';
    MenuRightBlackouts[0].style.opacity='1';
    BodyPanels[0].style.right = null;
    // BodyPanels[0].style.right = XDistance;
    RightMenus[0].style.width = XDistance;
   RightMenus[0].style.right = '0';
    // AllBlackout[0].style.height='100vh';
}

function RightMenuBlackoutMouseUp()
{   
    var MenuRightBlackouts = document.getElementsByClassName("MenuRightBlackout");
    var RightMenus = document.getElementsByClassName("RightMenuContainer");
    var BodyPanels = document.getElementsByClassName("MainPanel");
    var AllBlackout = document.getElementsByClassName("AllBlackout");
    
    MenuRightBlackouts[0].style.opacity='0';
    // MenuRightBlackouts[0].style.right = '0px';
    MenuRightBlackouts[0].style.zIndex='-199';
    MenuRightBlackouts[0].style.width='0%';
    // BodyPanels[0].style.right = '0';
    RightMenus[0].style.right = null;
    RightMenus[0].style.width = '0';
   RightMenus[0].style.right = '-'+XDistance;
//    AllBlackout[0].style.height ='100%';
}
function PricesMouseUp()
{
    var SubMenu = document.getElementsByClassName("SubMenuContainer");
    
    var SM = SubMenu[0];
    if (SM.clientHeight!=0)
    {
        SubMenu[0].style.height='0vw';
       
    }
    else
    {
    SubMenu[0].style.height='auto';
   
    }
}

$(window).resize(function () {
    // var RightMenu = document.getElementById("MenuRight");
    // RightMenu.style.width = '0vw';
   var BrowserWindowWidth =$( window).width();
   if (BrowserWindowWidth>=560)
   {
    //RightMenuBlackoutMouseUp();
   }

});
// var MenuRightBlackouts = document.getElementsByClassName("MenuRightBlackout");
// var MenuRightBlackout = MenuRightBlackouts[0];
// $(MenuRightBlackout).resize(function () {
   
//     var BlackOutWidth =$(MP_MenuRightBlackout).width();
//     if (BlackOutWidth<1)
//     {
//      var RightMenus = document.getElementsByClassName("RightMenuContainer");
//      RightMenus[0].style.width = '0vw';
//     }
 
//  });