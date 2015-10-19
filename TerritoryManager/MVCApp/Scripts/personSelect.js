$(function () {

    $("#personList tr").click(function () {
        var checkbox = $(this).find("input[ajax-selector='personSelectCheckbox']");

        if (!checkbox.is(':disabled')) {
            checkbox.attr('checked', !checkbox.is(':checked'));
            checkbox.trigger("click");
        }

    });

    $("input[ajax-selector='personSelectCheckbox']").click(function (event) {
        // Prevents the event from bubbling up the DOM tree, preventing any parent handlers from being notified of the event.
        // If the prograpation is not stopped, the ("#personList tr") click event is triggered after the current event.
        event.stopPropagation();

        var $input = $(this);
        $input.prop('disabled', true);

        var data = {};
        data['selected'] = $(this).is(':checked');
        data['personId'] = $(this).attr('personId');
        data['districtId'] = $(this).attr('districtId');

        $.ajax({
            url: $(this).data('url'),
            type: 'POST',
            data: data,
            success: function (result) {
                $input.attr('checked', result.selected);
                $input.prop('disabled', false);

                if (result.selected) {
                    $input.parents("tr").addClass('selected');
                }
                else {
                    $input.parents("tr").removeClass('selected');
                }
            },
            error: function (result) {
                // In case of error revert selection done
                $input.attr('checked', !data['selected']);
                $input.prop('disabled', false);
            }
        });
    });

});