var ImgSliders = document.getElementsByClassName("GalleryPanel");
var ImgSlider = ImgSliders[0];
var ImgSliderMasters = document.getElementsByClassName("GalleryMasterDiv");
var ImgSliderMaster = ImgSliderMasters[0];
// var ImgSlider = document.getElementById("ctl00_masterPageContainer_SliderPanel");
// var ImgSliderMaster = document.getElementById("ctl00_masterPageContainer_EventImageMasterDiv");
var ImgDiv=null;
var ClickedSlide= null;
var BuffOverImgDiv = null;
var BuffbuutDiv = null;
var BuffIfrmSRC = null;
var BuffIfrEvId = null;
var TRRect = 0;
var TLRect = 0;
var ImgSliderWidth = 0;
var SliderCenter = 0;
var ItemsCount = ImgSliderMaster.childElementCount - 1;
var CurImageNumb = 1;
var BufferTrigger = true;
var mouseDirection = 0;
var translatX =0;
var translXBuffer = 0;
var moUpX = 0;
var moUpY = 0;

var PlayInterval = setInterval(autoplay,5000);
ImgSliderMaster.addEventListener('touchstart', touch2Mouse, false)
ImgSliderMaster.addEventListener('touchmove', touch2Mouse, false)
ImgSliderMaster.addEventListener('touchend', touch2Mouse, false)

function touch2Mouse(e)
{
  var theTouch = e.changedTouches[0];
  var mouseEv;

  switch(e.type)
  {
    case "touchstart": mouseEv="mousedown"; break;  
    case "touchend":   mouseEv="mouseup"; break;
    case "touchmove":  mouseEv="mousemove"; break;
    default: return;
  }

  var mouseEvent = document.createEvent("MouseEvent");
  mouseEvent.initMouseEvent(mouseEv, true, true, window, 1, theTouch.screenX, theTouch.screenY, theTouch.clientX, theTouch.clientY, false, false, false, false, 0, null);
  theTouch.target.dispatchEvent(mouseEvent);

   e.preventDefault();
}

function moseDownInteraval()
{
    window.removeEventListener('mousemove', moveImage, true);
    window.removeEventListener('mouseup', mouseUp, false);
}

function autoplay()
{ 
   // NextImgSliderDwn();
  //  setTimeout(NextImgSliderUp,100);
   
}

function NextImgSliderDwn() {
    clearInterval(PlayInterval);
    if (CurImageNumb == (ItemsCount -1)) {
       
        var ImgDivs = document.getElementsByClassName("GallerySlideContainer");
        ImgSliderWidth = ImgDivs[0].offsetWidth;
       ImgSliderMaster.style.transition =null;
       ImgSliderMaster.style.transition = "none";
        translXBuffer = 0;
        CurImageNumb = 0;
        translatX = "translate(" + translXBuffer.toString() + "px)";
        ImgSliderMaster.style.transform = translatX;
    }
   
}
function NextImgSliderUp() {
    ImgSliderMaster.style.transition = "all 0.2s ease";
    var ImgDivs = document.getElementsByClassName("GallerySlideContainer");
    ImgSliderWidth = ImgDivs[0].offsetWidth;
    CurImageNumb++;
    translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
    translatX = "translate(" + translXBuffer.toString() + "px)";
    ImgSliderMaster.style.transform = translatX;

    if (BuffOverImgDiv != null) {
        BuffbuutDiv.style.display = "block";
        BuffOverImgDiv.style.display = "inline-block";
        BuffIfrEvId.src = BuffIfrmSRC;
        BuffOverImgDiv = null;
        BuffIfrEvId = null;
        BuffbuutDiv = null;
    }
   PlayInterval = setInterval(autoplay,5000);
}
function PrevImgSliderDwn() {
    clearInterval(PlayInterval);
    if (CurImageNumb == 1) {
        var ImgDivs = document.getElementsByClassName("GallerySlideContainer");
        ImgSliderWidth = ImgDivs[0].offsetWidth;
        ImgSliderMaster.style.transition =null;
        ImgSliderMaster.style.transition = "none";
        CurImageNumb = ItemsCount;
        translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
        translatX = "translate(" + translXBuffer.toString() + "px)";
        ImgSliderMaster.style.transform = translatX;
    }
}
function PrevImgSliderUp() {
    ImgSliderMaster.style.transition = "all 0.2s ease";
    var ImgDivs = document.getElementsByClassName("GallerySlideContainer");
  
    ImgSliderWidth = ImgDivs[0].offsetWidth;
    CurImageNumb--;
    translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
    translatX = "translate(" + translXBuffer.toString() + "px)";
    ImgSliderMaster.style.transform = translatX;

    if (BuffOverImgDiv != null) {
        BuffbuutDiv.style.display = "block";
        BuffOverImgDiv.style.display = "inline-block";
        BuffIfrEvId.src = BuffIfrmSRC;
        BuffOverImgDiv = null;
        BuffIfrEvId = null;
        BuffbuutDiv = null;
    }
    PlayInterval = setInterval(autoplay,5000);
}

function CloseGallery()
{
    history.back();
}

