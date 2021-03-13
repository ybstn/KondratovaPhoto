
$(function () {

        $('#sortable').sortable({
            items: 'tr:not(.Unsortable)',
            cursor: 'pointer',
            axis: 'y',
            placeholder:"selectedPlaceholder",
            dropOnEmpty: false,
            helper: fixWidthHelper,
            start: function (e, ui) {
                ui.item.addClass("selected");
            },
            stop: function (e, ui) {
                ui.item.removeClass("selected");
            },
            receive: function (e, ui) {
                $(this).find("tbody").append(ui.item);
            }
        });
        $('.Contactssortable').sortable({
            items: 'tr:not(.Unsortable)',
            cursor: 'pointer',
            axis: 'y',
            placeholder:"selectedPlaceholder",
            connectWith: ".Contactssortable",
            dropOnEmpty: true,
            helper: fixWidthHelper,
            start: function (e, ui) {
                ui.item.addClass("selected");
            },
            stop: function (e, ui) {
                ui.item.removeClass("selected");
                RowsCounter();
            },
            receive: function (e, ui) {
                $(this).find(".Contactssortable").append(ui.item);
            }
        });
    }); 
    function RowsCounter()
{
    var RowsCount = $(".Contactssortable").length;
    var ElemCountInRaws = [RowsCount];
    //-1 потому что шапка у строки есть
    for(let i =0;i<RowsCount;i++)
    {
        ElemCountInRaws[i] = $(".Contactssortable").eq(i).children().length -1;
        $(".RowsAfterMoveClass").eq(i).val(ElemCountInRaws[i]);
    }
}
    function fixWidthHelper(e,ui)
    {
        ui.children().each(function()
        {
        $(this).width($(this).width())
        });
        return ui;
    }
     //scroll в самый низ
     function AddButtonPressed()
     {
         var div = this.document.getElementById("scrollDiv");
         var scroll_position = this.document.getElementById("scroll_position");
         scroll_position.value = div.scrollHeight;
     }
      //scroll на значение до перезагрузки страницы
     window.onload = function () {
         var div = this.document.getElementById("scrollDiv");
         var scroll_position = this.document.getElementById("scroll_position");
         // решение по получению переменной, которое предлагалось в источнике (работает только при включении скрипта в View)
         // var position = this.parseInt('<%=Request.Form["scroll_position"]%>');
         var position = scroll_position.value;
         if (this.isNaN(position)) {
             position = 0;
         }
             window.scrollTo (0,position);
         this.document.body.onscroll = function () {
             scroll_position.value = window.pageYOffset || div.scrollTop || document.body.scrollTop;
         }
     }
//mainPage
    function confirmDeleteMainPage ()
    {
    if (confirm("Удалить ссылку с главной страницы?"))
    {
        return true;
    }
    else
    {
        return false;
    }
    }
//Portfolio, Projects
    function confirmDelete ()
    {
    if (confirm("Удалить альбом?"))
    {
        return true;
    }
    else
    {
        return false;
    }
    }
    function confirmDeleteGalleryImage ()
    {
    if (confirm("Удалить фото из галереи?"))
    {
        return true;
    }
    else
    {
        return false;
    }
    }
//InfoForClients
    function confirmDeleteInfoForClients ()
    {
    if (confirm("Удалить блок информации для клиентов?"))
    {
        return true;
    }
    else
    {
        return false;
    }
    }
//AboutMe
    function confirmDeleteAboutMe ()
    {
    if (confirm("Удалить блок информации обо мне?"))
    {
        return true;
    }
    else
    {
        return false;
    }
    }
//Contacts
    function confirmDeleteContactsLink ()
    {
    if (confirm("Удалить ссылку?"))
    {
        return true;
    }
    else
    {
        return false;
    }
    }
    function confirmDeleteContactsRow ()
    {
    if (confirm("Удалить весь ряд?"))
    {
        return true;
    }
    else
    {
        return false;
    }
    }