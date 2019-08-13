$(document).ready(function () {

    $("#export-to-excel").on("click", function (e) {
        e.preventDefault();
        var oldAction = $('#work-done-form').attr('action');
        $('#work-done-form').attr('action', "/Report/WorkDoneExportToExcel");
        $('#work-done-form').submit();
        $('#work-done-form').attr('action', oldAction);
    });

    $(".grid tr, .form-grid tr").click(function() {

        if($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        } else {
            $(this).addClass("selected");
        }

    })

    setTimeout(function() {
        $(".message").fadeOut("fast");
    }, 5000)


    $(".message").click(function() {
        $(".message").fadeOut("fast");
    })


    $("#header ul li a").hover(
        function() {
            $(this).parents("ul").prev().addClass("hover");
        },
        function() {
            $("#header ul li a").removeClass("hover");
        }
    )


    $(".print").click(function() {
        window.print();
        return false;
    })


    $(".refresh").click(function() {
        window.location.reload();
        return false;
    })


    $(".form .error, .form .delete, .form .denied, .form .question, .form .info, .form .success").click(function() {
        $(this).children("span").html("");
    })


    $("#about").click(function() {
        $("#about-application").dialog({
            width: 540,
            height: 280,
            modal: true
        });
    })


    $('.autocomplete-with-hidden-field').each(function () { 

        var url = $(this).data("autocomplete-url");

        $(this).autocomplete({ 
            source: function( request, response ) {
            
                $.ajax({
                    url: url,
                    type: "get",
                    dataType: "json",
                    data: { term: request.term },
                    success: function( data ) {
                        response( $.map( data, function( item ) {
                            return {
                                value: item.value,
                                value_id: item.value_id
                            }
                        }));
                    }
                });
            },
        
            minLength: 2,
            select: function( event, ui ) {
                $(this).next().val(ui.item.value_id);
            }
        });
    }); 


    $('.autocomplete').each(function () { 

        var url = $(this).data("autocomplete-url");

        $(this).autocomplete({ 
            source: function( request, response ) {
            
                $.ajax({
                    url: url,
                    type: "get",
                    dataType: "json",
                    data: { term: request.term },
                    success: function( data ) {
                        response( $.map( data, function( item ) {
                            return {
                                value: item.value,
                                value_id: item.value_id
                            }
                        }));
                    }
                });
            },
        
            minLength: 2
        });
    }); 


    $(".grid .delete").click(function() {

        var entry_name  = $(this).parent().parent().children("td:nth-child(3)").children().html();

        if(entry_name == null) {
            entry_name  = $(this).parent().parent().children("td:nth-child(3)").html();
        }

        var url = $(this).attr("href");

        $("body").append("<div id='dialog-confirm' title='Potvrda brisanja' class='error'>Jeste li sigurni da \u017eelite obrisati <b>" + entry_name + "</b>?</div>");

        $("#dialog-confirm").dialog({
			resizable: false,
			height:140,
			modal: true,
			buttons: {
				"Da, obri\u0161i zapis": function() {
					window.location = url;
				},
				"Ne, otka\u017ei": function() {
					$(this).dialog("close");
                    $("#dialog-confirm").remove();
				}
			}
		});

        return false;

    })

    $(".datepicker").datepicker({ 
   
        dateFormat: "dd.mm.yy.",
        yearRange: "1900:+15",
        changeMonth: true,
		changeYear: true,
 
    });

    $(".datetimepicker").datetimepicker({ dateFormat: "dd.mm.yy." });

    // grid filters
    $("#grid-controls select, #grid-controls input").change(function() {
        $(this).parents("form").submit();
    })

});


function DisplayLoader(id) {
        
    var selectBox           = $('#' + id);
    var selectBoxParent     = selectBox.parent();

    var selectBoxHeight     = selectBox.height();
    var selectBoxWidth      = selectBox.width();

    var loaderHeight        = 8;
    var loaderWidth         = 26;

    var selectBoxPadding    = parseInt(selectBox.css("padding").replace("px", ""));
    var selectBoxBorder     = parseInt(selectBox.css("border-width").replace("px", ""));
        
    var top                 = (selectBoxHeight + 2*selectBoxPadding + 2*selectBoxBorder - loaderHeight)/2;
    var left                = (selectBoxWidth + selectBoxPadding + selectBoxBorder - loaderHeight) - 50;

    selectBoxParent.append('<img class="ajax_loader" src="/Content/Themes/Default/Site/Images/select_box_ajax_loader.gif" />');

    $(".ajax_loader").css("top", top);
    $(".ajax_loader").css("left", left);

}


function HideLoader(id) {

    var selectBox           = $('#' + id);
    var selectBoxParent     = selectBox.parent();

    selectBoxParent.find(".ajax_loader").remove();
        
}


function EmptySelectBox(id) {

    var selectBox   = $('#' + id);

    selectBox.empty();
    selectBox.append($('<option/>', {
        value: '',
        text: '----' 
    }));
        
}


function showNextRowIfChecked(id) {

    $("#" + id).click(function() {
        if($("#" + id).is(":checked")) {
            $("#" + id).parent().parent().next(".row").show();
        } else {
            $("#" + id).parent().parent().next(".row").hide();
        }
    })

}


function showTwoNextRowsIfChecked(id) {

    $("#" + id).click(function() {
        if($("#" + id).is(":checked")) {
            $("#" + id).parent().parent().next(".row").show();
            $("#" + id).parent().parent().next(".row").next(".row").show();
        } else {
            $("#" + id).parent().parent().next(".row").hide();
            $("#" + id).parent().parent().next(".row").next(".row").hide();
        }
    })

}


function showCheckboxesGroups(selector, cssClass) {

    $("#" + selector).change(function() {

        if($(this).is(':checked')) {
            $("." + cssClass).attr('checked', true);
        } else {
            $("." + cssClass).attr('checked', false);
        }

    })

    $("." + cssClass).change(function() {

        var number_of_checkboxes = $("." + cssClass).length;
        var number_of_checked_checkboxes = $("." + cssClass).filter(':checked').length;
            
        if($(this).is(':checked') && number_of_checkboxes == number_of_checked_checkboxes) {
            $("#" + selector).attr('checked', true);
        } else {
            $("#" + selector).attr('checked', false);
        }

    })

}