function ImageMouseDown(e, SelectedImgNumb) {
   clearInterval(PlayInterval);
   
    var ImgDivs = document.getElementsByClassName("GallerySlideContainer");
    ImgDiv = ImgDivs[SelectedImgNumb];
    var ImgDivsBlackout = document.getElementsByClassName("GallerySlideBlckout");
    ImgDivBlackout = ImgDivsBlackout[SelectedImgNumb];
        moUpX = e.pageX;
        moUpY = e.pageY;
        ImgDivBlackout.style.cursor = "-webkit-grabbing";
        CurImageNumb = SelectedImgNumb;
        ImgSliderWidth = ImgSlider.offsetWidth;
        translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
        ClickedSlide= ImgDivBlackout;
        TRRect = ImgSlider.getBoundingClientRect().right;
        TLRect = ImgSlider.getBoundingClientRect().left;
    window.addEventListener('mousemove', moveImage, true);
    window.addEventListener('mouseup', mouseUp, false);
    MouseOrTouch = false;
    
 
  
}

function moveImage(e) {

    var moY = e.pageY;
    var YpathLength = moY - moUpY;
    
    if (YpathLength >= 50 || YpathLength<= -50) 
    {
        setTimeout(function() {window.scrollTo(0,-YpathLength);},1);
    }
  
  var moX = e.pageX;
    var pathLength = moX - moUpX;
    var SlideSwitchZonesWidth = Math.round(ImgSliderWidth * 0.2);
    if (pathLength != 0) {
        if (pathLength > 0) {
            mouseDirection = 1;
        }
        else {
            mouseDirection = -1;
        }
    }
    var TranslateValue = pathLength - (ImgSliderWidth * CurImageNumb);
    var translatXc = "translate(" + TranslateValue.toString() + "px)";
    ImgSliderMaster.style.transform = translatXc;
    if ((pathLength*mouseDirection) < SlideSwitchZonesWidth){
       ImgSliderMaster.style.transition = "all 0.2s ease";
       
     }
    else {
        if (BufferTrigger) {

            if (CurImageNumb == 1 && mouseDirection == 1) {
               ImgSliderMaster.style.transition = "none";
                CurImageNumb = ItemsCount;
               // translXBuffer = ImgSliderWidth * CurImageNumb * (-1) - pathLength;
               translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
                translatX = "translate(" + translXBuffer.toString() + "px)";
                ImgSliderMaster.style.transform = translatX;
                BufferTrigger = false;

            }
            else {

                if (CurImageNumb == (ItemsCount - 1) && mouseDirection == -1) {
                   ImgSliderMaster.style.transition = "none";
                    translXBuffer = pathLength;
                    CurImageNumb = 0;
                    translatX = "translate(" + translXBuffer.toString() + "px)";
                    ImgSliderMaster.style.transform = translatX;
                    BufferTrigger = false;
                }
                else {

                   ImgSliderMaster.style.transition = "all 0.2s ease";
                    translXBuffer = translXBuffer + (ImgSliderWidth * mouseDirection);
                    CurImageNumb = CurImageNumb - mouseDirection;

                    BufferTrigger = false;
                }
            }
        }

    }
    // if (TranslateValue>0||TranslateValue<(ImgSliderWidth*ItemsCount)*(-1))
    // {
        
    //                 translXBuffer = 0;
    //                 CurImageNumb = 0;
    //                 translatX = "translate(" + translXBuffer.toString() + "px)";
    //                 ImgSliderMaster.style.transform = translatX;
    //                 BufferTrigger = false;
    // }
    
}

function mouseUp(e) {
   
     ImgSliderMaster.style.transition = "all 0.2s ease";
    ClickedSlide.style.cursor = "-webkit-grab";
    window.removeEventListener('mousemove', moveImage, true);
    window.removeEventListener('mouseup', mouseUp, false);
    if (translatX>0||translatX<(ImgSliderWidth*ItemsCount)*(-1))
    {
        
        translXBuffer = ImgSliderWidth*-(1);
        CurImageNumb = 1;
        translatX = "translate(" + translXBuffer.toString() + "px)";
        ImgSliderMaster.style.transform = translatX;
        BufferTrigger = false;
    }
    translatX = "translate(" + translXBuffer.toString() + "px)";
    ImgSliderMaster.style.transform = translatX;
    BufferTrigger = true;
    if (CurImageNumb == 0) {
        CurImageNumb++;
        translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
        translatX = "translate(" + translXBuffer.toString() + "px)";
        ImgSliderMaster.style.transform = translatX;
    }
    if (CurImageNumb == ItemsCount) {
        CurImageNumb--;
        translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
        translatX = "translate(" + translXBuffer.toString() + "px)";
        ImgSliderMaster.style.transform = translatX;
    }
    if (BuffOverImgDiv != null) {
        BuffbuutDiv.style.display = "block";
        BuffOverImgDiv.style.display = "inline-block";
        BuffIfrEvId.src = BuffIfrmSRC;
        BuffOverImgDiv = null;
        BuffIfrEvId = null;
        BuffbuutDiv = null;
    }
   
    PlayInterval = setInterval(autoplay,5000);
}


$(document).ready(function () {
   
});



$(window).resize(function () {
    ImgSliderMaster.style.transition = "none";
    var ImgDivs = document.getElementsByClassName("GallerySlideContainer");
    ImgSliderWidth = ImgDivs[0].offsetWidth;
    translXBuffer = ImgSliderWidth * CurImageNumb * (-1);
    translatX = "translate(" + translXBuffer.toString() + "px)";
    ImgSliderMaster.style.transform = translatX;


});



function PanelResized() {

}

function InfiniteLogic() {

}

function ImageSlidePanelScrolled() {

}